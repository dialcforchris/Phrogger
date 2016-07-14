using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterName : MonoBehaviour
{
    public Text[] box;
    int[] currentCharacter;
    public Text score;
    int selectBox = 0;
    int selectChar= 65;
    float coolDown = 0;
    float maxCool = 0.2f;
    string theName;
    bool finished;
  
	void Start ()
    {
        coolDown = maxCool;
        currentCharacter = new int[box.Length];
        finished = false;
        theName = string.Empty;
        foreach(Text t in box)
        {
            t.text = "A";
        }
        for (int i = 0; i < currentCharacter.Length;i++ )
        {
            currentCharacter[i] = 65;
        }
	}

    // Update is called once per frame
    void Update()
    {
        MenuInput();
        box[selectBox].text = ((char)currentCharacter[selectBox]).ToString();
        ChangeTextColour();
        score.text = "Your Score: " + StatTracker.instance.GetScore().ToString();
        if (Input.GetButtonDown("Fire" + multiplayerManager.instance.currentActivePlayer.ToString()) && !finished)
            SelectName();
    }

    void MenuInput()
    {
        if (ConvertToPos("HorizontalStick" + multiplayerManager.instance.currentActivePlayer.ToString(),
            "VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()))
        {
            if (Input.GetAxis("HorizontalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("HorizontalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) > 0)
                {
                    if (selectBox == box.Length - 1)
                    {
                        selectBox = 0;
                    }
                    else
                    {
                        selectBox++;
                    }
                }
                else if (Input.GetAxis("HorizontalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) < 0)
                {
                    if (selectBox == 0)
                    {
                        selectBox = box.Length - 1;
                    }
                    else
                    {
                        selectBox--;
                    }
                }
                selectChar = currentCharacter[selectBox];
                coolDown = 0;
            }
        }
        else
        {
            if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) < 0)
                {
                    if (selectChar > 65)
                        selectChar--;

                    else
                        selectChar = 90;
                }
                else if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) > 0)
                {
                    if (selectChar < 90)
                        selectChar++;

                    else
                        selectChar = 65;
                }
                //if (Input.GetAxis("Vertical") > 0 && selectChar < 90)
                //if (selectChar == 90)
                //{
                //    selectChar = 65;
                //}
                //else
                //{
                //    selectChar++;
                //}
                coolDown = 0;
                currentCharacter[selectBox] = selectChar;
            }
        }
    }

    bool SelectCoolDown()
    {
        if (coolDown<maxCool)
        {
            coolDown += Time.deltaTime;
            return false;
        }
        else
        {
            return true;
        }
    }

    void ChangeTextColour()
    {
       for (int i=0;i<box.Length;i++)
       {
           if (selectBox == i)
           {
               box[i].color = Color.green;
           }
           else
           {
               box[i].color = Color.white;
           }
        }
    }
    
    void SelectName()
    {
        for (int i = 0; i < box.Length; i++)
        {
            theName = theName + box[i].text;
        }
        LeaderBoard.instance.SetName(theName);
        LeaderBoard.instance.AddNewScoreToLB();
        finished = true;

        //Reset this whole thing
        Start();
        gameObject.SetActive(false);
        
        multiplayerManager.instance.NextPlayer();
        //Check if the other player has finished playing
        if (multiplayerManager.instance.finishedPlaying[multiplayerManager.instance.currentActivePlayer])
        {
            //If both players have failed, trigger game over.
            if (!multiplayerManager.instance.win[0] && !multiplayerManager.instance.win[1])
                gameOverScreen.instance.StartCoroutine(gameOverScreen.instance.TriggerGameOver());
            else //Otherwise just fade to black
                MainMenu.instance.StartCoroutine(MainMenu.instance.wholeScreenFade(true));
        }
        else
        {
            //The other player hasn't finished playing yet.
            if (StatTracker.instance.numOfDaysCompleted[multiplayerManager.instance.currentActivePlayer] < dayTimer.instance.maxDays)
            {
                //Go to the players next day if they haven't done all the days yet
                dayTimer.instance.NewDayTransition();
            }
            else
            {
                //Or if they have, just go to the end of the game.
                dayTimer.instance.StartCoroutine(dayTimer.instance.FinishGame());
            }
        }
    }
    bool ConvertToPos(string ho, string ve)
    {
        float hori = Input.GetAxis(ho);
        float verti = Input.GetAxis(ve);
        if (hori < 0)
        {
            hori -= hori * 2;
        }
        if (verti < 0)
        {
            verti -= verti * 2;
        }
        if (hori > verti)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
