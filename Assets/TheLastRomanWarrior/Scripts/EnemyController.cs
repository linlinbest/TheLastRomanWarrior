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

    private float ySpeed;
 
   

    public System.Action dieAction;

    #region Javelin throw attributes
    //Enemy throw Javelin 
    private float gravity = 1.5f;

    private float angle;
    private float angleSpeed;
    private GameObject tempJavelinInstance;
    private Vector3 javelinMovingVec;
    private Vector3 targetPos;
    //Horizontal speed
    private float javelinSpeedHorizontal;
    private float verticalSpeed;
    private Rigidbody javelinRigid;
    private float javelinSpeedEndVertical = -2f;
    private bool isJavelinThrowed;
    private float flyTime;
    #endregion
    
    #region Timer

    private float timer;
    private float javelinTimer;
    
    

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
       
        minDistance = 6f;
        enemyRigid = this.GetComponent<Rigidbody>();
        javelinSpawnPoint = GameObject.Find("WeaponHandlePoint");
        javelinModel = GameObject.Find("JavelinModel");
        javelinSpeedHorizontal = 10f;
        ySpeed = 5f;
        

        isJavelinThrowed = false;
    }

    //
    void CalculateEnemyMove()
    {
        //
        //every frame need to update player's position and vector
        if (CheckPlayerValidity())
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
    int GenerateRandomNum()
    {

        if (timer >= 2f)
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
    void EnemyAttack(int randomNum)
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

    bool CheckPlayerValidity()
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
        CalculateEnemyMove();
        EnemyAttack(GenerateRandomNum());
        if (isJavelinThrowed)
        {
            ThrowTick();
        }
        
    }

    void FixedUpdate()
    {
        
        //actual move
        if (isRun && !enemyMoveLock)
        {
            enemyRigid.position += movingVec * Time.fixedDeltaTime;
            //enemyObj.transform.Translate(enemyObj.transform.forward * moveSpeed * Time.fixedDeltaTime, Space.World); 
        }
    }

    void LateUpdate()
    {
       
    }

    //Animation event

    public void LaunchJavelin()
    {
        ThrowInit();
        isJavelinThrowed = true;
    }

    public void ThrowInit()
    {
        GameObject spawnJavelin=Instantiate(throwJavelinInstance, javelinSpawnPoint.transform.position,
            throwJavelinInstance.transform.rotation);
        tempJavelinInstance = spawnJavelin;
        javelinRigid = tempJavelinInstance.GetComponent<Rigidbody>();
        javelinRigid.isKinematic = true;
        targetPos = player.transform.position+new Vector3(0,2,0);
        
        float tempDistance = Vector3.Distance(tempJavelinInstance.transform.position, targetPos);
        
        float heightDiff = targetPos.y - tempJavelinInstance.transform.position.y;
       
        Debug.Log("heightDiff"+heightDiff);
        float cosTheta = heightDiff / tempDistance;

        float horizontalDistance = tempDistance * Mathf.Sin(Mathf.Acos(cosTheta));
        Debug.Log("horizontalDistance: "+horizontalDistance);
        
        flyTime = horizontalDistance / javelinSpeedHorizontal;
        Debug.Log("flyTime: "+flyTime);
        
        Vector3 flyingDirHorizontal =
            new Vector3(targetPos.x - tempJavelinInstance.transform.position.x, 0,
                targetPos.z - tempJavelinInstance.transform.position.z).normalized * javelinSpeedHorizontal;

        float fallTime = javelinSpeedEndVertical / gravity;
        Debug.Log("fallTime: "+ fallTime);
        
        float riseTime = flyTime + fallTime;
        Debug.Log("riseTime: "+ riseTime);
        
        verticalSpeed = riseTime * gravity;
        Debug.Log("verticalSpeed: "+verticalSpeed);
        float tempTan = verticalSpeed / javelinSpeedHorizontal;  
        double hu = Math.Atan(tempTan);  
        angle = (float)(180 / Math.PI * hu);  
        transform.eulerAngles = new Vector3(-angle, transform.eulerAngles.y, transform.eulerAngles.z);  
        angleSpeed = angle / riseTime;  
        javelinMovingVec = targetPos - transform.position;
    }
    //Javelin Shoot
    void ThrowTick()
    {
        if (tempJavelinInstance == null)
        {
            Debug.Log("Javelin Null!");
        }
        float currentDist = Vector3.Distance(tempJavelinInstance.transform.position, targetPos);
        if (javelinTimer>=flyTime)
        {
            //finish
            isJavelinThrowed = false;
            javelinTimer = 0;
            javelinRigid.isKinematic = false;
            Debug.Log("fly end!");
        }
        tempJavelinInstance.transform.LookAt(targetPos);
        javelinTimer += Time.deltaTime;
        float test = verticalSpeed - gravity * javelinTimer;
        tempJavelinInstance.transform.Translate(javelinMovingVec.normalized * javelinSpeedHorizontal * Time.deltaTime, Space.World);
        tempJavelinInstance.transform.Translate(Vector3.up * test * Time.deltaTime,Space.World);
        
        float testAngle = -angle + angleSpeed * javelinTimer;  
        transform.eulerAngles = new Vector3(testAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
    
    public void changeToThrow()
    {
        enemyMoveLock = true;
        javelinModel.SetActive(true);
    }

    public void changeToHold()
    {

        enemyMoveLock = false;
        javelinModel.SetActive(true);
    }
    public void throwJavelin()
    {
        enemyMoveLock = true;
        LaunchJavelin();
        javelinModel.SetActive(false);
    }
    
    // Update is called once per frame

}
