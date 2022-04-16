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

    // Start is called before the first frame update
    void Start()
    {
        health = maxhealth;
        healthBar.setMaxHealth(maxhealth);
    }

    void endGame()
    {
        SceneManager.LoadScene("BeginScene");
    }
    // Update is called once per frame
    void Update()
    {
        healthBar.sethealth(health);
        if (health <= 0)
        {
            GameObject deathimage = GameObject.Find("/Canvas/DeathUI");
            deathimage.SetActive(true);
            Invoke("endGame", 5.0f);
        }
    }
}
