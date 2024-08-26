using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterClockwiseTrigger : MonoBehaviour
{
    public GameObject answerClockHand;
    public ClockManager clockManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Counter Clockwise Triggered");
            clockManager.timesSteppedOn++;
            if (clockManager.timesSteppedOn == 7 || clockManager.timesSteppedOn == 14)
            {
                clockManager.Randomize();
                clockManager.TriggerDialogue();
            }
            else
            {
                answerClockHand.transform.Rotate(0, 30, 0);
                clockManager.SubtractTime();
                clockManager.PlayInteractAudio("counterclockwise");
                clockManager.TriggerDialogue();
            }
        }
    }
}
