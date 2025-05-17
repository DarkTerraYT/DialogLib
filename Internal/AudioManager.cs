using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Internal;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.Audio;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using DialogLib.Ui;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BTD_Mod_Helper.Extensions;
using System.Data.SqlTypes;
using CommandLine;

namespace DialogLib.Internal
{
    internal static class AudioManager
    {
        internal static BloonsMod mod;

        static Assembly _asm => mod.MelonAssembly.Assembly;

        public static Dictionary<string, AudioClip> SoundsByName = [];

        public static List<AudioClip> GetClipsInBundle(string bundleName)
        {
            List<AudioClip> clips = new();
            foreach (var obj in ResourceHandler.Bundles[mod.IDPrefix + bundleName].LoadAllAssets())
            {
                if(obj.Cast<AudioClip>())
                {
                    clips.Add(obj.Cast<AudioClip>());
                }
            }
            return clips;
        }

        [HarmonyPatch(typeof(AudioFactory), nameof(AudioFactory.Start))]
        static class AudioFactory_CreateClips
        {
            [HarmonyPostfix]
            public static void Postfix(AudioFactory __instance)
            {
                foreach(var clip in GetClipsInBundle("dialogsounds"))
                {
                    SoundsByName[clip.name] = clip;
                    VoiceType type = Enum.GetValues<VoiceType>().ToList().Find(e => e.ToString() == clip.name.Split(".")[^2]);
                    var reference = new AudioClipReference(clip.name);
                    __instance.audioClipHandles[reference] = Addressables.Instance.ResourceManager.CreateCompletedOperation(clip, "");
                }
            }
        }
    }
}
