using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour 
{
    public Text score;
    public Text lives;
    public Player player;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        score.text = "Score: " +player.Score;
        lives.text = "Strikes Remaining: " + player.Strikes;
	}
}
