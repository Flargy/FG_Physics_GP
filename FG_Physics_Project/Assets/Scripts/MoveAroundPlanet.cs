using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class MoveAroundPlanet : MonoBehaviour
{

    [SerializeField] private Transform planet;
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float gravityStrength = 10.0f;
    [SerializeField] private float jumpStrength = 100.0f;
    [SerializeField] private GameObject bombPrefab;

    private Vector3 gravityVector;
    private Rigidbody2D body;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        RotateTowardsPlanet();
        GravitateTowardsPlanet();

        HandleInputAndMovement();

    }

    private void RotateTowardsPlanet()
    {
        if (!planet)
        {
            return;
        }
        gravityVector = (planet.position - transform.position).normalized;
        transform.up = -gravityVector;
    }

    private void GravitateTowardsPlanet()
    {
        body.AddForce(gravityVector * gravityStrength);
    }

    private void HandleInputAndMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            body.AddForce(-transform.up + transform.right * (movementSpeed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            body.AddForce(-transform.up + -transform.right * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            body.AddForce(transform.up * jumpStrength, ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Nuke();
        }
    }

    public void GetSnatchedByPlanet(Transform newPlanet)
    {
        planet = newPlanet;
    }

    private void Nuke()
    {
        GameObject currentBomb = Instantiate(bombPrefab);
    }
}
