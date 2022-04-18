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

    // Start is called before the first frame update
    void Start()
    {
        health = maxhealth;
        healthBar.setMaxHealth(maxhealth);
    }

    void endGame()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
