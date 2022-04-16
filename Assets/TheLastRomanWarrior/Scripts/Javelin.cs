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

    [Tooltip("Relative rotation to hand")]
    [SerializeField]
    private Vector3 relativeRotation = Vector3.zero;

    private bool isReleased;
    private bool canStick;

    [SerializeField]
    private Rigidbody headRB;

	[SerializeField]
    private Rigidbody shaftRB;

    
    // Start is called before the first frame update
    void Start()
    {
        canStick = false;
        isReleased = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // protected override void OnHandHoverBegin(Hand hand)
    // {
    //     base.OnHandHoverBegin(hand);
    //     // "Catch" the throwable by holding down the interaction button instead of pressing it.
    //     // Only do this if the throwable is moving faster than the prescribed threshold speed,
    //     // and if it isn't attached to another hand
    //     if ( !attached && catchingSpeedThreshold != -1)
    //     {

    //     }
    // }

    protected override void HandAttachedUpdate(Hand hand)
    {
        base.HandAttachedUpdate(hand);

        if (onHeldUpdate != null)
        {
            attachPosition = hand.transform.position;
            attachRotation = hand.transform.rotation * Quaternion.Euler(relativeRotation);
            transform.position = hand.transform.position;
            transform.rotation = hand.transform.rotation * Quaternion.Euler(relativeRotation);
        }
    }

    protected override void OnAttachedToHand(Hand hand)
    {
        base.OnAttachedToHand(hand);
        
        
        isReleased = false;
    }

    protected override void OnDetachedFromHand(Hand hand)
    {
        base.OnDetachedFromHand(hand);

        isReleased = true;
    }

    public override void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
    {
        base.GetReleaseVelocities(hand, out velocity, out angularVelocity);

        // Only keep the upward speed of the javelin
        Vector3 localVelocity = transform.worldToLocalMatrix * velocity;
        localVelocity = Vector3.Scale(localVelocity, Vector3.up);
        velocity = transform.localToWorldMatrix * localVelocity;

        angularVelocity = Vector3.zero;

        headRB.velocity = velocity;
        headRB.angularVelocity = angularVelocity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!isReleased)
        {
            return;
        }

        GameObject hitObject = collision.collider.gameObject;
        Enemy enemy = hitObject.GetComponent<Enemy>();

        float rbSpeed = headRB.velocity.magnitude;
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
        shaftRB.velocity = Vector3.zero;
		shaftRB.angularVelocity = Vector3.zero;
        shaftRB.isKinematic = true;
        shaftRB.useGravity = false;

        headRB.velocity = Vector3.zero;
		headRB.angularVelocity = Vector3.zero;
        headRB.isKinematic = true;
        headRB.useGravity = false;

        transform.parent = hitTarget.transform;

    }
}
