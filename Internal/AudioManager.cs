using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Internal;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using DialogLib.Ui;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.Audio;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static MelonLoader.MelonLogger;

namespace DialogLib.Internal
{
    internal static class AudioManager
    {
        internal static BloonsMod mod;

        public static Dictionary<string, AudioClip> SoundsByName = [];
        public static AudioFactory factory;

        public static List<AudioClip> GetClipsInBundle(string bundleName)
        {
            List<AudioClip> clips = new();
            foreach (var obj in ResourceHandler.Bundles[bundleName].LoadAllAssets())
            {
                if(obj.IsType<AudioClip>())
                {
                    clips.Add(obj.Cast<AudioClip>());
                }
            }
            return clips;
        }
        public static List<AudioClip> GetClipsInBundle(string modPrefix, string bundleName)
        {
            List<AudioClip> clips = new();
            foreach (var obj in ResourceHandler.Bundles[modPrefix + bundleName].LoadAllAssets())
            {
                if (obj.IsType<AudioClip>())
                {
                    clips.Add(obj.Cast<AudioClip>());
                }
            }
            return clips;
        }
        public static List<AudioClip> GetAllCips(IEnumerable<AssetBundle> bundles)
        {
            List<AudioClip> clips = new();
            foreach (var bundle in bundles)
            {
                foreach (var obj in bundle.LoadAllAssets())
                {
                    if (obj.IsType<AudioClip>())
                    {
                        clips.Add(obj.Cast<AudioClip>());
                    }
                }
            }
            return clips;
        }

        public static void LoadClips(IEnumerable<AudioClip> clips)
        {
            foreach(var clip in clips)
            {
                SoundsByName[clip.name] = clip;
                var reference = new AudioClipReference(clip.name);
                factory.audioClipHandles[reference] = Addressables.Instance.ResourceManager.CreateCompletedOperation(clip, "");
            }
        }

        [HarmonyPatch(typeof(AudioFactory), nameof(AudioFactory.Start))]
        static class AudioFactory_CreateClips
        {
            [HarmonyPostfix]
            public static void Postfix(AudioFactory __instance)
            {
                factory = __instance;
                LoadClips(GetAllCips(DialogLib.GetBundles(mod)));
            }
        }
    }
}
