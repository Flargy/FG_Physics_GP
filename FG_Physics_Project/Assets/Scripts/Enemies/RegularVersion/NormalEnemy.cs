
using System;
using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private Transform sprite;
    [SerializeField] private float maximumFallSpeed = 6.0f;
    [SerializeField] private float fallingGravity = 5.0f;
    [SerializeField, Min(0.1f)] private float fullSpinDuration = 0.5f;
    
    
    private bool isDead = false;
    private Rigidbody2D body;
    private Transform bodyTransform;
    private BoxCollider2D collider;
    private float currentMovement;
    private float raycastLength;
    private Animator anim;
    private float xScale;

    private bool isWalking;
    
    

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bodyTransform = body.transform;
        collider = body.gameObject.GetComponent<BoxCollider2D>();
        currentMovement = movementSpeed;
        raycastLength = collider.size.x;
        xScale = transform.localScale.y;

    }

    private void Start()
    {
        if(WorldRotation.Instance)
            WorldRotation.Instance.RegisterEnemy(this);
    }

    private void Update()
    {
        if (isWalking)
        {
            bodyTransform.up = -Physics2D.gravity.normalized;
            sprite.localRotation = Quaternion.identity;
            body.gravityScale = 0;
            body.velocity += (Vector2) bodyTransform.right * currentMovement; // not using delta time cause this is cheap shit
            body.velocity = Vector2.ClampMagnitude(body.velocity, movementSpeed);

            RaycastHit2D hit;
            hit = Physics2D.Raycast(bodyTransform.position, body.velocity.normalized, raycastLength, LayerMask.GetMask("Default"));

            if (hit)
            {
                currentMovement *= -1;
                FaceRight(currentMovement > 0);
            }

            if (!GroundCheck())
            {
                body.velocity = Vector2.zero;
                isWalking = false;
            }
        }
        else
        {
            body.gravityScale = fallingGravity;
            SpinAround();
            body.velocity = Vector2.ClampMagnitude(body.velocity, maximumFallSpeed);
            if (isDead)
            {
                return;
            }
            if (FallingGroundCheck())
            {
                isWalking = true;
            }
        }
    }
    
    
    private bool GroundCheck()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(bodyTransform.position, -bodyTransform.up, raycastLength, LayerMask.GetMask("Default"));

        if (hit)
        {
            return true;
        }
        
        hit = Physics2D.BoxCast(bodyTransform.position,
            collider.bounds.extents * 2,
            0, -bodyTransform.up,
            0.05f, LayerMask.GetMask("Default"));
        if (hit)
        {
            return true;
        }

        return false;
    }
    
    public void Rotate()
    {
        body.velocity = Vector2.zero;
        isWalking = false;
    }
    
    public void FaceRight(bool value)
    {
        float direction = value ? xScale: -xScale;
        transform.localScale = new Vector3(direction ,transform.localScale.y, transform.localScale.z);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 direction = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            float dot = Vector2.Dot(direction, -other.transform.up);
            
            Debug.Log(direction);

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
    
    private void Die()
    {
        isDead = true;
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var variable in cols)
        {
            variable.enabled = false;
        }
        if(WorldRotation.Instance)
            WorldRotation.Instance.UnregisterEnemy(this);
        Destroy(this, 2.5f);
        sprite.gameObject.GetComponent<SpriteRenderer>().renderingLayerMask = 50;
        body.velocity = Vector2.zero;
        isWalking = false;
    }
    
    public bool FallingGroundCheck()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(bodyTransform.position, Physics2D.gravity.normalized, raycastLength * 0.55f, LayerMask.GetMask("Default"));

        if (hit)
        {
            return true;
        }
        
        hit = Physics2D.BoxCast(bodyTransform.position,
            collider.bounds.extents * 2,
            0, Physics2D.gravity.normalized,
            0.1f, LayerMask.GetMask("Default"));
        if (hit)
        {
            return true;
        }

        return false;
    }
    
    public void SpinAround()
    {
        sprite.localRotation = Quaternion.Euler(0, 0, sprite.localRotation.eulerAngles.z + 360 * Time.deltaTime / fullSpinDuration);
    }
}
