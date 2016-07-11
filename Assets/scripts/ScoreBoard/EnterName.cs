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
    string theName = string.Empty;
    bool finished;
  
	void Start () 
    {
        coolDown = maxCool;
        currentCharacter = new int[box.Length];
        for (int i = 0; i < currentCharacter.Length;i++ )
        {
            currentCharacter[i] = 65;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
            MenuInput();
            box[selectBox].text = ((char)currentCharacter[selectBox]).ToString();
            ChangeTextColour();
            score.text = "Your Score: " +StatTracker.instance.GetScore().ToString();
            if (Input.GetButtonDown("Fire1") && !finished)
                SelectName();
    }

    void MenuInput()
    {
        if (ConvertToPos("Horizontal", "Vertical"))
        {
            if (Input.GetAxis("Horizontal") != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("Horizontal") > 0)
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
                else if (Input.GetAxis("Horizontal") < 0)
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
            if (Input.GetAxis("Vertical") != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("Vertical") < 0)
                {
                    if (selectChar > 65)
                        selectChar--;

                    else
                        selectChar = 90;
                }
                else if (Input.GetAxis("Vertical") > 0)
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
        if (Player.instance.strikes == 0)
        {
            gameOverScreen.instance.StartCoroutine(gameOverScreen.instance.TriggerGameOver());
            gameObject.SetActive(false);
        }
        else
            StartCoroutine(MainMenu.instance.wholeScreenFade(true));
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
        if (hori>verti)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
