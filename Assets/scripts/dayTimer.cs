using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class dayTimer : MonoBehaviour {

    public static dayTimer instance;

    public int secondsPerDay;
    public float currentTime;
    public GameObject progressUI;
    public Image bigHand, smallHand;
    public GameObject emailIconPrefab;
    public Animator dayFinishedText;
    public Sprite junkMailSprite;
    public List<completedEmail> todaysEmails = new List<completedEmail>();
    private List<GameObject> emailObjects = null;

    [System.Serializable]
    public struct completedEmail
    {
        public bool junk;
        public bool correctAnswer;
    }

    [Header("Day Over UI")]
    public Text dayCompletedHeader;
    public Text filedText, performanceText, performanceResult;

    private bool finishedDisplay = false;

    public void NewDay()
    {
        currentTime = 0;
        progressUI.SetActive(false);
        dayFinishedText.Stop();
        progressUI.GetComponent<Image>().enabled = false;
        dayCompletedHeader.text = string.Empty;
        dayCompletedHeader.enabled = false;
        filedText.enabled = false;
        for (int i = 0; i < todaysEmails.Count; i++)
        {
            Destroy(emailObjects[i]);
        }
        todaysEmails.Clear();
        emailObjects.Clear();

        performanceText.enabled = false;
        performanceResult.text = string.Empty;
        finishedDisplay = false;

        WorkerManager.instance.SetupDefaultPositions();
    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY && currentTime < secondsPerDay)
        {
            currentTime += Time.deltaTime;
            if (currentTime > secondsPerDay)
            {
                finishedDisplay = false;
                StartCoroutine(endOfDay());
            }

            //Go from 90 to -150
            smallHand.rectTransform.rotation = Quaternion.Euler(0, 0, 90+(currentTime/secondsPerDay * -240));

            //go from 0 to 2880
            bigHand.rectTransform.rotation = Quaternion.Euler(0, 0, (currentTime / secondsPerDay * -2880));
        }
        if(GameStateManager.instance.GetState() == GameStates.STATE_DAYOVER && finishedDisplay)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                foreach(WorldObject _wo in FindObjectsOfType<WorldObject>())
                {
                    _wo.Reset();
                }
                NewDay();
                GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);
            }
        }
    }

    IEnumerator endOfDay()
    {
        //play some sort of sound
        GameStateManager.instance.ChangeState(GameStates.STATE_DAYOVER);
        progressUI.SetActive(true);
        dayFinishedText.Play("dayfinished");

        yield return new WaitForSeconds(2.5f);
        progressUI.GetComponent<Image>().enabled = true;
        StatTracker.instance.numOfDaysCompleted++;
        dayCompletedHeader.text = "Day "+StatTracker.instance.numOfDaysCompleted + " completed";
        dayCompletedHeader.enabled = true;
        yield return new WaitForSeconds(1.5f);

        filedText.enabled = true;
        yield return new WaitForSeconds(1f);

        int top = 0, bot = 0;
        float correct = 0;

        emailObjects = new List<GameObject>();
        for (int i = 0; i < todaysEmails.Count; i++)
        {
            //For each email, do a prefab.
            GameObject email = Instantiate(emailIconPrefab) as GameObject;
            emailObjects.Add(email);

            if (todaysEmails[i].junk)
                email.GetComponent<Image>().sprite = junkMailSprite;

            email.GetComponent<RectTransform>().SetParent(filedText.transform);
            if (todaysEmails[i].correctAnswer)
            {
                email.GetComponent<RectTransform>().localPosition = new Vector2(300 + (top * 35), 50);
                top++;
                correct++;
            }
            else
            {
                email.GetComponent<RectTransform>().localPosition = new Vector2(300 + (bot * 35), -47.5f);
                bot++;
            }
            yield return new WaitForSeconds(.35f);
        }

        performanceText.enabled = true;
        yield return new WaitForSeconds(2f);
        
        if (correct / todaysEmails.Count < .15f)
            performanceResult.text = "HOW DO YOU STILL HAVE A JOB??";
        else if (correct / todaysEmails.Count < .30f)
            performanceResult.text = "SHAME";
        else if (correct / todaysEmails.Count < .45f)
            performanceResult.text = "NEEDS IMPROVEMENT";
        else if (correct / todaysEmails.Count < .60f)
            performanceResult.text = "MEDIOCRE";
        else if (correct / todaysEmails.Count < .75f)
            performanceResult.text = "SUPRISINGLY GOOD";
        else if (correct / todaysEmails.Count < .90f)
            performanceResult.text = "RIBBITING!";
        else
            performanceResult.text = "EMPLOYEE OF THE DAY!";

        performanceResult.color = Color.Lerp(Color.red, Color.green, correct / todaysEmails.Count);

        performanceResult.enabled = true;

        finishedDisplay = true;
    }
}
