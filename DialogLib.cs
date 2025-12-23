using MelonLoader;
using BTD_Mod_Helper;
using DialogLib;
using DialogLib.Internal;
using DialogLib.Ui;
using UnityEngine;
using System.Collections;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper.Api.Internal;
using System.Linq;
using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;

[assembly: MelonInfo(typeof(DialogLib.DialogLib), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace DialogLib;

/// <summary>
/// 
/// </summary>
public class DialogLib : BloonsTD6Mod
{
    /// <summary>
    /// Loads audio clips from a bundle into <see cref="AudioManager"/> which allows the mod to use them. If you're using embedded resources not in a bundle, the clips are automatically loaded.
    /// </summary>
    /// <param name="bundleName">full name of the bundle without the file extension</param>
    public static void LoadClips(string bundleName) 
    {
        AudioManager.LoadClips(AudioManager.GetClipsInBundle(bundleName));
    }
    /// <summary>
    /// Loads audio clips from a bundle into <see cref="AudioManager"/> which allows the mod to use them. If you're using embedded resources not in a bundle, the clips are automatically loaded.
    /// </summary>
    /// <param name="modPrefix"><see cref="BloonsMod.IDPrefix"/> of the mod</param>
    /// <param name="bundleName">name of the bundle without the file extension</param>
    public static void LoadClips(string modPrefix, string bundleName) => LoadClips(modPrefix + bundleName);

    internal static List<AssetBundle> GetBundles(BloonsMod mod)
    {
        List<AssetBundle> bundles = [];
        foreach ((string key, AssetBundle bundle) in ResourceHandler.Bundles)
        {
            if(key.StartsWith(mod.IDPrefix))
            {
                bundles.Add(bundle);
            }
        }

        return bundles;
    }

    /// <summary>
    /// Loads audio clips from every bundle from the provided mod into <see cref="AudioManager"/> which allows this mod to use them. If you're using embedded resources not in a bundle, the clips are automatically loaded.
    /// </summary>
    /// <param name="mod">Mod that added the bundles</param>
    public static void LoadClips(BloonsMod mod)
    {
        AudioManager.LoadClips(AudioManager.GetAllCips(GetBundles(mod)));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnTitleScreen()
    {
        AudioManager.LoadClips(ResourceHandler.AudioClips.Values);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnEarlyInitialize()
    {
        AudioManager.mod = this;
        soundRandom = new(ModHelperData.Name.GetHashCode());
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnRoundStart()
    {
        if (DialogUi.instance != null)
        {
            DialogUi.instance.QueueForRound(InGame.instance.bridge.GetCurrentRound());
        }
        else
        {
            ModHelper.Warning<DialogLib>("Dialog UI is null!");
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnMatchStart()
    {
        if (ShowExample)
        {
            if (DialogUi.instance != null)
            {
                DialogUi.instance.AddToDialogQueue(
                    new Dialog("Test 1", "This is a test message. As you can see the words slowly show.", ModContent.GetSpriteReference(this, "Icon"), 1),
                    new("Test 2", "The names can also change. So can the portraits.", ModContent.GetSpriteReference(this, "Icon"), 2),
                    new("Test 3", "It also works on multiple rounds as obviously seen.", ModContent.GetSpriteReference(this, "Icon"), 3),
                    new("Test 4", "You can also...", ModContent.GetSpriteReference(this, "Icon"), 4),
                    new("Test 4", "...Have multiple messages!",ModContent.GetSpriteReference(this, "Icon"), 4),
                    new("Test 5", "You may also give the dialog a voice, for example this is the provided medium voice", ModContent.GetSpriteReference(this, "Icon"), 5, Voice.Medium),
                    new("Test 6", "The background can also be changed", ModContent.GetSpriteReference(this, "Icon"), 6) { BackgroundGUID = VanillaSprites.MainBGPanelBlue},
                    new("Test 7", "There are also options. Choose one!", ModContent.GetSpriteReference(this, "Icon"), 7) { Options = [new RedOption("No.", new Dialog("Test 7", "That's not very kind!", ModContent.GetSpriteReference(this, "Icon"), 0)), new GreenOption("Okay", new Dialog("Test 7", "That's very kind!", ModContent.GetSpriteReference(this, "Icon"), 0))] },
                    new("Test 8", "That is all for now.", ModContent.GetSpriteReference(this, "Icon"), 8));
            }
        }
    }

    /// <summary>
    /// Seconds per character. Decreasing the value will cause words to appear faster.
    /// </summary>
    public static readonly ModSettingDouble CharacterSpeed = new(0.025f)
    {
        description = "Seconds per word. Decreasing the value will cause words to appear faster."
    };

    /// <summary>
    /// Show the example dialogs made to show what this mod can do.
    /// </summary>
    public static readonly ModSettingBool ShowExample = new(false)
    {
        description = "Show the example dialogs made to show what this mod can do."
    };

    static System.Random soundRandom;
    internal static void PlayRandomDialogSound(Voice voice)
    {
        if (voice.soundGUIDs != null && voice.soundGUIDs.Length > 0)
        {
            AudioClip clip = AudioManager.SoundsByName[voice.soundGUIDs[soundRandom.Next(voice.soundGUIDs.Length)]];
            Game.instance.audioFactory.PlaySoundFromUnity(clip, "FX");
        }
    }
}