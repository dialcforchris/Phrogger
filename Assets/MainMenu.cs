using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MainMenu : MonoBehaviour {

    int menuIndex;
    public menuState currentState = menuState.mainMenu;

    public enum menuState
    {
        mainMenu,
        credits,
        leaderboard,
        gameplay
    }

    public Text[] menuItems;
    public Image[] menuImages;
    public Image creditBackdrop;
    public Text TitleText,Credits;
    
    bool scrolling;
    void Update ()
    {
        if (currentState == menuState.mainMenu)
        {
            if (Input.GetAxis("Vertical") != 0 && scrolling == false)
            {
                scrolling = true;
                menuItems[menuIndex].color = Color.white;
                menuIndex -= (Input.GetAxis("Vertical") > 0 ? 1 : -1);
                if (menuIndex < 0)
                    menuIndex = menuItems.Length - 1;
                if (menuIndex > menuItems.Length - 1)
                    menuIndex = 0;
                SoundManager.instance.playSound(0, .75f);
                menuItems[menuIndex].color = Color.green;
            }
            else if (Input.GetAxis("Vertical") == 0)
            {
                scrolling = false;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                switch (menuIndex)
                {
                    case 0:
                        //Fade things out
                        //Peel back black bars
                        //Hand control over to the player
                        StartCoroutine(StartGame());
                        currentState = menuState.gameplay;
                        break;
                    case 1:
                        break;
                    case 2:
                        currentState = menuState.credits;
                        StartCoroutine(runCredits());
                        break;
                    case 3:
                        Application.Quit();
                        break;
                }
            }
        }
    }

    IEnumerator FadeInOutMainMenuUI(bool inOut)//True for in, false for out
    {
        while ((inOut) ? menuItems[0].color.a < 1 : menuItems[0].color.a > 0)
        {
            foreach (Image i in menuImages)
            {
                Color col = i.color;
                col.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                i.color = col;
            }
            foreach (Text t in menuItems)
            {
                Color col = t.color;
                col.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                t.color = col;

                Color outlineCol = t.GetComponent<Outline>().effectColor;
                outlineCol.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                t.GetComponent<Outline>().effectColor = outlineCol;
            }
            Color col_ = TitleText.color;
            col_.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
            TitleText.color = col_;
            Color outlineCol_ = TitleText.GetComponent<Outline>().effectColor;
            outlineCol_.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
            TitleText.GetComponent<Outline>().effectColor = outlineCol_;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartGame()
    {
        yield return StartCoroutine(FadeInOutMainMenuUI(false));
        float lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime * 5;
            if (lerpy > 1)
                lerpy = 1;

            //Black bars yo
            CameraZoom.instance.overlayBot.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(960, -410), new Vector2(960, -540), lerpy);
            CameraZoom.instance.overlayTop.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(960, 540), new Vector2(960, 670), lerpy);
            yield return new WaitForEndOfFrame();
        }
        GameStateManager.instance.ChangeState(GameStates.STATE_FROGGER);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator runCredits()
    {
        yield return StartCoroutine(FadeInOutMainMenuUI(false));

        //Fade into black backdrop
        while (creditBackdrop.color.a < .5f)
        {
            Color colB_ = creditBackdrop.color;
            colB_.a += Time.deltaTime;
            creditBackdrop.color = colB_;

            yield return new WaitForEndOfFrame();
        }

        //Scroll credits up
        float lerpy = 0;
        while (Credits.rectTransform.anchoredPosition.y < 1000)
        {
            Credits.rectTransform.anchoredPosition = Vector2.Lerp(-Vector2.up * 1000, Vector2.up * 1000, lerpy/25);
            if (Input.GetButton("Fire1"))
                lerpy += Time.deltaTime*5;

            lerpy += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Reset credit location
        Credits.rectTransform.anchoredPosition = -Vector2.up * 1000;

        //Fade out credit backdrop
        while (creditBackdrop.color.a > 0f)
        {
            Color newcol = creditBackdrop.color;
            newcol.a -= Time.deltaTime;
            creditBackdrop.color = newcol;

            yield return new WaitForEndOfFrame();
        }

        //Fade Main menu back in
        yield return StartCoroutine(FadeInOutMainMenuUI(true));
        currentState = menuState.mainMenu;
    }
}
