using UnityEngine;
using System.Collections;

public class BossFace : MonoBehaviour 

{
    public static BossFace instance = null;
    public Sprite[] faceList;
    public SpriteRenderer spriteRenderer;
    public GameObject eyes;
    public GameObject player;
    float emailCool = 0;
    float emailMaxCool = 6;
    float playerCool = 0;
    float playerMaxCool= 1.5f;
    Tile playerTile;
    float bossAngerExp = 0;
    int bossAngerLevel = 0;
    public bool wrongEmail = false;
	// Use this for initialization
	void Awake () 
    {
        if (instance==null)
        {
            instance = this;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        MoveEyes();
        ManyFacedBoss();
        AddToAnger();
        CoolDown();
	}
    void CoolDown()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_EMAIL)
        {
            if (emailCool< emailMaxCool)
            {
                emailCool += Time.deltaTime;
            }
            else
            {
                bossAngerExp += 0.5f;
                emailCool = 0;
            }
        }
        else if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
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
                    bossAngerExp += 0.5f;
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
        if (bossAngerExp>=1)
        {
            if (bossAngerLevel < faceList.Length - 1)
            {
                bossAngerLevel++; 
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
            bossAngerExp += 0.5f;
        }
        else
        {
            if (bossAngerLevel>=faceList.Length-1)
            {
                bossAngerLevel--;
            }
        }
   }
    void SummonBoss()
  {
        if (bossAngerLevel>=faceList.Length-1)
        {
            //make the boss come and fuck you up
        }
  }
}
