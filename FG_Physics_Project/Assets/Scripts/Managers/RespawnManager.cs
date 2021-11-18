
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private float deathDelay = 0.2f;
    private bool canDie = true;

    private static RespawnManager instance = null;
    
    public static RespawnManager Instance
    {
        get { return instance; }
    }

    private PlayerStateMachine player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RegisterPlayer(PlayerStateMachine p)
    {
        player = p;
    }

    public void Respawn()
    {
        if (!canDie)
        {
            return;
        }

        StartCoroutine(DeathTimer());
        player.Respawn(respawnPoint.position);
        if(UI_Manager.Instance)
            UI_Manager.Instance.RespawnDeathScreen();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Respawn();
        }
    }

    private IEnumerator DeathTimer()
    {
        canDie = false;
        float timer = 0;
        while (timer <= deathDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        canDie = true;
    }
}
