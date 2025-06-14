using System.Collections.Generic;
using UnityEngine;

public class LootGenerator : MonoBehaviour
{
    public static LootGenerator Instance { get; private set; }

    [System.Serializable]
    public class RaritySettings
    {
        public ItemRarity rarity;
        [Range(0f,1f)] public float weight = 1f;
        public GameObject prefab;
        public Vector2 damageRange = new Vector2(0.1f,0.2f);
        public Vector2 speedRange = new Vector2(0.05f,0.1f);
    }

    public List<WeaponTemplate> weaponTemplates = new List<WeaponTemplate>();
    public List<RaritySettings> rarityTable = new List<RaritySettings>();

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    public void DropLoot(Vector3 position)
    {
        if (weaponTemplates.Count == 0 || rarityTable.Count == 0) return;

        WeaponTemplate template = PickTemplate();
        RaritySettings settings = PickRarity();
        if (settings.prefab == null)
            settings.prefab = Resources.Load<GameObject>("Loot/LootItem");
        if (settings.prefab == null) return;

        GameObject obj = Instantiate(settings.prefab, position, Quaternion.identity);
        var li = obj.GetComponent<LootItem>();
        if (li != null)
        {
            float dmg = template.baseDamage + Random.Range(settings.damageRange.x, settings.damageRange.y);
            float spd = template.baseFireRate + Random.Range(settings.speedRange.x, settings.speedRange.y);
            li.Setup(dmg, spd, template.specialEffect, template.synergyTags);
        }
    }

    RaritySettings PickRarity()
    {
        float total = 0f;
        foreach (var r in rarityTable)
            total += LootSynergyManager.Instance?.GetRarityWeight(r.rarity, r.weight) ?? r.weight;
        float roll = Random.value * total;
        float accum = 0f;
        foreach (var r in rarityTable)
        {
            float w = LootSynergyManager.Instance?.GetRarityWeight(r.rarity, r.weight) ?? r.weight;
            accum += w;
            if (roll <= accum) return r;
        }
        return rarityTable[rarityTable.Count-1];
    }

    WeaponTemplate PickTemplate()
    {
        float total = 0f;
        foreach (var t in weaponTemplates)
            total += LootSynergyManager.Instance?.GetTemplateWeight(t) ?? 1f;
        float roll = Random.value * total;
        float accum = 0f;
        foreach (var t in weaponTemplates)
        {
            float w = LootSynergyManager.Instance?.GetTemplateWeight(t) ?? 1f;
            accum += w;
            if (roll <= accum) return t;
        }
        return weaponTemplates[weaponTemplates.Count-1];
    }
}

public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
