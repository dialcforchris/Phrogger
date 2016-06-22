using UnityEngine;
using System.Collections;

public class BossFace : WorldObject

{
    public static BossFace instance = null;
    public Sprite[] faceList;
    public Sprite bossGone;
//    public SpriteRenderer spriteRenderer;
    public GameObject eyes;
    public Player player;
    float bossAngerAddition = 0.45f;
    float emailCool = 0;
    float emailMaxCool = 10;
    float playerCool = 0;
    float playerMaxCool= 1.5f;
    Tile playerTile;
    float bossAngerExp = 0;
    int bossAngerLevel = 0;
    FaceState faceState;
	// Use this for initialization
	void Awake () 
    {
        if (instance==null)
        {
            instance = this;
        }
        faceState = FaceState.UI;
	}
	
	// Update is called once per frame
	void Update () 
    {
        MoveEyes();
        ManyFacedBoss();
        AddToAnger();
        CoolDown();
        States(faceState);
	}

    void CoolDown()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_EMAIL)
        {
            playerCool = 0;
            if (emailCool< emailMaxCool)
            {
                emailCool += Time.deltaTime;
            }
            else
            {
                bossAngerExp += bossAngerAddition;
                emailCool = 0;
            }
        }
        else if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY&&player.playerState==PlayerState.ACTIVE)
        {
            
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
                    bossAngerExp += bossAngerAddition;
                }
                playerCool = 0;
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
        if (bossAngerLevel <= faceList.Length - 1)
        {
            spriteRenderer.sprite = faceList[bossAngerLevel];
        }
    }

        void AddToAnger()
        {
            if (bossAngerExp >= 1)
            {
                if (bossAngerLevel < faceList.Length - 1)
                {
                    bossAngerLevel++;
                }
                else
                {
                    faceState = FaceState.CHASE;
                }
                bossAngerExp = 0;
            }
        }
   public void AngerLevelAdj(int amount = 1)
    {
        bossAngerLevel += amount;
    }

   public void CheckEmails(bool _correct)
   {
       if (!_correct)
       {
           bossAngerExp += bossAngerAddition;
       }
   }

   void States(FaceState _state)
   {
       switch (_state)
       {
           case FaceState.UI:
               {
                   spriteRenderer.sprite = faceList[bossAngerLevel];
                   break;
               }
           case FaceState.CHASE:
               {
                   spriteRenderer.sprite = bossGone;
                   break;
               }
       }
   }

   void SummonBoss()
   {
       if (bossAngerLevel >= faceList.Length - 1)
       {
           faceState = FaceState.CHASE;
       }
   }

   public void ChangeStateBack()
   {
       faceState = FaceState.UI;
       bossAngerLevel--;
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
    public override void Reset()
   {
       bossAngerExp = 0;
       bossAngerLevel = 0;
       faceState = FaceState.UI;
       spriteRenderer.sprite = faceList[bossAngerLevel];
       emailCool = 0;
       playerCool = 0;
   }
}
