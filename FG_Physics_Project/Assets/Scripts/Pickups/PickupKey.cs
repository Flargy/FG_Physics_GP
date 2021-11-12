using UnityEngine;

public class PickupKey : PickupBase
{
    [SerializeField] private GameObject door;
    protected override void Activate()
    {
        if (door)
        {
            door.SetActive(false); // basic version of a key that just removes a door
            Destroy(this);
        }
    }
    
}
