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

    [Header("Fire Rate")] 
    public float fireRate;
    
    //self attributes
    private GameObject enemyObj;
    
    //Instantiate object
    public GameObject throwJavelinInstance;
    private GameObject tempJavelinInstance;

    //model's holding
    public GameObject javelinSpawnPoint;
    public GameObject javelinModel;
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
    private float time;

    private float gravity = -9.8f;
    private bool isJavelinThrowed;

    private float javelinSpeed=10f;
    private Vector3 currentAngle;

    private Rigidbody javelinRigid;
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

        minDistance = 6f;
        enemyRigid = this.GetComponent<Rigidbody>();
        isJavelinThrowed = false;

        ySpeed = 5f;
        enemyObj = this.gameObject;

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
            enemyObj.transform.forward = Vector3.Slerp(enemyObj.transform.forward, targetDir, 0.3f);
           
            //Enemy move
            float distance = Vector3.Magnitude(playerPos - enemyObj.transform.position);

            if (distance > 30f)
            {
                javelinSpeed = 18f;
            }
            else if (distance > 10f)
            {
                javelinSpeed = 12f;
            }

            if (distance >= minDistance)
            {
                movingVec = moveSpeed * enemyObj.transform.forward;
                isRun = true;
            }
            else
            {
                movingVec = Vector3.zero;
                isRun = false;
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
        if (timer >= fireRate)
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
        if (randomNum >= 30)
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
    }

    void FixedUpdate()
    {
        //actual move
        if (isRun && !enemyMoveLock)
        {
            if (isRun)
            {
                enemyRigid.position += movingVec * Time.fixedDeltaTime;
            }
            enemyRigid.velocity = new Vector3(movingVec.x, enemyRigid.velocity.y, movingVec.z);
            //enemyObj.transform.Translate(enemyObj.transform.forward * moveSpeed * Time.fixedDeltaTime, Space.World); 
        }
    }

    void LateUpdate()
    {
       
    }

    //Animation event

    public void LaunchJavelin()
    {
        StartCoroutine(StartShoot());
    }

    
    //Javelin Shoot

    IEnumerator StartShoot()
    {
        //throw javelin
        GameObject tempJavelinInstance = Instantiate(throwJavelinInstance, javelinSpawnPoint.transform.position,
            javelinSpawnPoint.transform.rotation);
        
        Vector3 targetPos = player.transform.position;
        float distanceTotarget = Vector3.Distance(tempJavelinInstance.transform.position, targetPos);
        bool isReach = false;
        while (!isReach)
        {
            //Stop translating javelin after javelin sticks on the shield
            Javelin javelin = tempJavelinInstance.GetComponent<Javelin>();
            if (javelin && javelin.canStick) break;

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
