using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using DialogLib.Internal;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.Attributes;
using Il2CppNinjaKiwi.Common;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618

namespace DialogLib.Ui
{
    /// <summary>
    /// All of the voices that come with this mod
    /// </summary>
    [Obsolete("Replaced by DialogLib.Voice, exists for compatability reasons only.")]
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
        public Dialog(string name, string message, string portrait, int round, Voice voice)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait;
            Round = round - 1;
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
            Round = round - 1;
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
            Round = round - 1;
            Voice = Voice.Silent;
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
        public Dialog(string name, string message, string portrait, int round, VoiceType voice)
        {
            CharacterName = name;
            Text = message;
            PortraitGUID = portrait;
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
        /// Options which get displayed in the bottom right corner of the panel which can provide player interaction
        /// </summary>
        public DialogOption[] Options = [];

        /// <summary>
        /// What happens when this message is closed
        /// </summary>
        public Action OnNext;
        /// <summary>
        /// What happens when this message is shown
        /// </summary>
        public Action OnThis;

        /// <summary>
        /// How fast the text appears (<see cref="DialogLib.CharacterSpeed"/> / dialog.TextSpeed)
        /// </summary>
        public float TextSpeed = 1;

        /// <summary>
        /// After how many characters should a voice sound be played
        /// </summary>
        public int CharactersPerSound = 3;
    }

    /// <summary>
    /// Abstract class for dialog options. Override this to create your own custom options. By default the mod provides <see cref="RedOption"/> and <seealso cref="GreenOption"/>
    /// </summary>
    public abstract class DialogOption
    {
        /// <summary>
        /// Name of the button type. By default the type name.
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// Instance of DialogUi the option will use to access the ui.
        /// </summary>
        protected abstract DialogUi dialogUi { get; }

        /// <summary>
        /// GUID of the sprite of the button
        /// </summary>
        public abstract string Sprite { get; }

        /// <summary>
        /// What text is displayed on the button
        /// </summary>
        public abstract string OptionText { get; }

        /// <summary>
        /// Dialog shown after clicking this option
        /// </summary>
        public abstract Queue<Dialog> SequentialDialog { get; set; }

        /// <summary>
        /// Show the sequential dialog for this option
        /// </summary>
        public virtual void Click()
        {
            dialogUi.ShowQueue(SequentialDialog);
        }
        
        /// <summary>
        /// Get the button for this option
        /// </summary>
        /// <returns></returns>
        public virtual ModHelperButton Create(out bool addToGroupPanel)
        {
            addToGroupPanel = true;
            var btn = ModHelperButton.Create(new(Name, DialogUi.NextBtnWidth * 1.5f, DialogUi.NextBtnWidth * 1.5f / ModHelperButton.LongBtnRatio), Sprite, new Action(Click));
            btn.AddText(new("Text", InfoPreset.FillParent), OptionText).EnableAutoSizing(35);

            return btn;
        }
    }

    /// <summary>
    /// <see cref="DialogOption"/> using the sprite <see cref="VanillaSprites.RedBtnLong"/> and default click action
    /// </summary>
    public sealed class RedOption : DialogOption
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Sprite => VanillaSprites.RedBtnLong;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override DialogUi dialogUi { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string OptionText { get; }
        
        /// <summary>
        /// Additional thing to do on click.
        /// </summary>
        public Action OnClick { get; set; }

        /// <summary>
        /// Create a RedOption with the following text and dialog queue
        /// </summary>
        public RedOption(string text, Queue<Dialog> dialog)
        {
            SequentialDialog = dialog;
            dialogUi = DialogUi.defaultInstanceForOptions;
            OptionText = text;
        }
        /// <summary>
        /// Create a RedOption with the following text and dialog
        /// </summary>
        public RedOption(string text, params Dialog[] dialog)
        {
            SequentialDialog = new(dialog);
            dialogUi = DialogUi.defaultInstanceForOptions;
            OptionText = text;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Queue<Dialog> SequentialDialog { get; set; }

        public override void Click()
        {
            base.Click();
            OnClick?.Invoke();
        }
    }

    /// <summary>
    /// <see cref="DialogOption"/> using the sprite <see cref="VanillaSprites.GreenBtnLong"/> and default click action
    /// </summary>
    public sealed class GreenOption : DialogOption
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public sealed override string Sprite => VanillaSprites.GreenBtnLong;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override DialogUi dialogUi { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string OptionText { get; }

        /// <summary>
        /// Create a GreenOption with the following text and dialog queue
        /// </summary>
        public GreenOption(string text, Queue<Dialog> dialog)
        {
            SequentialDialog = dialog;
            dialogUi = DialogUi.defaultInstanceForOptions;
            OptionText = text;
        }
        /// <summary>
        /// Create a GreenOption with the following text and dialog
        /// </summary>
        public GreenOption(string text, params Dialog[] dialog)
        {
            SequentialDialog = new(dialog);
            dialogUi = DialogUi.defaultInstanceForOptions;
            OptionText = text;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Queue<Dialog> SequentialDialog { get; set; }
        
        /// <summary>
        /// Additional thing to do on click.
        /// </summary>
        public Action OnClick { get; set; }

        public override void Click()
        {
            base.Click();
            OnClick?.Invoke();
        }
    }

    /// <summary>
    /// UI that handles the dialogue, override to make your own dialogue ui. Requires you to make your own create method.
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class DialogUi : MonoBehaviour
    {
        /// <summary>
        /// The instance set by default for the dialog options that come with this mod.
        /// </summary>
        public static DialogUi defaultInstanceForOptions = instance;


        float timePerCharacter = DialogLib.CharacterSpeed;

        /// <summary>
        /// Instance of the ui.
        /// </summary>
        public static DialogUi instance { get; private set; }

        ///
        public ModHelperPanel mainPanel;

        Dictionary<int, Queue<Dialog>> QueuedDialogPerRound = [];

        Queue<Dialog> currentQueue;
        Queue<Dialog> cachedQueue;
        Dialog? currentDialog;

        ModHelperText text;
        ModHelperImage portrait;
        ModHelperText nameText;
        ModHelperButton nextButton;
        ModHelperButton exitButton;

        /// <summary>
        /// Group where options are placed
        /// </summary>
        public ModHelperScrollPanel OptionsGroup;
        /// <summary>
        /// Width of the next and close buttons
        /// </summary>
        public const float NextBtnWidth = 200;
        /// <summary>
        /// Height of the next and close buttons
        /// </summary>
        public const float NextBtnHeight = 200;

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
            OptionsGroup.ScrollContent.transform.DestroyAllChildren();
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
            OptionsGroup = transform.GetChild(5).GetComponent<ModHelperScrollPanel>();

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
        /// </summary>
        protected bool canGoToNext = false;
        /// <summary>
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
        public void QueueForRound(int round)
        {
            if (QueuedDialogPerRound.ContainsKey(round))
            {
                ShowQueue(QueuedDialogPerRound[round]);
            }
        }

        /// <summary>
        /// Show a queue of dialogs.
        /// </summary>
        /// <param name="queue">Queue in order of dialog shown.</param>
        [HideFromIl2Cpp]
        public virtual void ShowQueue(Queue<Dialog> queue)
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
            currentQueue = queue;
            cachedQueue = new(currentQueue);

            if (currentQueue.Count > 0)
            {
                dialog = currentQueue.Dequeue();

                close = false;
                ShowDialog(dialog);
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
            dialog.OnThis?.Invoke();

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

            if (dialog.Options == null || dialog.Options.Length == 0)
            {
                nextButton.SetActive(true);
                exitButton.SetActive(true);
                OptionsGroup.SetActive(false);
                if (currentQueue == null || currentQueue.Count == 0)
                {
                    nextButton.SetActive(false);
                }
                else
                {
                    exitButton.SetActive(false);
                } 
            }
            else
            {
                nextButton.SetActive(false);
                exitButton.SetActive(false);
                OptionsGroup.SetActive(true);
                OptionsGroup.ScrollContent.transform.DestroyAllChildren();
                foreach (var option in dialog.Options)
                {
                    var btn = option.Create(out bool add);
                    if (add)
                    {
                        OptionsGroup.AddScrollContent(btn);
                    }
                }
            }

            text.Text.SetText(dialog.Text);
            text.Text.maxVisibleCharacters = 0;
            for (int i = 0; i < dialog.Text.Length; i++)
            {
                if (skip)
                {
                    skip = false;
                    text.Text.maxVisibleCharacters = dialog.Text.Length;
                    break;
                }
                text.Text.maxVisibleCharacters++;
                if(text.Text.maxVisibleCharacters % dialog.CharactersPerSound == 0) //if (dialog.Text[i] == '.' || dialog.Text[i] == ' ' && dialog.Text[i - 1] != '.')
                    dialog.Voice.Play();

                yield return new WaitForSeconds(timePerCharacter / dialog.TextSpeed);
            }

            canGoToNext = true;
        }

        /// <summary>
        /// Adds the provided dialogs to the queue
        /// </summary>
        [HideFromIl2Cpp]
        public virtual void AddToDialogQueue(IEnumerable<Dialog> dialogs)
        {
            foreach (var dialog in dialogs)
            {

                QueuedDialogPerRound.TryAdd(dialog.Round, []);
                QueuedDialogPerRound[dialog.Round].Enqueue(dialog);
            }
        }
        /// <summary>
        /// Adds the provided dialogs to the queue
        /// </summary>
        [HideFromIl2Cpp]
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
            var nextButton = mainPanel.AddButton(new("NextBtn", 1000, 0, NextBtnWidth, NextBtnHeight), VanillaSprites.ContinueBtn, new Action(() => { }));
            nextButton.Image.color= Color.white;
            var closeButton = mainPanel.AddButton(new("NextBtn", 1000, 0, NextBtnWidth, NextBtnHeight), VanillaSprites.CloseBtn, new Action(() => { }));
            var optionGroup = mainPanel.AddScrollPanel(new("OptionsGroup", 500, -150, 1000, 300), RectTransform.Axis.Horizontal, null, 25, 25);
            /*optionGroup.RemoveComponent<Mask>();
            optionGroup.AddComponent<RectMask2D>();*/
            instance = mainPanel.AddComponent<DialogUi>();
            defaultInstanceForOptions = instance;
        }
    }
}
