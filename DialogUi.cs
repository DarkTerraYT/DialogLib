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

namespace DialogLib.Ui
{
    public enum VoiceType
    {
        Low,
        Medium,
        High,
        TenorMale,
        BassMale,
        SopranoFemale,
        MezzoSopranoFemale
    }

    public struct Dialog
    {
        public Dialog(string name, string message, string portrait, int round, VoiceType voice = VoiceType.Medium)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait;
            Round = round;
            Voice = voice;
        }
        public Dialog(string name, string message, SpriteReference portrait, int round, VoiceType voice = VoiceType.Medium)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait.guidRef;
            Round = round - 1;
            Voice = voice;
        }

        public string CharacterName;
        public string Text;
        public string PortraitGUID;

        internal VoiceType Voice;
    
        public int Round { get; }

        public Action OnNext;
        public Action OnThis;
    }

    [RegisterTypeInIl2Cpp]
    public class DialogUi : MonoBehaviour
    {
        float timePerWord = DialogLib.WordSpeed;

        public static DialogUi instance { get; private set; }

        ModHelperPanel mainPanel;

        Dictionary<int, Queue<Dialog>> QueuedDialogPerRound = [];

        Queue<Dialog> currentQueue;
        Queue<Dialog> cachedQueue;
        Dialog? currentDialog;

        ModHelperText text;
        ModHelperImage portrait;
        ModHelperText name;
        ModHelperButton nextButton;
        ModHelperButton exitButton;

        bool close = false;

        public void Close()
        {
            if(gameObject)
            {
                Destroy(gameObject);
            }
        }
        public void Hide()
        {
            mainPanel.SetActive(false);
            close = true;
            currentDialog = null;
            skip = false;
            canGoToNext = false;
            currentQueue = null;
        }

        void Start()
        {
            mainPanel = GetComponent<ModHelperPanel>();
            portrait = transform.GetChild(0).GetComponent<ModHelperImage>();
            name = transform.GetChild(1).GetComponent<ModHelperText>();
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
        void Update()
        {
        }


        bool canGoToNext = false;
        bool skip = false;


        [HideFromIl2Cpp]
        public void ShowDialog(Dialog dialog, float delay = 0)
        {
            MelonCoroutines.Start(ShowDialogCouroutine(dialog, delay));
        }

        public void QueueForRound(int round)
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

        [HideFromIl2Cpp]
        IEnumerator ShowDialogCouroutine(Dialog dialog, float waitTime = 0)
        {
            yield return new WaitForSeconds(waitTime);

            currentDialog = dialog;

            if (close) 
            {   
                close = false;
                yield break;
            }

            mainPanel.SetActive(true);

            name.SetText(dialog.CharacterName);
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

        public void AddToDialogQueue(IEnumerable<Dialog> dialogs)
        {
            foreach (var dialog in dialogs)
            {
                QueuedDialogPerRound.TryAdd(dialog.Round, []);

                var queue = QueuedDialogPerRound[dialog.Round];
                queue.Enqueue(dialog);
            }
        }
        public void AddToDialogQueue(params Dialog[] dialogs)
        {
            AddToDialogQueue((IEnumerable<Dialog>)dialogs);
        }

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
