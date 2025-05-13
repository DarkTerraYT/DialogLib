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

namespace DialogLib.Internal
{
    internal static class AudioManager
    {
        internal static BloonsMod mod;
        static Assembly _asm => mod.MelonAssembly.Assembly;

        public static Dictionary<string, AudioClip> SoundsByName = [];
        public static Dictionary<AudioClip, float> AudioClipLength = [];
        public static Dictionary<VoiceType, List<AudioClip>> SoundsByType = [];
        public static Dictionary<AudioClip, AudioClipReference> ReferenceByClip = [];

        public static AudioClip[] GetClips()
        {
            var clipSources = _asm.GetManifestResourceNames().Where(file => file.EndsWith(".wav")).ToList();

            AudioClip[] clips = new AudioClip[clipSources.Count];

            foreach (var clipSource in clipSources)
            {
                string name = "DialogLib-" + Path.GetFileNameWithoutExtension(clipSource);
                try
                {
                    clips[clipSources.IndexOf(clipSource)] = CreateAudioClip(new WaveFileReader(_asm.GetManifestResourceStream(clipSource)), name);
                }
                catch (Exception e)
                { 
                    mod.LoggerInstance.Warning("Failed to create clip " + name + "!");
                    mod.LoggerInstance.Warning(e);
                }
            }

            return clips;
        }

        public static AudioClip CreateAudioClip(WaveFileReader reader, string id)
        {
            try
            {
                WaveFormat waveFormat = reader.WaveFormat;
                int num = (int)(reader.SampleCount * waveFormat.Channels);
                float[] array = new float[num];
                int num2 = reader.ToSampleProvider().Read(array, 0, num);
                AudioClip audioClip = AudioClip.Create(id, num2 / 2, waveFormat.Channels, waveFormat.SampleRate, stream: false);
                if (audioClip.SetData(array, 0))
                {
                    AudioClipLength[audioClip] = (float)reader.TotalTime.TotalSeconds;
                    return audioClip;
                }

                mod.LoggerInstance.Warning("Failed to set data for audio clip " + id);
            }
            catch (Exception obj)
            {
                mod.LoggerInstance.Warning(obj);
            }

            return null;
        }

        [HarmonyPatch(typeof(AudioFactory), nameof(AudioFactory.Start))]
        static class AudioFactory_CreateClips
        {
            [HarmonyPostfix]
            public static void Postfix(AudioFactory __instance)
            {
                foreach(var clip in GetClips())
                {
                    SoundsByName[clip.name] = clip;
                    VoiceType type = Enum.GetValues<VoiceType>().ToList().Find(e => e.ToString() == clip.name.Split(".")[^2]);
                    SoundsByType.TryAdd(type, []);
                    SoundsByType[type].Add(clip);
                    var reference = new AudioClipReference(clip.name);
                    __instance.audioClipHandles[reference] = Addressables.Instance.ResourceManager.CreateCompletedOperation(clip, "Created by DialogLib -> DO NOT REPORT TO NINJA KIWI IF THIS ERRORS");
                    ReferenceByClip[clip] = reference;
                }
            }
        }
        [HarmonyPatch(typeof(AudioFactory), nameof(AudioFactory.PlaySoundFromUnity))]
        static class AudioFactoryPlaySoundFromUnity_Patch
        {
            public static void Prefix(ref AudioFactory __instance, AudioClip audioClip)
            {
                if (audioClip == null || !ReferenceByClip.ContainsKey(audioClip) || !__instance.audioClipHandles.ContainsKey(ReferenceByClip[audioClip]))
                {

                    foreach (var clip in GetClips())
                    {
                        SoundsByName[clip.name] = clip;
                        VoiceType type = Enum.GetValues<VoiceType>().ToList().Find(e => e.ToString() == clip.name.Split(".")[^2]);
                        SoundsByType.TryAdd(type, []);
                        SoundsByType[type].Add(clip);
                        var reference = new AudioClipReference(clip.name);
                        __instance.audioClipHandles[reference] = Addressables.Instance.ResourceManager.CreateCompletedOperation(clip, "Created by DialogLib -> DO NOT REPORT TO NINJA KIWI IF THIS ERRORS");
                        ReferenceByClip[clip] = reference;
                    }
                }
            }
        }
    }
}
