using System;
using UnityEngine;

public class StickyEnemy : MonoBehaviour
{
    [SerializeField] private float deathTime = 1.0f;
    [SerializeField] private float movementSpeed = 2.5f;
    
    
    public Transform sprite;

    private Animator anim;
    private bool isDead = false;
    private Rigidbody2D body;
    private Transform bodyTransform;
    private BoxCollider2D collider;

    private float currentMovement;
    private float raycastLength;
    private float xScale;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        collider = GetComponent<BoxCollider2D>();
        xScale = transform.localScale.y;
        currentMovement = movementSpeed;
        raycastLength = collider.size.x;
        bodyTransform = body.transform;

    }

    private void Start()
    {
        body.gravityScale = 0;
    }

    private void Update()
    {
        if (!isDead)
        {
            body.velocity +=
                (Vector2) bodyTransform.right * currentMovement; // not using delta time cause this is cheap shit
            body.velocity = Vector2.ClampMagnitude(body.velocity, movementSpeed);

            RaycastHit2D hit;
            hit = Physics2D.Raycast(bodyTransform.position, body.velocity.normalized, raycastLength,
                LayerMask.GetMask("Default"));

            if (hit || !GroundCheck())
            {
                currentMovement *= -1;
                FaceRight(currentMovement > 0);
            }
        }
    }
    
    private bool GroundCheck()
    {
        Vector2 pos = bodyTransform.position;
        if (currentMovement > 0)
        {
            pos += (Vector2)bodyTransform.right;
        }
        else
        {
            pos -= (Vector2)bodyTransform.right;
        }

        RaycastHit2D hit;
        hit = Physics2D.Raycast(pos, -bodyTransform.up, raycastLength, LayerMask.GetMask("Default"));

        if (hit)
        {
            return true;
        }
        

        return false;
    }


    private void Die()
    {
        isDead = true;
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var variable in cols)
        {
            variable.enabled = false;
        }
        sprite.gameObject.GetComponent<SpriteRenderer>().renderingLayerMask = 50;
        body.velocity = Vector2.zero;
        anim.SetBool("IsDead", true);
        Destroy(gameObject, deathTime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            float dot = Vector2.Dot(direction, -transform.up);

            if (dot < -0.5f)
            {
                Die();
            }
            else
            {
                RespawnManager.Instance.Respawn();
            }
        }
    }
    
    private void FaceRight(bool value)
    {
        float direction = value ? xScale: -xScale;
        transform.localScale = new Vector3(direction ,transform.localScale.y, transform.localScale.z);
    }
    
}
