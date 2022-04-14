using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIObject : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject selectMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openLevelSelection()
    {
        Debug.Log("level selection");
    }

    public void endGame()
    {
        Application.Quit();
    }

    public void pressStart()
    {
        startMenu.SetActive(false);
        selectMenu.SetActive(true);
    }

    public void pressReturn()
    {
        selectMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void openLevelOne()
    {
        SceneManager.LoadScene("level1");
    }
}
