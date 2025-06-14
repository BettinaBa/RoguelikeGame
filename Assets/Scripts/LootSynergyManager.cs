using System.Collections.Generic;
using UnityEngine;

public class LootSynergyManager : MonoBehaviour
{
    public static LootSynergyManager Instance { get; private set; }

    private HashSet<string> acquiredTags = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    public void RegisterTags(IEnumerable<string> tags)
    {
        bool changed = false;
        foreach (var tag in tags)
        {
            if (!string.IsNullOrEmpty(tag) && acquiredTags.Add(tag))
                changed = true;
        }
        if (changed)
            Debug.Log("Active synergies: " + string.Join(", ", acquiredTags));
    }

    public float GetTemplateWeight(WeaponTemplate template)
    {
        if (template == null || template.synergyTags == null) return 1f;
        foreach (var t in template.synergyTags)
        {
            if (acquiredTags.Contains(t))
                return 2f; // bias towards templates sharing tags
        }
        return 1f;
    }

    public float GetRarityWeight(ItemRarity rarity, float baseWeight)
    {
        if (acquiredTags.Count == 0)
            return baseWeight;

        // Prefer better rarities when synergies are active
        return rarity == ItemRarity.Common ? baseWeight * 0.5f : baseWeight * 1.5f;
    }
}
