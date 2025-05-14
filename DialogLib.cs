using MelonLoader;
using BTD_Mod_Helper;
using DialogLib;
using DialogLib.Internal;
using DialogLib.Ui;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api;
using static MelonLoader.MelonLogger;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

[assembly: MelonInfo(typeof(DialogLib.DialogLib), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace DialogLib;

public class DialogLib : BloonsTD6Mod
{
    public override void OnEarlyInitialize()
    {
        AudioManager.mod = this;
        soundRandom = new(ModHelperData.Name.GetHashCode());
    }

    public override void OnRoundStart()
    {
        if (DialogUi.instance != null)
        {
            ModHelper.Msg<DialogLib>("Dialog UI is not null!");
            DialogUi.instance.QueueForRound(InGame.instance.bridge.GetCurrentRound());
        }
        else
        {
            ModHelper.Warning<DialogLib>("Dialog UI is null!");
        }
    }

    public override void OnMatchStart()
    {
        if (ShowExample)
        {
            if (DialogUi.instance != null)
            {
                DialogUi.instance.AddToDialogQueue(new Dialog("Test 1", "This is a test message. As you can see the words slowly show.", ModContent.GetSpriteReference(this, "Icon"), 1), new("Test 2", "The names can also change. So can the portraits.", ModContent.GetSpriteReference(this, "Icon"), 2), new("Test 3", "It also works on multiple rounds as obviously seen.", ModContent.GetSpriteReference(this, "Icon"), 3), new("Test 4", "You can also...", ModContent.GetSpriteReference(this, "Icon"), 4), new("Test 4", "...Have multiple messages!", ModContent.GetSpriteReference(this, "Icon"), 4), new("Test 5", "That is really all for now. I will add voices later.", ModContent.GetSpriteReference(this, "Icon"), 5));
            }
        }
    }

    public static readonly ModSettingDouble WordSpeed = new(0.1)
    {
        description = "Seconds per word. Decreasing the value will cause words to appear faster."
    };

    public static readonly ModSettingBool ShowExample = new(false)
    {
        description = "Show the example dialogs made to show what this mod can do."
    };

    static bool playingSound = false;

    static IEnumerator SoundCoroutine(float audioLength)
    {
        playingSound = true;
        yield return new WaitForSeconds(audioLength);
        playingSound = false;
    }

    static System.Random soundRandom;
    internal static void PlayRandomDialogSound(VoiceType voice)
    {
        // Disable as of now due to being buggy.

        /*List<AudioClip> clips = AudioManager.SoundsByType[voice];
        var clip = clips[soundRandom.Next(clips.Count)];
        Game.instance.audioFactory.PlaySoundFromUnity(clip, "FX");
        MelonCoroutines.Start(SoundCoroutine(AudioManager.AudioClipLength[clip]));*/
    }
}