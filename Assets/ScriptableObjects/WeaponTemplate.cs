using UnityEngine;

[CreateAssetMenu(menuName="Loot/WeaponTemplate")]
public class WeaponTemplate : ScriptableObject
{
    public string weaponName;
    public float baseDamage = 1f;
    public float baseFireRate = 1f;
    [Tooltip("Default special effect for this weapon, e.g. Piercing")]
    public string specialEffect;
}
