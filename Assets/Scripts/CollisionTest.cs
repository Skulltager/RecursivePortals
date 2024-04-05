using System;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody = default;
    [SerializeField] private float movementSpeed = default;
    [SerializeField] private bool showUpdateDebug = default;
    [SerializeField] private bool showFixedUpdateDebug = default;
    [SerializeField] private bool showCollisionDebug = default;
    
    private void Start()
    {
        if (rigidBody == null)
            return;

        rigidBody.velocity = new Vector3(0, 0, 100);
        rigidBody.velocity *= Mathf.Max(0, 1 - rigidBody.drag * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if(showUpdateDebug)
            Debug.Log("Update " + transform.position.z);
    }

    private void FixedUpdate()
    { 
        if (showFixedUpdateDebug)
            Debug.Log("FixedUpdate " + rigidBody.velocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (showCollisionDebug)
            Debug.Log("Collider");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (showCollisionDebug)
            Debug.Log("Trigger " + gameObject.name + " triggered with " + other.gameObject.name);
    }
}
