using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public string[] dialogue;
    public TextMeshProUGUI dialogueText;
    public bool isCamera = false;
    public FirstPersonDrifter player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<FirstPersonDrifter>();
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator TypeSentence(string sentence)
    {
        player.clickDisabled = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.025f);
        }
        player.clickDisabled = false;
    }
    public void Interact()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        StartCoroutine(TypeSentence(dialogue[0]));
    }
}
