using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using DialogLib.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppAssets.Scripts.Unity.Bridge;

namespace DialogLib.Internal
{
    [HarmonyPatch(typeof(InGame), nameof(InGame.Quit))]
    internal static class InGame_EndMatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (DialogUi.instance != null)
            {
                DialogUi.instance.Close();
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
    internal static class InGame_StartMatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (DialogUi.instance == null)
            {
                DialogUi.CreateInstance();
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.StartRound))]
    internal static class UnityToSimulation_StartRound
    {
        [HarmonyPostfix]
        public static void Postfix(UnityToSimulation __instance) 
        {
            if (DialogUi.instance != null)
            {
                DialogUi.instance.QueueForRound(__instance.GetCurrentRound());
            }
        }
    }
}
