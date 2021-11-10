
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private static RespawnManager instance = null;
    
    public static RespawnManager Instance
    {
        get { return instance; }
    }

    private RotatingPlayer player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RegisterPlayer(RotatingPlayer p)
    {
        player = p;
    }

    public void Respawn()
    {
        player.Respawn(respawnPoint.position);
    }
}
