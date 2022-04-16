using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject stoneModel;
    [SerializeField] private GameObject launchPoint;
    public GameObject throwStoneInstance;

    void Awake()
    {
        launchPoint=GameObject.Find("LaunchPoint");
        stoneModel=GameObject.Find("Stone_Model");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
