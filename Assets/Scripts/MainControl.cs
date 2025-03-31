using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainControl : MonoBehaviour
{
    public GameObject objRank;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Robby");
    }


    public void Rank()
    {
        objRank.SetActive(true);
    }


    public void Exit()
    {
        Application.Quit();
    }

    public void SetClose()
    {
        objRank.SetActive(false);
    }
}
