using UnityEngine;

public class Ammo : MonoBehaviour
{
    [Header("Check class: AmmoType")]
    [SerializeField] private AmmoSlot[] ammoSlots;
    
    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType ammoType;
        public int ammoAmount;
    }
    public int GetTotalAmmo(AmmoType ammoType)
    {
        return GetAmmoSlot(ammoType).ammoAmount;
    }

    public void IncreaseTotalAmmo(AmmoType ammoType, int ammoAmount)
    {
        GetAmmoSlot(ammoType).ammoAmount += ammoAmount;
    }
    
    public void ReduceTotalAmmo(AmmoType ammoType)
    {
        if (GetAmmoSlot(ammoType).ammoAmount == 0) return; 
        GetAmmoSlot(ammoType).ammoAmount--;
    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.ammoType == ammoType)
            {
                return slot;
            }
        }
        return null;
    }
}
