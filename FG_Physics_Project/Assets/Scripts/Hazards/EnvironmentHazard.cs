using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnvironmentHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 playerVelocity = other.gameObject.GetComponent<Rigidbody2D>().velocity;

            float dotValue = Vector2.Dot(transform.up, playerVelocity);

            if (dotValue < 0.7f)
            {
                Debug.Log("Respawn");
                RespawnManager.Instance.Respawn();
            }
            
        }
    }
}
