using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public ClockManager clockManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "EndScreen" || SceneManager.GetActiveScene().name == "TitleScreen")
        {
            return;
        }
        if (clockManager.puzzleSolved)
        {
            FadeToLevel();
        }
    }
    public void FadeToLevel()
    {
        animator.SetTrigger("FadeIn");
    }
    public void OnFadeComplete()
    {
        SceneManager.LoadScene("EndScreen");
    }
}
