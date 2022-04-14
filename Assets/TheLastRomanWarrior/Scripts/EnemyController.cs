using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator enemyAnim;
    [SerializeField] private GameObject player;
    [SerializeField] private bool isAttack;
    [SerializeField] private float moveSpeed;
    //self
    private GameObject enemyObj;
    private bool isRun;
    
    void Awake()
    {
        enemyAnim = this.GetComponent<Animator>();
        player=GameObject.FindGameObjectWithTag("Player");
        isAttack = false;
        moveSpeed = 20f;
        enemyObj = this.gameObject;
        
    }
    //
    void enemyMove()
    {
        

    }

    void enemyAttack()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemyMove();
    }
}
