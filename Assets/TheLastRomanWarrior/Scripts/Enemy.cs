using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 1;

    [SerializeField] private Animator enemyAnim;

    private Collider enemyCollider;
    private Rigidbody enemyRigid;
    // Start is called before the first frame update
    void Awake()
    {
        enemyAnim = this.GetComponent<Animator>();
        enemyCollider = this.GetComponent<Collider>();
        enemyRigid = this.GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReduceHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void destroyObject()
    {
        Destroy(this.gameObject);
    }
    public void Die()
    {
        int deathNum = Random.Range(0, 2);
        enemyAnim.SetBool("isDeath",true);
        enemyAnim.SetInteger("DeathNum",deathNum);
        enemyCollider.enabled = false;
        enemyRigid.isKinematic = true;
        Invoke("destroyObject",4);
    }
}
