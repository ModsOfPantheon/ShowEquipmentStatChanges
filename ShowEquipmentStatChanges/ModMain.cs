using Il2Cpp;
using Il2CppPantheonPersist;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;

namespace ShowEquipmentStatChanges;

public class ModMain : MelonMod
{
    public static TextMeshProUGUI EquipmentText;
    private static Item _lastHoveredItem;
    private static bool _showTooltip = false;
    
    public override void OnInitializeMelon()
    {
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt) || Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            if (!_showTooltip)
            {
                return;
            }
            
            OnItemTooltipShow(_lastHoveredItem);
        }
    }

    public static void OnItemTooltipShow(Item item)
    {
        _showTooltip = true;
        _lastHoveredItem = item;
        EquipmentText.gameObject.SetActive(false);
        EquipmentText.text = string.Empty;
        
        // Item is equipped, no change
        if (item.SlotType == SlotType.Equipped)
        {
            return;
        }
        
        // Equipment is not weapon or armor, don't display the stat differences
        var itemTypeId = item.Template.ItemTypeId;
        if (itemTypeId != ItemType.Weapon && itemTypeId != ItemType.Armor)
        {
            return;
        }

        var text = StatLabelBuilder.BuildChangeText(item);

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        
        EquipmentText.gameObject.SetActive(true);
        EquipmentText.text = text;
    }

    public const string ModVersion = "1.0.0";

    public static void OnHideTooltip()
    {
        _showTooltip = false;
    }
}