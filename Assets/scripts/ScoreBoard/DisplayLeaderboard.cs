using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayLeaderboard : MonoBehaviour 
{
    public Text[] scores;
    public Text[] names;
    public Text[] rank;
    bool once = false;
		
	// Update is called once per frame
	void Update () 
    {
	    if (!once)
        {
            List<KeyValuePair<string,int>> k = LeaderBoard.instance.ReturnLeaderBoard();
            for (int i=0;i<k.Count;i++)
            {
                rank[i].text = (i + 1).ToString();
                scores[i].text = k[i].Value.ToString();
                names[i].text = k[i].Key;
                rank[i].color = Color.Lerp(Color.green, Color.red, (float)(i * 0.05f));
                scores[i].color = Color.Lerp(Color.green, Color.red, (float)(i * 0.05f));
                names[i].color = Color.Lerp(Color.green, Color.red, (float)(i * 0.05f));
            }
        }
	}
}
