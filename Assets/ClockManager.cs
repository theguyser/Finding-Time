using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    public FirstPersonDrifter player;
    private List<int> possible = Enumerable.Range(1, 12).ToList();
    private List<int> listNumbers = new List<int>();
    public int correctNumber;
    public List<int> clock1Numbers = new List<int>();
    public List<int> clock2Numbers = new List<int>();

    public List<GameObject> clock1Objects = new List<GameObject>();
    public List<GameObject> clock2Objects = new List<GameObject>();

    public GameObject clock1;
    public GameObject clock2;
    public GameObject answerClock;

    private int answerClockNumber;

    public AudioClip correctSound;
    public AudioClip wrongSound;
    private AudioSource audioSource;
    public AudioClip doorClick;
    [SerializeField] private AudioClip[] interactSounds = default;
    public GameObject answerClockHand;

    [SerializeField] private int currentNote = 13;

    public GameObject doorHinge;
    public bool puzzleSolved = false;
    private float rotateSpeed = 1.25f;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isRotating = false;
    public int timesSteppedOn = 0;
    public TextMeshProUGUI dialogueText;

    private int recall = 0;
    public GameObject Camera;
    public Clickable door;
    //private TextMeshProUGUI clockUI;

    // Start is called before the first frame update
    void Start()
    {
        //clockUI = GameObject.Find("ClockUI").GetComponent<TextMeshProUGUI>();
        player = GameObject.Find("Player").GetComponent<FirstPersonDrifter>();
        Camera.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        
        //At start of game, generate 7 numbers between 1 and 12 with no duplicates
        for (int i = 0; i < 8; i++)
        {
            int index = Random.Range(1, possible.Count);
            listNumbers.Add(possible[index]);
            possible.RemoveAt(index);
        }
        
        correctNumber = listNumbers[0];
        print("Correct Number: " + correctNumber);
        clock1Numbers.Add(listNumbers[0]);
        clock1Numbers.Add(listNumbers[1]);
        clock1Numbers.Add(listNumbers[2]);
        clock1Numbers.Add(listNumbers[3]);
        
        clock2Numbers.Add(listNumbers[0]);
        clock2Numbers.Add(listNumbers[4]);
        clock2Numbers.Add(listNumbers[5]);
        clock2Numbers.Add(listNumbers[6]);

        answerClockNumber = listNumbers[7];
        if (answerClockNumber == 12)
        {
            answerClockHand.transform.Rotate(0, 0, 0);
        }
        else
        {
            recall = -30 * answerClockNumber;
            answerClockHand.transform.Rotate(0, recall, 0);
        }


        //Adds all clock objects to a list
        for (int i = 0; i < 12; i++)
        {
            clock1Objects.Add(clock1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < 12; i++)
        {
            clock2Objects.Add(clock2.transform.GetChild(i).gameObject);
        }

        //disables clock objects that are not in the list
        for (int i = 0; i < clock1Objects.Count; i++)
        {
            if(clock1Numbers.Contains(i+1))
            {
                clock1Objects[i].SetActive(true);
            }
            else
            {
                clock1Objects[i].SetActive(false);
            }
        }

        for (int i = 0; i < clock2Objects.Count; i++)
        {
            if (clock2Numbers.Contains(i + 1))
            {
                clock2Objects[i].SetActive(true);
            }
            else
            {
                clock2Objects[i].SetActive(false);
            }
        }

        initialRotation = answerClockHand.transform.rotation;
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 120, transform.eulerAngles.z);

        dialogueText.text = "";
        player.lastClickTime = -5f;
        StartCoroutine(TypeSentence("I can leave for the blue hour once the clock in my room is pointing to the right hour"));
    }
    public void ResetClock()
    {
        answerClockHand.transform.rotation = new Quaternion(0,0,0,0);
    }
    public void Randomize()
    {
        //reset answer clock hand
        answerClockHand.transform.Rotate(0, -1 * recall, 0);

        //clears all lists
        clock1Numbers.Clear();
        clock2Numbers.Clear();
        
        //reset many hand Clocks
        for (int i = 0; i < clock1Objects.Count; i++)
        {
            clock1Objects[i].SetActive(true);
        }
        for (int i = 0; i < clock2Objects.Count; i++)
        {
            clock2Objects[i].SetActive(true);
        }
        //add all numbers back to possible list
        for (int i = 0; i < listNumbers.Count; i++)
        {
            possible.Add(listNumbers[i]);
        }
        listNumbers.Clear();
        
        //At start of game, generate 7 numbers between 1 and 12 with no duplicates
        for (int i = 0; i < 8; i++)
        {
            int index = Random.Range(1, possible.Count);
            listNumbers.Add(possible[index]);
            possible.RemoveAt(index);
        }

        correctNumber = listNumbers[0];
        print("Correct Number: " + listNumbers[0]);
        clock1Numbers.Add(listNumbers[0]);
        clock1Numbers.Add(listNumbers[1]);
        clock1Numbers.Add(listNumbers[2]);
        clock1Numbers.Add(listNumbers[3]);

        clock2Numbers.Add(listNumbers[0]);
        clock2Numbers.Add(listNumbers[4]);
        clock2Numbers.Add(listNumbers[5]);
        clock2Numbers.Add(listNumbers[6]);

        answerClockNumber = listNumbers[7];
        ResetClock();
        if (answerClockNumber == 12)
        {
            answerClockHand.transform.Rotate(0, 0, 0);
        }
        else
        {
            recall = -30 * answerClockNumber;
            answerClockHand.transform.Rotate(0, recall, 0);

        }

        //disables clock objects that are not in the list
        for (int i = 0; i < clock1Objects.Count; i++)
        {
            if (clock1Numbers.Contains(i + 1))
            {
                clock1Objects[i].SetActive(true);
            }
            else
            {
                clock1Objects[i].SetActive(false);
            }
        }

        for (int i = 0; i < clock2Objects.Count; i++)
        {
            if (clock2Numbers.Contains(i + 1))
            {
                clock2Objects[i].SetActive(true);
            }
            else
            {
                clock2Objects[i].SetActive(false);
            }
        }
    }
    public void CheckAnswer()
    {
        if (answerClockNumber == correctNumber)
        {
            print("Correct Answer");
            audioSource.PlayOneShot(correctSound);
            puzzleSolved = true;
        }
        else
        {
            print("Incorrect Answer");
            audioSource.PlayOneShot(wrongSound);
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("I think that changed the time I am allowed to leave"));
            Randomize();
        }
    }
    public void AddTime()
    {
        answerClockNumber++;
        if(answerClockNumber > 12)
        {
            answerClockNumber = 1;
        }
        if(answerClockNumber == correctNumber)
        {
            audioSource.PlayOneShot(doorClick);
            
        }
    }
    public void SubtractTime()
    {
        answerClockNumber--;
        if (answerClockNumber < 1)
        {
            answerClockNumber = 12;
        }
        if (answerClockNumber == correctNumber)
        {
            audioSource.PlayOneShot(doorClick);
            
        }
    }
    public void PlayInteractAudio(string clock)
    {
        if (clock == "clockwise")
        {
            currentNote++;
            {
                if (currentNote > 28)
                {
                    currentNote = 28;
                }
            }
        }
        else if (clock == "counterclockwise")
        {
            currentNote--;
            {
                if (currentNote < 0)
                {
                    currentNote = 0;
                }
            }
        }
        audioSource.clip = interactSounds[currentNote];
        audioSource.PlayOneShot(audioSource.clip);

    }
    // Update is called once per frame
    void Update()
    {
        if (puzzleSolved && !isRotating)
        {
            isRotating = true;
        }

        if (isRotating)
        {
            doorHinge.transform.rotation = Quaternion.Lerp(doorHinge.transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(doorHinge.transform.rotation, targetRotation) < 0.1f)
            {
                doorHinge.transform.rotation = targetRotation; // Snap to the target rotation
                isRotating = false; // Stop rotating
            }
        }

        if(answerClockNumber == correctNumber)
        {
            if (Camera.activeSelf == false)
            {
                Camera.SetActive(true);
            }
            door.canLeave = true;
        }
        else
        {
            if(Camera.activeSelf == true)
            {
                Camera.SetActive(false);
            }
            door.canLeave = false;
        }
        //clockUI.text = "Answer Clock Time: " + answerClockNumber;

    }
    public void TriggerDialogue()
    {
        if (timesSteppedOn == 1)
        {
            StopAllCoroutines();
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("I should check how the clock in my room has changed after hearing this sound"));
        }
        else if (timesSteppedOn == 3)
        {
            StopAllCoroutines();
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("Stepping here seems to either raise or lower the time on my clock"));
        }
        else if (timesSteppedOn == 7)
        {
            audioSource.PlayOneShot(wrongSound);
            StopAllCoroutines();
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("I think that changed the time I need to leave"));
        }
        else if (timesSteppedOn == 10)
        {
            StopAllCoroutines();
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("When the pitch increases, the time on my clock increases by 1. When the pitch decreases, it decreases by 1"));
        }
        else if (timesSteppedOn == 12)
        {
            StopAllCoroutines();
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("The hour hand the living room clock and toilet clock have in common is the time my clock needs to be"));
        }
        else if (timesSteppedOn == 14)
        {
            audioSource.PlayOneShot(wrongSound);
            StopAllCoroutines();
            dialogueText.text = "";
            player.lastClickTime = -5f;
            StartCoroutine(TypeSentence("I need to find the right time for my clock in less than 7 sounds or else it will reset the clocks"));
            timesSteppedOn = 0;
        }
    }
    public IEnumerator TypeSentence(string sentence)
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
}
