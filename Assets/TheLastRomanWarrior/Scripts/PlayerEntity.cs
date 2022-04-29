using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField]
    private int maxhealth;

    [SerializeField]
    private int health;

    public HealthBar healthBar;

    public GameObject deathUI;

    public GameObject shieldObj;
    public ProgressBar progressbar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxhealth;
        healthBar.setMaxHealth(maxhealth);
        
    }

    void endGame()
    {
        Reset();
        SceneManager.LoadScene("RealBeginScene");
    }

    public void ReduceHealth(int damage)
    {
        health -= damage;
        healthBar.sethealth(health);
        if (health <= 0)
        {
            if(deathUI==null)
            {
                Debug.Log("not found!");
                // deathUI.SetActive(true);
            }
            deathUI.SetActive(true);
            Invoke("endGame",3.0f);
        }
    }

    public void ClearShield()
    {
        foreach(Transform shieldChild in shieldObj.transform)
        {
            if (shieldChild.tag == "Javelin")
            {
                Destroy(shieldChild.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        health = maxhealth;
        GlobalObject globalObj = GameObject.Find("GlobalObject").GetComponent<GlobalObject>();
        progressbar.maximum = globalObj.maxEnemyNum;
        ClearShield();

        if (deathUI == null) deathUI = GameObject.Find("/LevelCanvas/DeathUI");
        if (deathUI) deathUI.SetActive(false);
    }
}
