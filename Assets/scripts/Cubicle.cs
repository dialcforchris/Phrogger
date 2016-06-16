using UnityEngine;
using System.Collections;

public class Cubicle : WorldObject 
{
    public SpriteRenderer deskFodder;
    public Sprite[] tidyDesk;
    public Sprite[] messyDesk;
    int currentDesk;
	// Use this for initialization
	void Awake () 
    {
        currentDesk = Random.Range(0, tidyDesk.Length);
        deskFodder.sprite = tidyDesk[currentDesk];
    }
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
