using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Javelin : Throwable
{
    [SerializeField]
    private int damage = 1;

    [Tooltip("The speed threshold to deal damage on enemies")]
    [SerializeField]
    private float validSpeed = 0.5f;

    [Tooltip("Relative rotation to hand")]
    [SerializeField]
    private Vector3 relativeRotation = Vector3.zero;

    private bool isReleased;
    public bool canStick;

    [SerializeField]
    private Rigidbody headRB;

	[SerializeField]
    private Rigidbody shaftRB;

    private Quaternion prevRotation;
    private Vector3 prevVelocity;
	private Vector3 prevHeadPosition;

    [SerializeField] private AudioClip hitShieldSound;

    
    // Start is called before the first frame update
    void Start()
    {
        canStick = false;
        isReleased = true;
        
    }

    void FixedUpdate()
		{
			if (isReleased)
			{
				prevRotation = transform.rotation;
				prevVelocity = headRB.velocity;
				prevHeadPosition = headRB.transform.position;
			}
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
        canStick = false;
    }

    protected override void OnDetachedFromHand(Hand hand)
    {
        base.OnDetachedFromHand(hand);

        shaftRB.isKinematic = false;
        shaftRB.useGravity = true;
        headRB.isKinematic = false;
        headRB.useGravity = true;

        headRB.velocity = shaftRB.velocity;
        headRB.angularVelocity = shaftRB.angularVelocity;

        isReleased = true;
    }

    public override void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
    {
        base.GetReleaseVelocities(hand, out velocity, out angularVelocity);

        // Only keep the forward speed of the javelin
        // Vector3 localVelocity = Quaternion.Inverse(transform.rotation) * velocity;
        // localVelocity = Vector3.Scale(localVelocity, Vector3.forward);
        // velocity = transform.rotation * localVelocity;

        // Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        // localVelocity = Vector3.Scale(localVelocity, Vector3.forward);
        // velocity = transform.TransformDirection(localVelocity);

        angularVelocity = Vector3.zero;

        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!isReleased)
        {
            return;
        }

        GameObject hitObject = collision.collider.gameObject;
        Enemy enemy = hitObject.GetComponent<Enemy>();
        PlayerEntity playerEntity = null;
        if (hitObject.transform.parent != null) playerEntity = hitObject.transform.parent.GetComponentInParent<PlayerEntity>();

        bool hitShield = hitObject.GetComponent<Shield>() != null;
        bool hitPlayer = playerEntity != null;

        float rbSpeed = headRB.velocity.magnitude;
        if (enemy != null && rbSpeed > validSpeed)
        {
            enemy.ReduceHealth(damage);
        }
        else if (!hitShield && playerEntity != null && rbSpeed > 5.0f)
        {
            playerEntity.ReduceHealth(damage);
        }

        if (collision.relativeVelocity.magnitude > validSpeed)
        {
            // audioSource.Play();
        }

        
        

        canStick = ( rbSpeed > validSpeed  && (hitShield));

        if (canStick)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = hitShieldSound;
            audio.Play();

            StickInTarget(collision, false);
        }

            
    }

    private void StickInTarget(Collision collision, bool bSkipRayCast)
    {
        Vector3 prevForward = prevRotation * Vector3.forward;
        Vector3 hitPos = Vector3.positiveInfinity;

        // Only stick in target if the collider is front of the javelin head
		if ( !bSkipRayCast )
		{
			RaycastHit[] hitInfo;
            // 2.0f is the length of the shaft
			hitInfo = Physics.RaycastAll( prevHeadPosition - prevForward * 2.0f, prevForward, 3.0f );
			bool properHit = false;
			for ( int i = 0; i < hitInfo.Length; ++i )
			{
				RaycastHit hit = hitInfo[i];

				if ( hit.collider == collision.collider )
				{
					properHit = true;
                    hitPos = hit.point;
					break;
				}
			}

			if ( !properHit )
			{
				return;
			}
		}
        

        shaftRB.velocity = Vector3.zero;
		shaftRB.angularVelocity = Vector3.zero;
        shaftRB.isKinematic = true;
        shaftRB.useGravity = false;

        headRB.velocity = Vector3.zero;
		headRB.angularVelocity = Vector3.zero;
        headRB.isKinematic = true;
        headRB.useGravity = false;

        GameObject hitTarget = collision.collider.gameObject;
        transform.parent = hitTarget.transform;

        float distHeadToShaftCenter = Vector3.Distance(headRB.transform.position, headRB.transform.parent.position) - 0.1f;

        if (hitPos != Vector3.positiveInfinity)
        {
            transform.position = hitPos - distHeadToShaftCenter * headRB.transform.forward;
        }

    }
}
