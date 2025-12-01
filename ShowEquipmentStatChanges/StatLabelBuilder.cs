using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;

namespace ShowEquipmentStatChanges;

public static class StatLabelBuilder
{
    public static string? BuildChangeText(Item item)
    {
        var stringBuilder = new StringBuilder();
        
        var allowedLocations = GetEnumFlags<EquipSlotTypeFlag>(item.Template.AllowedLocations.Unbox<EquipSlotTypeFlag>());
        
        var equipment = EntityPlayerGameObject.LocalPlayer.Cast<EntityPlayerGameObject>().GetComponent<Equipment>();
        Dictionary<StatType, Stat.Modifier> diffDictionary;

        if (allowedLocations.Count == 1)
        {
            var location = allowedLocations.First();
            var matching = equipment.logic.GetEquippedItem((EquipSlotType)Enum.Parse(typeof(EquipSlotType), location.ToString(), true));

            diffDictionary = GetStatDifferences(matching, item);
        }
        else
        {
            var firstLocation = allowedLocations.First();
            if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            {
                var matching = equipment.logic.GetEquippedItem((EquipSlotType)Enum.Parse(typeof(EquipSlotType), firstLocation.ToString(), true));
                diffDictionary = GetStatDifferences(matching, item);
            }
            else
            {
                var alternateLocation = GetAlternateLocation(firstLocation);
                var matching = equipment.logic.GetEquippedItem((EquipSlotType)Enum.Parse(typeof(EquipSlotType), alternateLocation.ToString(), true));
                diffDictionary = GetStatDifferences(matching, item);
            }
        }
        
        if (!diffDictionary.Any())
        {
            return null;
        }
        
        stringBuilder.Append($"Equipping this item will change your stats:{Environment.NewLine}");
        
        foreach (var (key, statModifier) in diffDictionary.OrderBy(k => k.ToString()))
        {
            stringBuilder.Append(GetStatLabel(key, statModifier));
        }
        
        return stringBuilder.ToString();
    }

    private static Dictionary<StatType, Stat.Modifier> GetStatDifferences(Item? equippedItem, Item hoveredItem)
    {
        var diffDictionary = new Dictionary<StatType, Stat.Modifier>();

        if (equippedItem == null)
        {
            // There is no equipment in the slot, so just process the hovered equipment
            foreach (var stat in hoveredItem.statModifiers)
            {
                var realStat = ReadStatTypeFromMemory(stat);
                diffDictionary.Add(realStat, stat.Item2);
            }
        }
        else
        {
            var equippedStats = equippedItem.statModifiers;

            var hoveredStats = hoveredItem.statModifiers;

            var equippedDictionary = new Dictionary<StatType, Stat.Modifier>();
            var hoveredDictionary = new Dictionary<StatType, Stat.Modifier>();

            foreach (var stat in equippedStats)
            {
                var realStat = ReadStatTypeFromMemory(stat);
                equippedDictionary.Add(realStat, stat.Item2);
            }

            foreach (var stat in hoveredStats)
            {
                var realStat = ReadStatTypeFromMemory(stat);
                hoveredDictionary.Add(realStat, stat.Item2);
            }

            foreach (var (key, statModifier) in equippedDictionary)
            {
                // Stat exists in both equipped item, and the item being hovered
                if (hoveredDictionary.TryGetValue(key, out var hovered))
                {
                    var diff = hovered.Value - statModifier.Value;

                    if (diff != 0)
                    {
                        diffDictionary.Add(key,
                            new Stat.Modifier(diff, statModifier.ModifierType, statModifier.ModifierCategory));
                    }
                }
                // Stat exists in the equipped item, but not in the hovered item
                else
                {
                    diffDictionary.Add(key,
                        new Stat.Modifier(-statModifier.Value, statModifier.ModifierType,
                            statModifier.ModifierCategory));
                }
            }

            foreach (var (key, statModifier) in hoveredDictionary)
            {
                // Stat exists in both, already handled by above code
                if (equippedDictionary.TryGetValue(key, out _))
                {
                    continue;
                }

                // Stat exists in hovered item, but not in equipped item
                diffDictionary.Add(key, statModifier);
            }
        }

        return diffDictionary;
    }
    
    private static StatType ReadStatTypeFromMemory(Il2CppSystem.ValueTuple<StatType, Stat.Modifier> stat)
    {
        // Annoying hack that we have to do because of an issue with il2cppinterop not being able
        // to read ValueTuples correctly, so every key will have the same value
        var rawData = new Il2CppStructArray<byte>(17);
        Marshal.Copy(stat.Pointer, rawData, 0, rawData.Length);

        var statType = rawData.Last();
        var realStat = (StatType)statType;
        
        return realStat;
    }

    private static string GetStatLabel(StatType statType, Stat.Modifier stat)
    {
        return $"{GetLabelColor(stat)}{GetStatText(stat)}</color> {statType.ToString()}{Environment.NewLine}";
    }

    private static string GetStatText(Stat.Modifier stat)
    {
        if (stat.ModifierType == ModifierType.Additive)
        {
            return stat.Value < 0 ? $"-{Math.Abs(stat.Value)}" : $"+{stat.Value}";
        }

        // Not sure there are any multiplicative gear values yet?
        return $"*{stat.Value}";
    }

    private static string GetLabelColor(Stat.Modifier stat)
    {
        if (stat.Value > 0)
        {
            return "<color=\"green\">";
        }

        return "<color=\"red\">";
    }

    private static List<T> GetEnumFlags<T>(T? mask) where T : struct, Enum
    {
        var result = new List<T>();

        // Check if mask is null
        if (!mask.HasValue)
        {
            return result;
        }

        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (mask.Value.HasFlag(value) && !value.Equals(default(T)))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static EquipSlotTypeFlag GetAlternateLocation(EquipSlotTypeFlag flag)
    {
        return flag switch
        {
            EquipSlotTypeFlag.LeftEar => EquipSlotTypeFlag.RightEar,
            EquipSlotTypeFlag.LeftFinger => EquipSlotTypeFlag.RightFinger,
            EquipSlotTypeFlag.PrimaryHand => EquipSlotTypeFlag.SecondaryHand,
            _ => throw new InvalidOperationException()
        };
    }
}