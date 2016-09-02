using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
    public static MainMenu instance;

    int menuIndex,gameModeIndex=1,playerSelectionIndex=1;
    public menuState currentState = menuState.mainMenu;
    public enum menuState
    {
        mainMenu,
        credits,
        leaderboard,
        gameplay,
        modeSelect,
        playerSelect
    }

    public Text[] menuItems;
    public Image[] menuImages;
    public Image creditBackdrop,logo,fadeMe;
    public Text Credits;
    public GameObject leaderBoard;

    // public Image creditBackdrop,logo;
    // public Text Credits;

    [Header("Game mode options")]
    public Text[] GameModeOptions;

    [Header("Player options")]
    public Text[] PlayerOptions;

    void Awake()
    {
        instance = this;
        StartCoroutine(wholeScreenFade(false));
    }

    public IEnumerator wholeScreenFade(bool b,int sceneToLoad=0) //False for fade in from black, true to fade to black and reload scene
    {
        yield return new WaitForSeconds(.25f);
        if (!b)
        {
            while (fadeMe.color.a > 0)
            {
                Color col = fadeMe.color;
                col.a -= Time.fixedDeltaTime * 2;
                fadeMe.color = col;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (fadeMe.color.a < 1)
            {
                Color col = fadeMe.color;
                col.a += Time.fixedDeltaTime;
                fadeMe.color = col;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene(0);
        }
    }

    bool scrolling;
    void Update ()
    {
        #region main menu
        if (currentState == menuState.mainMenu)
        {
            if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) != 0 && scrolling == false)
            {
                scrolling = true;
                menuItems[menuIndex].color = Color.white;
                menuIndex -= (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) > 0 ? 1 : -1);
                if (menuIndex < 0)
                    menuIndex = menuItems.Length - 2;
                if (menuIndex > menuItems.Length - 2)
                    menuIndex = 0;
                
                SoundManager.instance.playSound(0, Random.Range(.7f, .8f));
                menuItems[menuIndex].color = Color.green;
            }
            else if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) == 0)
            {
                scrolling = false;
            }

            if (Input.GetButtonDown("Fire"+multiplayerManager.instance.currentActivePlayer.ToString()))
            {
                SoundManager.instance.playSound(0, Random.Range(.7f,.8f));
                switch (menuIndex)
                {
                    case 0:
                        PlayerOptions[playerSelectionIndex].color = Color.white;
                        playerSelectionIndex = 1;
                        PlayerOptions[playerSelectionIndex].color = Color.green;
                        menuImages[0].gameObject.SetActive(false);
                        menuImages[1].gameObject.SetActive(false);
                        menuImages[2].gameObject.SetActive(true);
                        currentState = menuState.playerSelect;
                        break;
                    case 1:
                        currentState = menuState.leaderboard;
                        StartCoroutine(LeaderBoard());
                       
                        break;
                    case 2:
                        currentState = menuState.credits;
                        StartCoroutine(runCredits());
                        break;
                    case 3:
                        //Application.Quit();
                        break;
                }
            }
        }
        #endregion
        #region gamemode selection
        else if (currentState == menuState.modeSelect)
        {
            if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) != 0 && scrolling == false)
            {
                scrolling = true;
                GameModeOptions[gameModeIndex].color = Color.white;
                gameModeIndex -= (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) > 0 ? 1 : -1);
                if (gameModeIndex < 1)
                    gameModeIndex = GameModeOptions.Length - 1;
                if (gameModeIndex > GameModeOptions.Length - 1)
                    gameModeIndex = 1;
                SoundManager.instance.playSound(0, .75f);
                GameModeOptions[gameModeIndex].color = Color.green;
            }
            else if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) == 0)
            {
                scrolling = false;
            }

            if (Input.GetButtonDown("Fire" + multiplayerManager.instance.currentActivePlayer.ToString()))
            {
                SoundManager.instance.playSound(0, .75f);
                switch (gameModeIndex)
                {
                    case 1:
                        dayTimer.instance.maxDays = 1;

                        if (multiplayerManager.instance.numberOfPlayers == 1)
                            StartCoroutine(StartGame());
                        else
                        {
                            StartCoroutine(FadeInOutMainMenuUI(false));

                            //Turn off all the frogger sounds
                            Player.instance.silenceOriginalFroggerSounds(0.75f);
                            Player.instance.froggerCompleted = true;
                            //Remember to disable hud things
                            dayTimer.instance.NewDayTransition();
                        }
                        currentState = menuState.gameplay;
                        break;
                    case 2:
                        dayTimer.instance.maxDays = 5;

                        if (multiplayerManager.instance.numberOfPlayers == 1)
                            StartCoroutine(StartGame());
                        else
                        {
                            StartCoroutine(FadeInOutMainMenuUI(false));

                            //Turn off all the frogger sounds
                            Player.instance.silenceOriginalFroggerSounds(0.75f);
                            Player.instance.froggerCompleted = true;
                            //Remember to disable hud things
                            dayTimer.instance.NewDayTransition();
                        }

                            currentState = menuState.gameplay;
                        break;
                    case 3:
                        PlayerOptions[playerSelectionIndex].color = Color.white;
                        playerSelectionIndex = 1;
                        PlayerOptions[playerSelectionIndex].color = Color.green;
                        menuImages[1].gameObject.SetActive(false);
                        menuImages[2].gameObject.SetActive(true);
                        currentState = menuState.playerSelect;
                        break;
                }
            }
        }
        #endregion
        #region player selection
        else if (currentState == menuState.playerSelect)
        {
            if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) != 0 && scrolling == false)
            {
                scrolling = true;
                PlayerOptions[playerSelectionIndex].color = Color.white;
                playerSelectionIndex -= (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) > 0 ? 1 : -1);
                if (playerSelectionIndex < 1)
                    playerSelectionIndex = PlayerOptions.Length - 1;
                if (playerSelectionIndex > PlayerOptions.Length - 1)
                    playerSelectionIndex = 1;
                SoundManager.instance.playSound(0, .75f);
                PlayerOptions[playerSelectionIndex].color = Color.green;
            }
            else if (Input.GetAxis("VerticalStick" + multiplayerManager.instance.currentActivePlayer.ToString()) == 0)
            {
                scrolling = false;
            }

            if (Input.GetButtonDown("Fire" + multiplayerManager.instance.currentActivePlayer.ToString()))
            {
                SoundManager.instance.playSound(0, .75f);
                switch (playerSelectionIndex)
                {
                    case 1:
                        GameModeOptions[gameModeIndex].color = Color.white;
                        gameModeIndex = 1;
                        GameModeOptions[gameModeIndex].color = Color.green;
                        multiplayerManager.instance.numberOfPlayers = 1;
                        menuImages[1].gameObject.SetActive(true);
                        menuImages[2].gameObject.SetActive(false);
                        currentState = menuState.modeSelect;
                        break;
                    case 2:
                        GameModeOptions[gameModeIndex].color = Color.white;
                        gameModeIndex = 1;
                        GameModeOptions[gameModeIndex].color = Color.green;
                        multiplayerManager.instance.numberOfPlayers = 2;
                        menuImages[1].gameObject.SetActive(true);
                        menuImages[2].gameObject.SetActive(false);
                        currentState = menuState.modeSelect;
                        break;
                    case 3:
                        menuImages[0].gameObject.SetActive(true);
                        menuImages[1].gameObject.SetActive(false);
                        menuImages[2].gameObject.SetActive(false);
                        currentState = menuState.mainMenu;
                        break;
                }
            }
        }
        #endregion

        if (!Input.anyKey && Input.GetAxis("VerticalStick0")<.1f && Input.GetAxis("VerticalStick1") < .1f)
        {
            idleTime += Time.deltaTime;
        }
        else
        {
            idleTime = 0;
        }

        if (idleTime > 150 && GameStateManager.instance.GetState() != GameStates.STATE_DAYOVER)
        {
            GameStateManager.instance.ChangeState(GameStates.STATE_DAYOVER);
            StartCoroutine(wholeScreenFade(true, 0));
        }
    }
    [SerializeField]
    float idleTime;
    IEnumerator FadeInOutMainMenuUI(bool inOut,bool fadeLogo = true)//True for in, false for out
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
            foreach (Text t in GameModeOptions)
            {
                Color col = t.color;
                col.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                t.color = col;

                Color outlineCol = t.GetComponent<Outline>().effectColor;
                outlineCol.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                t.GetComponent<Outline>().effectColor = outlineCol;
            }
            if (fadeLogo)
            {
                Color col_ = logo.color;
                col_.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                logo.color = col_;
                Color outlineCol_ = logo.GetComponent<Outline>().effectColor;
                outlineCol_.a -= (inOut) ? -Time.deltaTime * 2 : Time.deltaTime * 2;
                logo.GetComponent<Outline>().effectColor = outlineCol_;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator SwitchViews(bool direction)
    {
        float lerpy = 0;
        if (direction)
        {
            while (lerpy < 1)
            {
                lerpy += Time.deltaTime;
                menuImages[0].rectTransform.anchoredPosition = Vector2.Lerp(menuImages[0].rectTransform.anchoredPosition, new Vector2(-1000, -150), lerpy);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (lerpy < 1)
            {
                lerpy += Time.deltaTime;
                menuImages[0].rectTransform.anchoredPosition = Vector2.Lerp(menuImages[0].rectTransform.anchoredPosition, new Vector2(0, -150), lerpy);
                yield return new WaitForEndOfFrame();
            }
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
        while (Credits.rectTransform.anchoredPosition.y < 1750)
        {
            Credits.rectTransform.anchoredPosition = Vector2.Lerp(-Vector2.up * 750, Vector2.up * 1750, lerpy / 25);
            if (Input.GetButton("Fire" + multiplayerManager.instance.currentActivePlayer.ToString()))
                lerpy += Time.deltaTime * 5;

            lerpy += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Reset credit location
        Credits.rectTransform.anchoredPosition = -Vector2.up * 750;

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
    IEnumerator LeaderBoard()
    {
        yield return StartCoroutine(SwitchViews(true));
        yield return WaitForKeyDown("Fire" + multiplayerManager.instance.currentActivePlayer.ToString());
        yield return StartCoroutine(SwitchViews(false));
        currentState = menuState.mainMenu;
    }
    IEnumerator WaitForKeyDown(string fire)
    {
        while (!Input.GetButton(fire))
            yield return null;
    }
}
