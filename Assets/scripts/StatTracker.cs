using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatTracker : MonoBehaviour
{
    public static StatTracker instance;

    public int junkEmailsCorrect, safeEmailsCorrect, safeEmailsWrong, junkEmailsWrong;
    int score;
    public int scoreToAdd;
    public Text ScoreText,LivesText;

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
}
