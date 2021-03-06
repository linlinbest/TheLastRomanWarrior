using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
public class Catapult : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject stoneModel;
    [SerializeField] private GameObject launchPoint;
    [SerializeField] private int ammunition;
    [SerializeField] private GameObject AlarmUI;
    private Animator catapultAnim;
    private Rigidbody stoneRigid;
    public GameObject StoneInstance;
    private GameObject player;
    private bool isAttack;
    private bool canAttack;
    private float flySpeed;
    private AudioSource AudioSource;

    [Header("Fire Rate")] 
    public float fireRate;

    private float timer;

    void Awake()
    {
        if (launchPoint == null)
        {
            launchPoint=GameObject.Find("LaunchPoint");
        }
        if (stoneModel == null)
        {
            stoneModel=GameObject.Find("Stone_Model");
        }
        player=GameObject.FindGameObjectWithTag("Player");
        catapultAnim = this.GetComponent<Animator>();
        isAttack = false;
        canAttack = true;
        ammunition = 3;
        flySpeed = 14;
        AudioSource = this.GetComponent<AudioSource>();
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
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (ammunition <= 0)
        {
            catapultAnim.SetBool("noAmmo", true);
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
        CatapultAttack(GenerateRandomNum());
        this.transform.LookAt(player.transform.position);
    }

    void LaunchStone()
    {
        StartCoroutine(StartShoot());
    }
    int GenerateRandomNum()
    {

        if (timer >= fireRate)
        {
            Random random = new Random();
            int randomNum = random.Next(50);
            timer = 0;
            return randomNum;
        }

        return 0;
    }

    //Do the attack animation
    void CatapultAttack(int randomNum)
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
        catapultAnim.SetBool("isAttack", isAttack);
    }
    IEnumerator StartShoot()
    {
        //throw javelin

        AlarmUI.SetActive(true);
        Invoke("CloseAlarm", 2.0f);
        GameObject tempStoneInstance = Instantiate(StoneInstance, launchPoint.transform.position,
            StoneInstance.transform.rotation);
        stoneRigid = tempStoneInstance.GetComponent<Rigidbody>();
        stoneRigid.isKinematic = true;
        AudioSource.Play();
        Vector3 targetPos = player.transform.position;
        float distanceTotarget = Vector3.Distance(tempStoneInstance.transform.position, targetPos);
        bool isReach = false;
        while (!isReach)
        {
          
            tempStoneInstance.transform.LookAt(targetPos);
            float angle = Mathf.Min(1, Vector3.Distance(tempStoneInstance.transform.position, targetPos) / distanceTotarget) * 45;
            tempStoneInstance.transform.rotation = tempStoneInstance.transform.rotation *
                                                   Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
            
            float currentDist = Vector3.Distance(tempStoneInstance.transform.position, targetPos);
            if (currentDist < 0.5f)
            {
                isReach = true;
                stoneRigid.isKinematic = false;

            }
            tempStoneInstance.transform.Translate(Vector3.forward * Mathf.Min( flySpeed * Time.deltaTime, currentDist));
            yield return null;
        }
    }
    
    //Animation Event
    public void ThrowStone()
    {
        stoneModel.SetActive(false);
        ammunition--;
        LaunchStone();
    }
    public void Reload()
    {
        stoneModel.SetActive(true);
    }

    public void CloseAlarm()
    {
        AlarmUI.SetActive(false);
    }
}
