using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = System.Random;


public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator enemyAnim;
    [SerializeField] private GameObject player;


    //self attributes
    private GameObject enemyObj;

    //Instantiate object
    public GameObject throwJavelinInstance;

    //model's holding
    [SerializeField] private GameObject javelinSpawnPoint;
    [SerializeField] private GameObject javelinModel;
    private Rigidbody enemyRigid;

    [Header("Moving Control")] [SerializeField]
    private float moveSpeed;
    public float minDistance;

    private float maxShootDistance;
    //Only inside the shoot range

    public bool enemyMoveLock;

    private bool canAttack;
    private Vector3 movingVec;
    private bool isRun;
    private bool isReachMin;
    private bool isAttack;

    private float javelinSpeed;

    #region Timer

    private float timer;

    #endregion

    void Awake()
    {
        enemyAnim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        canAttack = false;
        isAttack = false;
        moveSpeed = 2f;
        enemyMoveLock = false;
        maxShootDistance = 30f;
        enemyObj = this.gameObject;
        minDistance = 12f;
        enemyRigid = this.GetComponent<Rigidbody>();
        javelinSpawnPoint = GameObject.Find("WeaponHandlePoint");
        javelinModel = GameObject.Find("JavelinModel");
        javelinSpeed = 20f;
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
            if (distance < maxShootDistance)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }

            if (distance >= minDistance)
            {
                Vector3 tempVec = moveSpeed * enemyObj.transform.forward;
                movingVec = Vector3.Slerp(movingVec, tempVec, 0.3f);
                isRun = true;
                isReachMin = false;
            }
            else
            {
                movingVec = Vector3.zero;
                isRun = false;
                isReachMin = true;
            }

        }
        else
        {
            //player is null
            movingVec = Vector3.zero;
            isRun = false;
        }

        enemyAnim.SetBool("isRun", isRun);
        enemyAnim.SetBool("isReachMin", isReachMin);
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
            return 0;
        }
    }

    //Do the attack animation
    void enemyAttack(int randomNum)
    {
        if (randomNum >= 30 && canAttack)
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }

        // When attack, lock move
        enemyAnim.SetBool("isAttack", isAttack);
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
        if (isRun && !enemyMoveLock)
        {
            enemyRigid.position += movingVec * Time.deltaTime;
        }
    }
    

    //Animation event

    public void launchJavelin()
    {
        StartCoroutine(startShoot());
    }

    IEnumerator startShoot()
    {
        //throw javelin
        Quaternion rotation=Quaternion.Euler(-90,0,0);
        GameObject tempJavelinInstance = Instantiate(throwJavelinInstance, javelinSpawnPoint.transform.position,
            rotation);
        
        Vector3 targetPos = player.transform.position;
        float distanceTotarget = Vector3.Distance(tempJavelinInstance.transform.position, targetPos);
        bool isReach = false;
        while (!isReach)
        {
           
            tempJavelinInstance.transform.LookAt(targetPos);
            float angle = Mathf.Min(1, Vector3.Distance(tempJavelinInstance.transform.position, targetPos) / distanceTotarget) * 45;
            tempJavelinInstance.transform.rotation = tempJavelinInstance.transform.rotation *
                                                     Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
            
            float currentDist = Vector3.Distance(tempJavelinInstance.transform.position, targetPos);
            if (currentDist < 0.5f) isReach = true;
            tempJavelinInstance.transform.Translate(Vector3.forward * Mathf.Min( javelinSpeed * Time.deltaTime, currentDist));
            yield return null;
        }
    }
    
    public void changeToThrow()
    {
        enemyMoveLock = true;
        javelinModel.SetActive(true);
        javelinSpawnPoint.transform.rotation=Quaternion.Euler(0,0,0);
    }

    public void changeToHold()
    {

        enemyMoveLock = false;
        javelinModel.SetActive(true);
        javelinSpawnPoint.transform.rotation=Quaternion.Euler(-90,0,0);
    }
    public void throwJavelin()
    {
        enemyMoveLock = true;
        javelinSpawnPoint.transform.rotation=Quaternion.Euler(90,0,0);
        launchJavelin();
        javelinModel.SetActive(false);
    }
    
    // Update is called once per frame

}
