using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class PickupBase : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Activate();
        }
    }

    protected virtual void Activate(){}
}
