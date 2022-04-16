using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.SceneManagement;


public class GlobalObject : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator[] humanEnemyGenerator;

    [SerializeField]
    private int[] enemyWavePosNum;  //how many enemy genrator position used for each wave

    [SerializeField]
    private float[] waveTime;     //time for each wave

    [SerializeField]
    private int numberEnemyEachPos;  //number of enemy spawned for each position

    public int maxEnemyNum;
    public int enemyGenerated;
    public int enemyDestoried;


    int waveIdx; //index for waves
    int totalWaveNum; //number of total waves
    int totalHumanEnemyPosNum;
    float level_time;
    Vector3[] enemyPos;
    float xOffset;
    float zOffset;
    

    

    // Start is called before the first frame update
    void Start()
    {
        waveIdx = 0;
        totalWaveNum = waveTime.Length;
        totalHumanEnemyPosNum = humanEnemyGenerator.Length;
        level_time = 0.0f;
        xOffset = 0.0f;
        zOffset = 0.0f;
        enemyGenerated =0;
        enemyDestoried = 0;


    }

    // Update is called once per frame
    void Update()
    {
        level_time += Time.deltaTime;
        if (waveIdx < totalWaveNum && level_time >=waveTime[waveIdx]  && enemyGenerated < maxEnemyNum)
        {
            for (int i=0; i<totalHumanEnemyPosNum; i++)
            {
                Vector3 generatorPos = humanEnemyGenerator[i].transform.position;
                GameObject humanprefab = humanEnemyGenerator[i].enemyPrefab;
                for(int j=0; j< numberEnemyEachPos; j++)
                {
                    xOffset = Random.Range(-5, 5);
                    zOffset = Random.Range(-5, 5);
                    Vector3 spawnPos = new Vector3(generatorPos.x + xOffset, generatorPos.y, generatorPos.z + zOffset);
                    Enemy newEnemy = Instantiate(humanprefab, spawnPos, gameObject.transform.rotation).GetComponent<Enemy>();
                    newEnemy.dieAction += invaderDestoryed;
                    enemyGenerated += 1;
                    if (enemyGenerated == maxEnemyNum) break;
                }
                //Array.Clear(enemyPos, 0, enemyPos.Length);
                if (enemyGenerated == maxEnemyNum) break;

            }
            waveIdx += 1;
            Debug.Log("wave: "+ waveIdx);

        }
    }

    void endGame()
    {
        SceneManager.LoadScene("BeginScene");
    }

    void invaderDestoryed()
    {
        enemyDestoried++;
        if (enemyDestoried== maxEnemyNum)
        {
            GameObject winImage = GameObject.Find("/Canvas/WinUI");
            winImage.SetActive(true);
            Invoke("endGame", 5f);

        }

    }
}
