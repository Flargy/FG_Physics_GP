using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPickup : PickupBase
{
    protected override void Activate()
    {
        UI_Manager.Instance.AddCrystal();
        Destroy(gameObject);
    }
}
