using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class StatTracker : MonoBehaviour
{
    public static StatTracker instance;

    public int[] junkEmailsCorrect, safeEmailsCorrect, safeEmailsWrong, junkEmailsWrong, numOfDaysCompleted, messyDesks;
    public int[] totalDeaths, bossDeaths, bossAngered, score,scoreToAdd, bossAngeredDay;
    public float[] totalProfessionalism;
    public Text ScoreText;
    List<int> dayPerformances = new List<int>();
    List<int> dayPerformancesP2 = new List<int>();

    [Header("Lives UI")]
    public Animator[] lifeAnimators;

    [Header("Game Over UI")]
    public GameObject GameOverUI;
    public Text causeOfDeath, daysCompleted, daysCompletedValue, emailsFiled, emailsFiledValue, emailsHandled,
        emailsHandledValue, professionalism, professionalismValue, finalScore, finalScoreValue;
    public Image gameoverScreen;
    [Header("")]
    public float bossAngerLevel;

    void Awake()
    {
        junkEmailsCorrect = new int[2] { 0, 0 };
        junkEmailsCorrect = new int[2] { 0, 0 };
        safeEmailsCorrect = new int[2] { 0, 0 };
        safeEmailsWrong = new int[2] { 0, 0 };
        junkEmailsWrong = new int[2] { 0, 0 };
        numOfDaysCompleted = new int[2] { 0, 0 };
        messyDesks = new int[2] { 0, 0 };
        totalDeaths = new int[2] { 0, 0 };
        bossDeaths = new int[2] { 0, 0 };
        bossAngeredDay = new int[2] { 0, 0 };
        bossAngered = new int[2] { 0, 0 };
        scoreToAdd = new int[2] { 0, 0 };
        totalProfessionalism = new float[2] { 0, 0 };
        score = new int[2] { 0, 0 };

        instance = this;
    }
    public void changeLifeCount(int l, bool gainLoss)
    {
        if (gainLoss)
            lifeAnimators[l].Play("life_gain");
        else
            lifeAnimators[l].Play("life_loss");
    }

    public void setLifeCount(int l) //Remember to call this when switching players, make sure the HuD is accurate
    {
        for (int i = 0; i < l; i++)
        {
            lifeAnimators[i].Play("life_idle");
        }

        for (int i = l; i < lifeAnimators.Length; i++)
        {
            lifeAnimators[i].Play("life_idle_dead");
        }
    }

    public float getAveragePerformance()
    {
        float total = 0;
        if (multiplayerManager.instance.currentActivePlayer == 0)
        {
            for (int i = 0; i < dayPerformances.Count; i++)
            {
                total += dayPerformances[i];
            }

            return total / dayPerformances.Count;
        }
        else
        {
            for (int i = 0; i < dayPerformancesP2.Count; i++)
            {
                total += dayPerformancesP2[i];
            }

            return total / dayPerformancesP2.Count;
        }
    }

    public void addDayPerformance(int performance)
    {
        if (multiplayerManager.instance.currentActivePlayer == 0)
            dayPerformances.Add(performance);
        else
            dayPerformancesP2.Add(performance);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (scoreToAdd[multiplayerManager.instance.currentActivePlayer] > 0)
            {
                scoreToAdd[multiplayerManager.instance.currentActivePlayer]--;
                score[multiplayerManager.instance.currentActivePlayer]++;
                ScoreText.text = "SCORE:" + score[multiplayerManager.instance.currentActivePlayer];
            }
            else if (scoreToAdd[multiplayerManager.instance.currentActivePlayer] < 0)
            {
                scoreToAdd[multiplayerManager.instance.currentActivePlayer]++;
                score[multiplayerManager.instance.currentActivePlayer]--;
                ScoreText.text = "SCORE:" + score[multiplayerManager.instance.currentActivePlayer];
            }
        }
    }

    public IEnumerator GameOverUIReveal()
    {
        multiplayerManager.instance.win[multiplayerManager.instance.currentActivePlayer] = false;

        //We come here when a player dies and has no lives left
        SoundManager.instance.officeAmbience.DOFade(0, 3);
        SoundManager.instance.music.Stop();

        float total = junkEmailsCorrect[multiplayerManager.instance.currentActivePlayer] + junkEmailsWrong[multiplayerManager.instance.currentActivePlayer] + safeEmailsCorrect[multiplayerManager.instance.currentActivePlayer] + safeEmailsWrong[multiplayerManager.instance.currentActivePlayer];
        float correct = junkEmailsCorrect[multiplayerManager.instance.currentActivePlayer] + safeEmailsCorrect[multiplayerManager.instance.currentActivePlayer];

        //.25f pitch for bad
        //normal for good
        yield return new WaitForSeconds(.75f);
        GameOverUI.SetActive(true);
        SoundManager.instance.playSound(0,.25f);
        yield return new WaitForSeconds(.75f);
        causeOfDeath.enabled = true;

        yield return new WaitForSeconds(.5f);
        daysCompleted.enabled = true;
        yield return new WaitForSeconds(.75f);
        daysCompletedValue.text = "" + numOfDaysCompleted[multiplayerManager.instance.currentActivePlayer];
        daysCompletedValue.enabled = true;
        if (numOfDaysCompleted[multiplayerManager.instance.currentActivePlayer] < 3)
            SoundManager.instance.playSound(0, .25f);
        else
            SoundManager.instance.playSound(0, 0.95f);

        yield return new WaitForSeconds(.5f);
        emailsFiled.enabled = true;
        yield return new WaitForSeconds(.75f);
        if (correct != 0)
            emailsFiledValue.text = (int)(correct / total * 100) + "%";
        else
            emailsFiledValue.text = "0%";
        emailsFiledValue.enabled = true;
        if ((correct / total * 100) < 51 || correct ==0)
            SoundManager.instance.playSound(0, .25f);
        else
            SoundManager.instance.playSound(0, 0.95f);

        yield return new WaitForSeconds(.5f);
        emailsHandled.enabled = true;
        yield return new WaitForSeconds(.75f);
        emailsHandledValue.text = "" + total;
        emailsHandledValue.enabled = true;
        if (total < 8)
            SoundManager.instance.playSound(0, .25f);
        else
            SoundManager.instance.playSound(0, .95f);

        yield return new WaitForSeconds(.5f);
        professionalism.enabled = true;
        yield return new WaitForSeconds(.75f);

        CalculateProfessionalism();

        professionalismValue.enabled = true;
        if (Mathf.Max(0, ((100 - (int)(messyDesks[multiplayerManager.instance.currentActivePlayer] / 47f * 100)) - (bossAngeredDay[multiplayerManager.instance.currentActivePlayer] * 10))) < 50)
            SoundManager.instance.playSound(0, .25f);
        else
            SoundManager.instance.playSound(0, .95f);


        yield return new WaitForSeconds(.5f);
        finalScore.enabled = true;
        yield return new WaitForSeconds(1f);
        finalScoreValue.enabled = true;

        score[multiplayerManager.instance.currentActivePlayer] += scoreToAdd[multiplayerManager.instance.currentActivePlayer];
        scoreToAdd[multiplayerManager.instance.currentActivePlayer] = 0;

        //Count up to score
        int temp=0;
        if (score[multiplayerManager.instance.currentActivePlayer] > 0)
        {
            while (temp < score[multiplayerManager.instance.currentActivePlayer])
            {
                temp += 10;
                finalScoreValue.text = "" + temp;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (temp > score[multiplayerManager.instance.currentActivePlayer])
            {
                temp -= 10;
                finalScoreValue.text = "" + temp;
                yield return new WaitForEndOfFrame();
            }

        }

        while (!Input.GetButtonDown("Fire1"))
            yield return null;

        GameOverUI.SetActive(false);
        
        //Reset everything
        causeOfDeath.enabled = false;
        daysCompleted.enabled = false;
        daysCompletedValue.enabled = false;
        emailsFiled.enabled = false;
        emailsFiledValue.enabled = false;
        emailsHandled.enabled = false;
        emailsHandledValue.enabled = false;
        professionalism.enabled = false;
        professionalismValue.enabled = false;
        finalScore.enabled = false;
        finalScoreValue.enabled = false;

        bossAngeredDay[multiplayerManager.instance.currentActivePlayer] = 0;
        LeaderBoard.instance.SetScore(score[multiplayerManager.instance.currentActivePlayer]);
    }

    public int GetScore()
    {
        return score[multiplayerManager.instance.currentActivePlayer];
    }

    public void CalculateProfessionalism()
    {
        if (messyDesks[multiplayerManager.instance.currentActivePlayer] != 0 || bossAngeredDay[multiplayerManager.instance.currentActivePlayer] != 0)
        {
            totalProfessionalism[multiplayerManager.instance.currentActivePlayer] += Mathf.Max(0,((100 - (int)(messyDesks[multiplayerManager.instance.currentActivePlayer] / 47f * 100)) - (bossAngeredDay[multiplayerManager.instance.currentActivePlayer] * 10)));
            professionalismValue.text = Mathf.Max(0, ((100 - (int)(messyDesks[multiplayerManager.instance.currentActivePlayer] / 47f * 100)) - (bossAngeredDay[multiplayerManager.instance.currentActivePlayer] * 10))) + "%";//NOT A MAGIC NUMBER HONEST, 47 is the current number of desks you can mess up
        }
        else
        {
            totalProfessionalism[multiplayerManager.instance.currentActivePlayer] += 100;
            professionalismValue.text = "100%";
        }
        messyDesks[multiplayerManager.instance.currentActivePlayer] = 0;
    }
}
