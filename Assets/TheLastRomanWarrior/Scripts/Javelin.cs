using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Javelin : Throwable
{
    [SerializeField]
    private int damage = 1;

    [Tooltip("The speed threshold to deal damage")]
    [SerializeField]
    private float validSpeed = 0.5f;

    private bool canStick = false;
    
    // Start is called before the first frame update
    void Start()
    {
        canStick = false;
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
        GameObject hitObject = collision.collider.gameObject;
        Enemy enemy = hitObject.GetComponent<Enemy>();

        float rbSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        if (enemy != null && rbSpeed > validSpeed)
        {
            enemy.ReduceHealth(damage);
        }
        if (collision.relativeVelocity.magnitude > validSpeed)
        {
            // audioSource.Play();
        }

        
        bool hitShield = hitObject.GetComponent<Shield>() != null;
        bool hitPlayer = hitObject.GetComponent<PlayerEntity>() != null;

        canStick = ( rbSpeed > validSpeed  && (hitShield || hitPlayer));

        if (canStick)
        {
            StickInTarget(hitObject, true);
        }

            
    }

    private void StickInTarget(GameObject hitTarget, bool bSkipRayCast)
    {
        // Only stick in target if the collider is front of the arrow head
		// if ( !bSkipRayCast )
		// {
		// 	RaycastHit[] hitInfo;
		// 	hitInfo = Physics.RaycastAll( prevHeadPosition - prevVelocity * Time.deltaTime, prevForward, prevVelocity.magnitude * Time.deltaTime * 2.0f );
		// 	bool properHit = false;
		// 	for ( int i = 0; i < hitInfo.Length; ++i )
		// 	{
		// 		RaycastHit hit = hitInfo[i];

		// 		if ( hit.collider == collision.collider )
		// 		{
		// 			properHit = true;
		// 			break;
		// 		}
		// 	}

		// 	if ( !properHit )
		// 	{
		// 		return;
		// 	}
		// }


        // this.rigidbody.detectCollisions = false;
        this.rigidbody.velocity = Vector3.zero;
		this.rigidbody.angularVelocity = Vector3.zero;
        this.rigidbody.isKinematic = true;
        this.rigidbody.useGravity = false;
        transform.parent = hitTarget.transform;

    }
}
