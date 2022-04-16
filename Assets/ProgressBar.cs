using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int minimum;
    public int current;
    public Image mask;
    GlobalObject global;


    // Start is called before the first frame update
    void Start()
    {
        global = GameObject.Find("GlobalObject").GetComponent<GlobalObject>();
        maximum = global.maxEnemyNum;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFilled();
    }

    void GetCurrentFilled()
    {        
        GlobalObject global = GameObject.Find("GlobalObject").GetComponent<GlobalObject>();
        float currentFilled = global.enemyDestoried - minimum;
        mask.fillAmount = currentFilled / (maximum - minimum);
    }
}
