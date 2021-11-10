using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnvironmentHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RespawnManager.Instance.Respawn();
        }
    }
}
