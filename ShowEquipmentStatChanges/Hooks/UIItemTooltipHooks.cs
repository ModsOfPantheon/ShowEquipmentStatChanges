using HarmonyLib;
using Il2Cpp;
using Il2CppPantheonPersist;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShowEquipmentStatChanges.Hooks;

[HarmonyPatch(typeof(UIItemTooltip), nameof(UIItemTooltip.Awake))]
public class AwakeHook
{
    private static void Postfix(UIItemTooltip __instance)
    {
        var useText = __instance.UseText;
        var clone = Object.Instantiate(useText, useText.transform.position, useText.transform.rotation, useText.transform.parent);

        clone.transform.name = "Text_EquipmentChanges";
        clone.text = "";
        clone.color = Color.white;

        ModMain.EquipmentText = clone;
    }
}

[HarmonyPatch(typeof(UIItemTooltip), nameof(UIItemTooltip.ShowItem))]
public class ShowItemHook
{
    private static void Prefix(UIItemTooltip __instance, Item item)
    {
        ModMain.OnItemTooltipShow(item);
    }
}

[HarmonyPatch(typeof(UIItemTooltip), nameof(UIItemTooltip.Hide))]
[HarmonyPatch(typeof(UIItemTooltip), nameof(UIItemTooltip.HideTooltip))]
public class HideItemHook
{
    private static void Prefix(UIItemTooltip __instance)
    {
        ModMain.OnHideTooltip();
    }
}