using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class dayTimer : MonoBehaviour {

    public static dayTimer instance;
    [SerializeField]
    private int secondsPerDay;
    [SerializeField]
    private float currentTime;
    [SerializeField]
    private GameObject progressUI;
    [SerializeField]
    private Image bigHand, smallHand;
    [SerializeField]
    private GameObject emailIconPrefab;
    [SerializeField]
    private Animator dayFinishedText;
    [SerializeField]
    private Sprite junkMailSprite;
    public  List<completedEmail> todaysEmails = new List<completedEmail>();
    [SerializeField]
    private List<GameObject> emailObjects = null;
    [SerializeField]
    private AudioClip stampSound;
    [SerializeField]
    private Slider timeSlider;

    [System.Serializable]
    public struct completedEmail
    {
        public bool junk;
        public bool correctAnswer;
    }

    [Header("Day Over UI")]
    public Text dayCompletedHeader;
    public Text filedText, performanceText, performanceResult;

    [Header("Day transition UI")]
    public Text DayText;
    public Image background;
    private bool finishedDisplay = false;

    public void NewDay()
    {
        transitioning = false;
        GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);
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

            timeSlider.value = currentTime / secondsPerDay;
        }
        if(GameStateManager.instance.GetState() == GameStates.STATE_DAYOVER && finishedDisplay)
        {
            if(Input.GetButtonDown("Fire1") && !transitioning)
            {
                transitioning = true;
                StartCoroutine(NextDayTransition());
            }
        }
    }

    bool transitioning;

    IEnumerator NextDayTransition() //Fades the screen to black, display the day # text and fades back in
    {
        //Fade to black
        while(background.color.a < 1)
        {
            Color col = background.color;
            col.a += Time.deltaTime;
            background.color = col;
            yield return new WaitForEndOfFrame();
        }

        DayText.text = "Day " + (StatTracker.instance.numOfDaysCompleted +1);

        //Fade in day text
        while (DayText.color.a < 1)
        {
            Color col = DayText.color;
            col.a += Time.deltaTime;
            DayText.color = col;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.5f);

        foreach (WorldObject _wo in FindObjectsOfType<WorldObject>())
        {
            _wo.Reset();
        }
        TileManager.instance.UpgradeSpawners(0.8f, 0.8f, 1.1f);
        Boss.instance.ModifyBoss(1.1f);
        BossFace.instance.Reset();
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

        //Also reset the clock
        smallHand.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
        bigHand.rectTransform.rotation = Quaternion.Euler(0, 0, 0);

        WorkerManager.instance.SetupDefaultPositions();

        //Fade out both
        while (DayText.color.a > 0)
        {
            Color colA = DayText.color;
            colA.a -= Time.deltaTime;
            DayText.color = colA;
            Color colB = background.color;
            colB.a -= Time.deltaTime;
            background.color = colB;
            yield return new WaitForEndOfFrame();
        }

        NewDay();
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
                SoundManager.instance.playSound(0, .95f);
                email.GetComponent<RectTransform>().localPosition = new Vector2(300 + (top * 35), 50);
                top++;
                correct++;
            }
            else
            {
                SoundManager.instance.playSound(0, .25f);
                email.GetComponent<RectTransform>().localPosition = new Vector2(300 + (bot * 35), -47.5f);
                bot++;
            }
            yield return new WaitForSeconds(.35f);
        }

        performanceText.enabled = true;
        yield return new WaitForSeconds(2f);

        int livesToAdd = 0;
        //8 ranks
        int performanceRank;
        if (todaysEmails.Count == 0)
        {
            performanceRank = 0;
            performanceResult.color = Color.red;
        }
        else
        {
            if (correct / todaysEmails.Count < .15f)
            {
                performanceRank = 1;
            }
            else if (correct / todaysEmails.Count < .30f)
            {
                performanceRank = 2;
            }
            else if (correct / todaysEmails.Count < .45f)
            {
                performanceRank = 3;
                livesToAdd = 1;
            }
            else if (correct / todaysEmails.Count < .60f)
            {
                performanceRank = 4;
                livesToAdd = 1;
            }
            else if (correct / todaysEmails.Count < .75f)
            {
                performanceRank = 5;
                livesToAdd = 2;
            }
            else if (correct / todaysEmails.Count < .90f)
            {
                performanceRank = 6;
                livesToAdd = 2;
            }
            else
            {
                performanceRank = 7;
                livesToAdd = 3;
            }


            int min = 8 + StatTracker.instance.numOfDaysCompleted;

            if (todaysEmails.Count < min)
            {
                performanceRank -= min - todaysEmails.Count;
            }

            StatTracker.instance.addDayPerformance(performanceRank);

            switch (performanceRank)
            {
                case 0:
                    performanceResult.text = "USELESS";
                    break;
                case 1:
                    performanceResult.text = "HOW DO YOU STILL HAVE A JOB??";
                    break;
                case 2:
                    performanceResult.text = "SHAME";
                    break;
                case 3:
                    performanceResult.text = "NEEDS IMPROVEMENT";
                    break;
                case 4:
                    performanceResult.text = "MEDIOCRE";
                    break;
                case 5:
                    performanceResult.text = "SUPRISINGLY GOOD";
                    break;
                case 6:
                    performanceResult.text = "RIBBITING!";
                    break;
                case 7:
                    performanceResult.text = "EMPLOYEE OF THE DAY!";
                    break;
            }

            performanceResult.color = Color.Lerp(Color.red, Color.green, correct / todaysEmails.Count);
        }
        SoundManager.instance.playSound(stampSound);
        performanceResult.enabled = true;

        yield return new WaitForSeconds(1.5f);
        if (Player.instance.strikes < 4)
        {
            for (int i = 0; i < livesToAdd; i++)
            {
                Player.instance.strikes++;
                if (Player.instance.strikes > 3)
                {
                    Player.instance.strikes = 3;
                    break;
                }
                SoundManager.instance.playSound(0, 1.2f);
                StatTracker.instance.changeLifeCount(Player.instance.strikes);
                yield return new WaitForSeconds(.75f);
            }
        }

        //Add new lives on and do a sound

        finishedDisplay = true;
    }
}
