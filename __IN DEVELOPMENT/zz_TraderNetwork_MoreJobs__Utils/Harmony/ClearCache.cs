using HarmonyLib;
using UnityEngine;

namespace Harmony
{
    [HarmonyPatch(typeof(XUiC_MainMenu))]
    [HarmonyPatch("OnOpen")]
    public class Menu_ClearCache
    {
        private static void Postfix(XUiC_MainMenu __instance)
        {
            Debug.Log("Clearing Cache...");
        }
    }
}