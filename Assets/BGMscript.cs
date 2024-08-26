using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMscript : MonoBehaviour
{
    private static BGMscript instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // Ensures there is only one instance of the BGMManager
        }
    }
}
