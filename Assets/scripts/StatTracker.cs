using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatTracker : MonoBehaviour
{
    public static StatTracker instance;

    public int junkEmailsCorrect, safeEmailsCorrect, safeEmailsWrong, junkEmailsWrong,numOfDaysCompleted;
    int score;
    public int scoreToAdd;
    public Text ScoreText, LivesText;
    [Header("Game Over UI")]
    public GameObject GameOverUI;
    public Text causeOfDeath,daysCompleted, daysCompletedValue, emailsFiled, emailsFiledValue, emailsHandled, emailsHandledValue, finalScore, finalScoreValue;

    [Header("")]
    public float bossAngerLevel;

    void Awake()
    {
        instance = this;
    }
    public void changeLifeCount(int i)
    {
        LivesText.text = "x" + i;
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
        Debug.Log("GameOverState");

        float total = junkEmailsCorrect + junkEmailsWrong + safeEmailsCorrect + safeEmailsWrong;
        float correct = junkEmailsCorrect + safeEmailsCorrect;


        yield return new WaitForSeconds(1.5f);
        GameOverUI.SetActive(true);
        SoundManager.instance.playSound(0);
        yield return new WaitForSeconds(1.5f);
        causeOfDeath.enabled = true;
        SoundManager.instance.playSound(0);

        yield return new WaitForSeconds(1f);
        daysCompleted.enabled = true;
        SoundManager.instance.playSound(0);
        yield return new WaitForSeconds(1.5f);
        daysCompletedValue.text = ""+numOfDaysCompleted;
        daysCompletedValue.enabled = true;
        SoundManager.instance.playSound(0);

        yield return new WaitForSeconds(1f);
        emailsFiled.enabled = true;
        SoundManager.instance.playSound(0);
        yield return new WaitForSeconds(1.5f);
        emailsFiledValue.text = (int)(correct / total* 100) + "%";
        emailsFiledValue.enabled = true;
        SoundManager.instance.playSound(0);

        yield return new WaitForSeconds(1f);
        emailsHandled.enabled = true;
        SoundManager.instance.playSound(0);
        yield return new WaitForSeconds(1.5f);
        emailsHandledValue.text = "" + total;
        emailsHandledValue.enabled = true;
        SoundManager.instance.playSound(0);

        yield return new WaitForSeconds(1f);
        finalScore.enabled = true;
        SoundManager.instance.playSound(0);
        yield return new WaitForSeconds(1.5f);
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
        SoundManager.instance.playSound(0);
    }
}
