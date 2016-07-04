using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossFace :MonoBehaviour
{
    public static BossFace instance = null;
    [SerializeField]
    private SpriteRenderer spriteRenderer = null;
    [SerializeField]
    private Animator bossFaceAnimator = null;
    public Sprite[] faceList;
    public Sprite bossGone;
    public ParticleSystem steam;
    public GameObject eyes;
    public Player player;
    public Slider[] XPbar;
    public float bossAngerAddition = 0.35f;
    float emailCool = 0;
    float emailTimeLeniency = 0;
    float emailMaxCool = 20;
    float playerCool = 0;
    float playerMaxCool = 1.5f;
    float workCool = 0;
    float maxWorkCool = 10;
    float bossAngerExp = 0;
    float XPtoAdd = 0;
    float venganceCool = 2.5f;
    Tile playerTile;
    int bossAngerLevel = 0;
    private bool slacker = false;
    [SerializeField]
    AudioClip kettle;
    
    FaceState faceState;

    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ChangeState(FaceState.UI);
    }

    public void addEmailAngerXP()
    {
        XPtoAdd += (emailCool / emailMaxCool) * bossAngerAddition;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (XPtoAdd > 0)
            {
                XPtoAdd -= Time.deltaTime;
                if (XPtoAdd < 0)
                {
                    bossAngerExp += (Time.deltaTime + XPtoAdd);
                    XPtoAdd = 0;
                }
                else
                {
                    bossAngerExp += Time.deltaTime;
                }
            }
            XPtoAdd += Time.deltaTime * 0.0325f;
        }

        XPbar[0].value = bossAngerLevel+ bossAngerExp;
        XPbar[1].value = bossAngerLevel + bossAngerExp;
        MoveEyes();
        ManyFacedBoss();
        AddToAnger();
        CoolDown();
        SteamParticles();
    }

    void CoolDown()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_EMAIL)
        {
            slacker = false;
            playerCool = 0;
            workCool = 0;
            //Only do when we've been going for a few seconds
            if (mailOpener.instance.activeCountdown)
            {
                emailTimeLeniency += Time.deltaTime;
                //
                if (emailTimeLeniency > 4.5f)
                {
                    if (emailCool < emailMaxCool)
                    {
                        emailCool += Time.deltaTime;
                        mailOpener.instance.angerMeter.value = emailCool;

                        //Anger bar colour lerp from green to red
                        //ColorBlock cols = mailOpener.instance.angerMeter.colors;
                        //cols.disabledColor = Color.Lerp(Color.green, Color.red, (emailCool / (emailMaxCool * .75f)));
                        //mailOpener.instance.angerMeter.colors = cols;
                    }
                }
            }
        }
        else if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY && player.playerState == PlayerState.ACTIVE)
        {
            emailTimeLeniency = 0;
            emailCool = 0;
            if (playerCool < playerMaxCool)
            {
                if (playerCool == 0)
                {
                    playerTile = TileManager.instance.GetTile(player.transform.position);
                }
                playerCool += Time.deltaTime;
            }
            else
            {
                if (playerTile == TileManager.instance.GetTile(player.transform.position))
                {
                    XPtoAdd += bossAngerAddition;
                }
                playerCool = 0;
            }
            float appliedWorkCool = slacker ? venganceCool : maxWorkCool;
            if (workCool < appliedWorkCool)
            {
                workCool += Time.deltaTime;
            }
            else
            {
                slacker = true;
                XPtoAdd += bossAngerAddition;
                workCool = 0;
            }
        }
    }

    void MoveEyes()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            eyes.transform.position = new Vector2(transform.position.x +
            ((Camera.main.WorldToViewportPoint(player.transform.position).x * 2 - 1) / 10), eyes.transform.position.y);
        }
    }

    void ManyFacedBoss()
    {
        if (bossAngerLevel < faceList.Length)
        {
            spriteRenderer.sprite = faceList[bossAngerLevel];
        }
    }

    void AddToAnger()
    {
        if (bossAngerExp >= 1)
        {
            if (bossAngerLevel < faceList.Length)
            {
                bossAngerLevel++;
                if (bossAngerLevel == faceList.Length)
                {
                    ChangeState(FaceState.CHASE);
                }
                else
                {
                    bossAngerExp -= 1;
                }
            }
        }
    }
    public void AngerLevelAdj(int amount = 1)
    {
        bossAngerLevel += amount;
    }
    void SteamParticles()
    {

        if (bossAngerLevel == 3 && faceState == FaceState.UI)
        {
            if (!steam.isPlaying)
            {
                steam.Play();
                SoundManager.instance.playSound(kettle);
            }
        }
        else
        {
            if (steam.isPlaying)
            {
                steam.Stop();
                SoundManager.instance.StopSound(kettle);
            }
        }
    }

    public void CheckEmails(bool _correct)
    {
        if (!_correct)
        {
            XPtoAdd += (bossAngerAddition * 2.5f);
        }
    }

    void ChangeState(FaceState _state)
    {
        if (faceState != _state)
        {
            faceState = _state;
            switch (faceState)
            {
                case FaceState.UI:
                    {
                        spriteRenderer.sprite = faceList[bossAngerLevel];
                        break;
                    }
                case FaceState.CHASE:
                    {
                        spriteRenderer.sprite = bossGone;
                        bossFaceAnimator.Play("boss_leave");
                        Invoke("bossChase", .5f);
                        break;
                    }
            }
        }
    }

    void bossChase()
    {
        Boss.instance.BeginChase();
    }

    public void ChangeStateBack()
    {
        XPtoAdd = 0;
        bossAngerExp = 0;
        bossAngerLevel -= 2;
        ChangeState(FaceState.UI);
    }

    public void NextEmail()
    {
        emailCool = 0;
    }

    public enum FaceState
    {
        UI,
        CHASE,
    }
    public void Reset()
    {
        XPtoAdd = 0;
        bossAngerExp = 0;
        bossAngerLevel = 0;
        ChangeState(FaceState.UI);
        spriteRenderer.sprite = faceList[bossAngerLevel];
        emailCool = 0;
        playerCool = 0;
        steam.Stop();
    }
}
