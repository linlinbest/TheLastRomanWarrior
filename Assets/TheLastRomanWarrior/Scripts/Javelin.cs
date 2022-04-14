using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Javelin : Throwable
{
    [SerializeField]
    private int damage = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
    {
        base.GetReleaseVelocities(hand, out velocity, out angularVelocity);
        angularVelocity = Vector3.zero;
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject gameObject = collision.collider.gameObject;
        Enemy enemy = gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ReduceHealth(damage);
        }
        if (collision.relativeVelocity.magnitude > 2)
        {
            // audioSource.Play();
        }
            
    }
}
