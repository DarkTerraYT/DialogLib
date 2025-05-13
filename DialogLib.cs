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

    public static readonly ModSettingDouble wordSpeed = new(0.1)
    {
        description = "Seconds per word. Decreasing the value will cause words to appear faster."
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