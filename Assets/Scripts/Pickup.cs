using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Health, XP }
    public PickupType type = PickupType.XP;
    public int amount = 1;
}