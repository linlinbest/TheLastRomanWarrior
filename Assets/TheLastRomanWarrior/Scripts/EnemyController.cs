using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Runtime;
using Random = System.Random;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator enemyAnim;
    [SerializeField] private GameObject player;

   
    //self attributes
    private GameObject enemyObj;
    public GameObject javelin;
    private Transform javelinSpawnPoint;
    private Rigidbody enemyRigid;
    
    [Header("Moving Control")]
    [SerializeField] private float moveSpeed;
    public float minDistance;
    private Vector3 movingVec;
    private bool isRun;
    private bool isReachMin;
    private bool isAttack;

    #region Timer

    private float timer;

    #endregion
    
    void Awake()
    {
        enemyAnim = this.GetComponent<Animator>();
        player=GameObject.FindGameObjectWithTag("Player");
        isAttack = false;
        moveSpeed = 10f;
        enemyObj = this.gameObject;
        minDistance = 12f;
        enemyRigid = this.GetComponent<Rigidbody>();
    }
    //
    void calculateEnemyMove()
    {
        //
        //every frame need to update player's position and vector
        if (checkPlayerValidity())
        {
            //player is not null
            Vector3 playerPos = player.transform.position;
            Vector3 targetDir = Vector3.Normalize(playerPos - enemyObj.transform.position);
            //Enemy turns towards player
            Vector3 targetEnemyForwardDir = Vector3.Slerp(enemyObj.transform.forward, targetDir, 0.3f);
            enemyObj.transform.forward = targetEnemyForwardDir;
           //Enemy move
           float distance = Vector3.Magnitude(playerPos - enemyObj.transform.position);
           if (distance > minDistance)
           {
               Vector3 tempVec = moveSpeed * enemyObj.transform.forward;
               movingVec = Vector3.Slerp(movingVec, tempVec, 0.3f);
               isRun = true;
               isReachMin = false;
           }
           else
           {
               movingVec=Vector3.zero;
               isRun = false;
               isReachMin = true;
           }
           
        }
        else
        {
            //player is null
            movingVec=Vector3.zero;
            isRun = false;
        }
        enemyAnim.SetBool("isRun",isRun);
        enemyAnim.SetBool("isReachMin",isReachMin);
    }

    //Random num was generated to control whether enemy can attack at this time
    int generateRandomNum()
    {
       
        if (timer >= 1.5f)
        {
            Random random = new Random();
            int randomNum = random.Next(50);
            timer = 0;
            return randomNum;
        }
        else
        {
            //during interval
            return -1;
        }
    }
    void enemyAttack(int randomNum)
    {
        if (randomNum >= 30)
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }
        enemyAnim.SetBool("isAttack",isAttack);
    }

    bool checkPlayerValidity()
    {
        if (player == null)
        {
            return false;
        }
        else return true;
    }
    void Start()
    {
        
    }
    void Update()
    {
        timer += Time.deltaTime;
        calculateEnemyMove();
        enemyAttack(generateRandomNum());
    }

    void FixedUpdate()
    {
        //actual move
        enemyRigid.position += movingVec * Time.deltaTime;
    }

    //Animation event
    public void throwJavelin()
    {
        Debug.Log("throw");
    }
    // Update is called once per frame

}
