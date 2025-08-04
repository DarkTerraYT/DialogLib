using DialogLib.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0618

namespace DialogLib
{
    /// <summary>
    /// Holds sounds for dialog message.
    /// </summary>
    public struct Voice
    {
        // Presets
        /// <summary>
        /// The provided Low voice
        /// </summary>
        public static Voice Low => FromType(VoiceType.Low);
        /// <summary>
        /// The provided Medium voice
        /// </summary>
        public static Voice Medium => FromType(VoiceType.Medium);
        /// <summary>
        /// The provided High voice
        /// </summary>
        public static Voice High => FromType(VoiceType.High);
        /// <summary>
        /// The provided BassMale voice
        /// </summary>
        public static Voice BassMale => FromType(VoiceType.BassMale);
        /// <summary>
        /// The provided TenorMale voice
        /// </summary>
        public static Voice TenorMale => FromType(VoiceType.TenorMale);
        /// <summary>
        /// The provided SopranoFemale voice
        /// </summary>
        public static Voice SopranoFemale => FromType(VoiceType.SopranoFemale);
        /// <summary>
        /// The provided MezzoSopranoFemale voice
        /// </summary>
        public static Voice MezzoSopranoFemale => FromType(VoiceType.MezzoSopranoFemale);
        /// <summary>
        /// The provided Silent voice. Default voice and sounds like nothing
        /// </summary>
        public static Voice Silent => new();

        /// <summary>
        /// Play a random sound in this voice
        /// </summary>
        public void Play()
        {
            if (soundGUIDs.Length > 0)
            {
                DialogLib.PlayRandomDialogSound(this);
            }
        }

        /// <summary>
        /// GUIDs of the voices
        /// </summary>
        public string[] soundGUIDs = [];

        /// <summary>
        /// Creates a voice with the provided sounds
        /// </summary>
        /// <param name="soundGUIDs">GUIDs of the audio clips, doesn't support vanilla audio</param>
        public Voice(params string[] soundGUIDs)
        {
            this.soundGUIDs = soundGUIDs;
        }

        internal Voice(string baseId, int count)
        {
            soundGUIDs = new string[count];

            for (int i = 0; i < count; i++)
            {
                soundGUIDs[i] = $"DialogSound.{baseId}.{i}";
            }
        }

        internal static Voice FromType(VoiceType type)
        {
            return new(type.ToString(), 3);
        }
    }
}
