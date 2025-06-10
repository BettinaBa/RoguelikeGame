using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { XP, Health }
    public PickupType type;
    public int amount = 1;
}
