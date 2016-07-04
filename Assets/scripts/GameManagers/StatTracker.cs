using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StatTracker : MonoBehaviour
{
    public static StatTracker instance;

    public int junkEmailsCorrect, safeEmailsCorrect, safeEmailsWrong, junkEmailsWrong, numOfDaysCompleted, messyDesks;
    public int totalDeaths, bossDeaths, bossAngered;
    public int score;
    public float totalProfessionalism = 0;
    public int scoreToAdd;
    public Text ScoreText;
    List<int> dayPerformances = new List<int>();

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
        instance = this;
        numOfDaysCompleted = 0;
    }
    public void changeLifeCount(int l, bool gainLoss)
    {
        if (gainLoss)
            lifeAnimators[l].Play("life_gain");
        else
            lifeAnimators[l].Play("life_loss");
    }

    public float getAveragePerformance()
    {
        float total=0;
        for (int i=0; i < dayPerformances.Count;i++)
        {
            total += dayPerformances[i];
        }

        return total / dayPerformances.Count;
    }

    public void addDayPerformance(int performance)
    {
        dayPerformances.Add(performance);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (scoreToAdd > 0)
            {
                scoreToAdd--;
                score++;
                ScoreText.text = "" + score;
            }
            else if (scoreToAdd < 0)
            {
                scoreToAdd++;
                score--;
                ScoreText.text = "" + score;
            }
        }
    }

    public IEnumerator GameOverUIReveal()
    {
        gameoverScreen.gameObject.SetActive(true);

        float total = junkEmailsCorrect + junkEmailsWrong + safeEmailsCorrect + safeEmailsWrong;
        float correct = junkEmailsCorrect + safeEmailsCorrect;

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
        daysCompletedValue.text = "" + numOfDaysCompleted;
        daysCompletedValue.enabled = true;
        if (numOfDaysCompleted < 3)
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
        if (100 - (messyDesks / 28f * 100) < 50)
            SoundManager.instance.playSound(0, .25f);
        else
            SoundManager.instance.playSound(0, .95f);


        yield return new WaitForSeconds(.5f);
        finalScore.enabled = true;
        yield return new WaitForSeconds(1f);
        finalScoreValue.enabled = true;

        score += scoreToAdd;
        scoreToAdd = 0;

        //Count up to score
        int temp=0;
        if (score > 0)
        {
            while (temp < score)
            {
                temp += 10;
                finalScoreValue.text = "" + temp;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (temp > score)
            {
                temp -= 10;
                finalScoreValue.text = "" + temp;
                yield return new WaitForEndOfFrame();
            }

        }

        while (!Input.GetButtonDown("Fire1"))
            yield return null;

        GameOverUI.SetActive(false);
        LeaderBoard.instance.SetScore(score);
        
    }

    public int GetScore()
    {
        return score;
    }

    public void CalculateProfessionalism()
    {
        if (messyDesks != 0)
        {
            totalProfessionalism += (100 - (int)(messyDesks / 47f * 100));
            professionalismValue.text = 100 - (int)(messyDesks / 47f * 100) + "%";//NOT A MAGIC NUMBER HONEST, 47 is the current number of desks you can mess up
        }
        else
        {
            totalProfessionalism += 100;
            professionalismValue.text = "100%";
        }
        messyDesks = 0;
    }
}
