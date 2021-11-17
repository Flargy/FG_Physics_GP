using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPickup : PickupBase
{
    protected override void Activate()
    {
        if(UI_Manager.Instance)
            UI_Manager.Instance.AddCrystal();
        Destroy(gameObject);
    }
}
