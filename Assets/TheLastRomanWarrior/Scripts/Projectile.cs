using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.collider.gameObject;
        PlayerEntity playerEntity = null;
        if (hitObject.transform.parent != null && hitObject.transform.parent.parent != null) playerEntity = hitObject.transform.parent.parent.GetComponentInParent<PlayerEntity>();
        // if (hitObject.transform.parent != null) playerEntity = hitObject.transform.parent.GetComponentInParent<PlayerEntity>();

        if (playerEntity != null)
        {
            playerEntity.ReduceHealth(damage);
        }
    }
}
