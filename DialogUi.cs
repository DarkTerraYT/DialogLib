using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using BTD_Mod_Helper.Api;
using MelonLoader;
using DialogLib.Internal;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime.Attributes;
using System;
using BTD_Mod_Helper;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.Bridge;

#pragma warning disable CS0618

namespace DialogLib.Ui
{
    /// <summary>
    /// All of the voices that come with this mod
    /// </summary>
    [Obsolete("Replaced by DialogLib.Voice")]
    public enum VoiceType
    {
        /// <summary>
        /// The provided Low voice
        /// </summary>
        Low,
        /// <summary>
        /// The provided Medium voice
        /// </summary>
        Medium,
        /// <summary>
        /// The provided High voice
        /// </summary>
        High,
        /// <summary>
        /// The provided TenorMale voice
        /// </summary>
        TenorMale,
        /// <summary>
        /// The provided BassMale voice
        /// </summary>
        BassMale,
        /// <summary>
        /// The provided SopranoFemale voice
        /// </summary>
        SopranoFemale,
        /// <summary>
        /// The provided MezzoSopranoFemale voice
        /// </summary>
        MezzoSopranoFemale
    }

    /// <summary>
    /// Struct to hold all data required for a dialog message
    /// </summary>
    public struct Dialog
    {
        /// <summary>
        /// Create a dialog with the specified name, message, portrait, round, and voice
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="message">What the character is saying</param>
        /// <param name="portrait">What the character looks like</param>
        /// <param name="round">The round it appears on</param>
        /// <param name="voice">What this character sounds like</param>
        [Obsolete]
        public Dialog(string name, string message, string portrait, int round, VoiceType voice)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait;
            Round = round;
            Voice = Voice.FromType(voice);
        }
        /// <summary>
        /// Create a dialog with the specified name, message, portrait, round, and voice
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="message">What the character is saying</param>
        /// <param name="portrait">What the character looks like</param>
        /// <param name="round">The round it appears on</param>
        /// <param name="voice">What this character sounds like</param>
        [Obsolete]
        public Dialog(string name, string message, SpriteReference portrait, int round, VoiceType voice)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait.guidRef;
            Round = round - 1;
            Voice = Voice.FromType(voice);
        }
        /// <summary>
        /// Create a dialog with the specified name, message, portrait, round, and voice
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="message">What the character is saying</param>
        /// <param name="portrait">What the character looks like</param>
        /// <param name="round">The round it appears on</param>
        /// <param name="voice">What this character sounds like</param>
        public Dialog(string name, string message, string portrait, int round, Voice voice)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait;
            Round = round;
            Voice = voice;
        }
        /// <summary>
        /// Create a dialog with the specified name, message, portrait, round, and voice
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="message">What the character is saying</param>
        /// <param name="portrait">What the character looks like</param>
        /// <param name="round">The round it appears on</param>
        /// <param name="voice">What this character sounds like</param>
        public Dialog(string name, string message, SpriteReference portrait, int round, Voice voice)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait.guidRef;
            Round = round - 1;
            Voice = voice;
        }
        /// <summary>
        /// Create a dialog with the specified name, message, portrait, and round
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="message">What the character is saying</param>
        /// <param name="portrait">What the character looks like</param>
        /// <param name="round">The round it appears on</param>
        public Dialog(string name, string message, string portrait, int round)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait;
            Round = round;
            Voice = Voice.Silent;
        }
        /// <summary>
        /// Create a dialog with the specified name, message, portrait, and round
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="message">What the character is saying</param>
        /// <param name="portrait">What the character looks like</param>
        /// <param name="round">The round it appears on</param>
        public Dialog(string name, string message, SpriteReference portrait, int round)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait.guidRef;
            Round = round;
            Voice = Voice.Silent;
        }

        /// <summary>
        /// The character's name
        /// </summary>
        public string CharacterName;
        /// <summary>
        /// What the character says
        /// </summary>
        public string Text;
        /// <summary>
        /// GUID of the portrait
        /// </summary>
        public string PortraitGUID;
        /// <summary>
        /// Sprite GUID of the background
        /// </summary>
        public string BackgroundGUID = VanillaSprites.MainBgPanel;
        /// <summary>
        /// Texure2D of the background.
        /// </summary>
        public Texture2D BackgroundTex;
        /// <summary>
        /// Sprite of the background.
        /// </summary>
        public Sprite Background;

        /// <summary>
        /// What the character sounds like
        /// </summary>
        public Voice Voice;
    
        /// <summary>
        /// The round it spawns on
        /// </summary>
        public int Round { get; }

        /// <summary>
        /// What happens when this message is closed
        /// </summary>
        public Action OnNext;
        /// <summary>
        /// What happens when this message is shown
        /// </summary>
        public Action OnThis;
    }

    /// <summary>
    /// UI that handles the dialog, override to make your own dialog. Requires you to make your own create method.
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class DialogUi : MonoBehaviour
    {
        float timePerWord = DialogLib.WordSpeed;

        /// <summary>
        /// Instance of the ui.
        /// </summary>
        public static DialogUi instance { get; private set; }

        ModHelperPanel mainPanel;

        Dictionary<int, Queue<Dialog>> QueuedDialogPerRound = [];

        Queue<Dialog> currentQueue;
        Queue<Dialog> cachedQueue;
        Dialog? currentDialog;

        ModHelperText text;
        ModHelperImage portrait;
        ModHelperText nameText;
        ModHelperButton nextButton;
        ModHelperButton exitButton;

        bool close = false;

        /// <summary>
        /// Destroys the parent of the MonoBehavior, only use when this ui is no longer needed as garbage collection doesn't work due to IL2CPP
        /// </summary>
        public void Close()
        {
            if(gameObject)
            {
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// Hides this ui and sets values back to default, use if the ui will be used again.
        /// </summary>
        public virtual void Hide()
        {
            mainPanel.SetActive(false);
            close = true;
            currentDialog = null;
            skip = false;
            canGoToNext = false;
            currentQueue = null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {
            mainPanel = GetComponent<ModHelperPanel>();
            portrait = transform.GetChild(0).GetComponent<ModHelperImage>();
            nameText = transform.GetChild(1).GetComponent<ModHelperText>();
            text = transform.GetChild(2).GetComponent<ModHelperText>();
            nextButton = transform.GetChild(3).GetComponent<ModHelperButton>();
            exitButton = transform.GetChild(4).GetComponent<ModHelperButton>();

            nextButton.Button.AddOnClick(() =>
            {

                if (!canGoToNext)
                {
                    skip = true;
                    return;
                }
                if (currentQueue == null || currentQueue.Count == 0)
                {
                    Hide();
                    return;
                }
                canGoToNext = false;
                currentDialog?.OnNext?.Invoke();
                ShowDialog(currentQueue.Dequeue());

            });

            exitButton.Button.AddOnClick(() => {
                if (!canGoToNext)
                {
                    skip = true;
                    return;
                }

                Hide();
            });

            nextButton.SetActive(false);
            exitButton.SetActive(false);
            mainPanel.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool canGoToNext = false;
        /// <summary>
        /// 
        /// </summary>
        protected bool skip = false;

        /// <summary>
        /// Shows the provided dialog with the given delay. Use <see cref="QueueForRound(int)"/> to load all dialog in a round.
        /// </summary>
        [HideFromIl2Cpp]
        public void ShowDialog(Dialog dialog, float delay = 0)
        {
            MelonCoroutines.Start(ShowDialogCouroutine(dialog, delay));
        }

        /// <summary>
        /// Propely show all dialogs for the provided round
        /// </summary>
        public virtual void QueueForRound(int round)
        {
            if (QueuedDialogPerRound.ContainsKey(round))
            {
                Dialog dialog;

                currentDialog?.OnNext?.Invoke();
                if (currentQueue != null && currentQueue.Count > 0)
                {
                    while (currentQueue.Count > 0)
                    {
                        dialog = currentQueue.Dequeue();
                        dialog.OnThis?.Invoke();
                        dialog.OnNext?.Invoke();
                    }
                    currentQueue = null;
                }
                currentQueue = QueuedDialogPerRound[round];
                cachedQueue = new(currentQueue);
                if (currentQueue.Count > 0)
                {
                    dialog = currentQueue.Dequeue();

                    close = false;
                    ShowDialog(dialog);
                }
            }
        }

        [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.Continue))]
        static class UnityToSimulation_Continue
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if(instance != null)
                {
                    instance.currentQueue = new(instance.cachedQueue);
                }
            }
        }

        /// <summary>
        /// Actual method that shows the ui. Remember, this is a coroutine!
        /// </summary>
        [HideFromIl2Cpp]
        protected virtual IEnumerator ShowDialogCouroutine(Dialog dialog, float waitTime = 0)
        {
            yield return new WaitForSeconds(waitTime);

            currentDialog = dialog;

            if (close) 
            {   
                close = false;
                yield break;
            }

            if (dialog.Background != null)
            {
                mainPanel.Background.SetSprite(dialog.Background);
            }
            else if (dialog.BackgroundTex != null)
            {
                mainPanel.Background.sprite.SetTexture(dialog.BackgroundTex);
            }
            else if(dialog.BackgroundGUID != null)
            {
                mainPanel.Background.SetSprite(dialog.BackgroundGUID);
            }
            else 
            {
                mainPanel.Background.SetSprite(VanillaSprites.MainBgPanel);
            }

            mainPanel.SetActive(true);

            nameText.SetText(dialog.CharacterName);
            portrait.Image.SetSprite(dialog.PortraitGUID);
            text.SetText("");

            nextButton.SetActive(true);
            exitButton.SetActive(true);
            if (currentQueue == null || currentQueue.Count == 0)
            {
                nextButton.SetActive(false);
            }
            else
            {
                exitButton.SetActive(false);
            }

            string[] words = dialog.Text.Split(' ');
            foreach(string word in words)
            {
                if(skip)
                {
                    text.Text.SetText(dialog.Text);
                    skip = false;
                    break;
                }
                var nextWord = word.Trim();
                bool yield = false;
                if (words.Last() != word)
                {
                    nextWord += " ";
                    yield = true;
                }
                text.Text.SetText(text.Text.text + nextWord);
                DialogLib.PlayRandomDialogSound(dialog.Voice);
                if (yield)
                {
                    yield return new WaitForSeconds(timePerWord);
                    if (skip)
                    {
                        text.Text.SetText(dialog.Text);
                        skip = false;
                        break;
                    }
                }
            }

            canGoToNext = true;
        }

        /// <summary>
        /// Adds the provided dialogs to the queue
        /// </summary>
        public virtual void AddToDialogQueue(IEnumerable<Dialog> dialogs)
        {
            foreach (var dialog in dialogs)
            {
                QueuedDialogPerRound.TryAdd(dialog.Round, []);

                var queue = QueuedDialogPerRound[dialog.Round];
                queue.Enqueue(dialog);
            }
        }
        /// <summary>
        /// Adds the provided dialogs to the queue
        /// </summary>
        public void AddToDialogQueue(params Dialog[] dialogs)
        {
            AddToDialogQueue((IEnumerable<Dialog>)dialogs);
        }

        /// <summary>
        /// Creates the instance of this ui, shows the ui if <see cref="instance"/> isn't null.
        /// </summary>
        public static void CreateInstance()
        {
            if(instance != null)
            {
                instance.Show();
                return;
            }

            ModHelperPanel mainPanel = InGame.instance.mapRect.gameObject.AddModHelperPanel(new("DialogUi", 0, -1000, 2000, 600), VanillaSprites.MainBgPanel);
            var image = mainPanel.AddImage(new("Portrait", -725, 0, 450), VanillaSprites.DartMonkey000); 
            var name = mainPanel.AddText(new("Name", -725, 250, 500, 100), "N/A");
            name.Text.enableAutoSizing = true;
            name.Text.fontSizeMax = 100;
            var text = mainPanel.AddText(new("Text", 250, 0, 1200, 400), "");
            text.Text.enableAutoSizing = true;
            text.Text.fontSizeMax = 60;
            text.Text.alignment = Il2CppTMPro.TextAlignmentOptions.TopLeft;
            var nextButton = mainPanel.AddButton(new("NextBtn", 1000, 0, 200), VanillaSprites.ContinueBtn, new Action(() => { }));
            var closeButton = mainPanel.AddButton(new("NextBtn", 1000, 0, 200), VanillaSprites.CloseBtn, new Action(() => { }));
            instance = mainPanel.AddComponent<DialogUi>();
        }
    }
}
