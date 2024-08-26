using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((SceneManager.GetActiveScene().name == "EndScreen" || SceneManager.GetActiveScene().name == "TitleScreen") && Input.GetButton("Jump"))
        {
            BeginGame();
        }
    }
    public void BeginGame()
    {
        SceneManager.LoadScene("Game");
    }
}
