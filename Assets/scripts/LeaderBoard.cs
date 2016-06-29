﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour 
{
   //ask for player name
    //assign on UI
    //great job!
    private static LeaderBoard leader = null;
    public static LeaderBoard instance
    {
        get { return leader; }
    }
    bool once = false;
    public string playerName;
    public string gameName;
    int playerScore;
    public int stringLength = 3;
    List<KeyValuePair<string, int>> scores = new List<KeyValuePair<string,int>>();

	// Use this for initialization
	void Awake () 
    {
	    if (leader==null)
        {
            leader = this;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!once)
        {
            if (CheckScoreFile())
            {
                ReadScoreFile();
            }
            else
            {
                CreateScoreFile();
            }
            once = true;
        }
       
	}

    /// <summary>
    /// adds new score to list
    /// </summary>
    /// <param name="_score">current players sccore</param>
    /// <param name="_name">current players name</param>
    void AddToList(int _score, string _name)
    {
        KeyValuePair<string, int> newPlayer = new KeyValuePair<string, int>(_name, _score);
        scores.Add(newPlayer);
    }

    /// <summary>
    /// Custom comparison. Sorts by value
    /// </summary>
    static int Compare(KeyValuePair<string, int>_a,KeyValuePair<string,int>_b)
    {
        return _a.Value.CompareTo(_b.Value);
    }
   
    /// <summary>
    /// Sorts the scores in descending order
    /// </summary>
    void SortScores()
    {
        for (int j = 0; j < scores.Count; j++)
        {
            for (int i = 0; i < scores.Count - 1; i++)
            {
                if (scores[i].Value < scores[i + 1].Value)
                {
                    KeyValuePair<string, int> temp = scores[i];
                    scores[i] = scores[i + 1];
                    scores[i + 1] = temp;
                }
            }
        }
    }

    /// <summary>
    /// keeps the list down to 10
    /// ONLY USE AFTER SORTSCORES() HAS BEEN CALLED
    /// </summary>
    void TrimList()
    {
        for (int i = scores.Count - 1; i > 9; i--)
        {
            scores.RemoveAt(i);
        }
        scores.TrimExcess();
    }

    /// <summary>
    /// Creates a Score file if there isn't one
    /// </summary>
    void CreateScoreFile()
    {
        StreamWriter makeFile = File.CreateText(gameName + "Scores.dat");
        foreach(KeyValuePair<string,int> k in scores)
        {
            makeFile.WriteLine(k.Key + " " + k.Value);
        }
    } 

    void WriteToFile()
    {
        StreamWriter write = new StreamWriter(gameName + "Scores.dat", false);
        foreach (KeyValuePair<string, int> k in scores)
        {
            write.WriteLine(k.Key + " " + k.Value);
        }
        write.Close();
    }

    /// <summary>
    /// read in the high Score file
    /// </summary>
    void ReadScoreFile()
    {
        List<string> fileInput = new List<string>();
     //   int index = 0;
        scores.Clear();
        StreamReader highScores = new StreamReader(gameName + "Scores.dat");
        while (!highScores.EndOfStream)
        {
            fileInput.Add(highScores.ReadLine());
        //    index++;
        }
        for (int i = 0; i < fileInput.Count;i++ )
        {
           string entry = fileInput[i].Substring(0,stringLength);
           int _score = int.Parse(fileInput[i].Substring(stringLength+1));
           AddToList(_score, entry);
        }
        highScores.Close();
        AddToList(playerScore, playerName);
        SortScores();
        TrimList();
        WriteToFile();
       // scores.Clear();
        //for (int i = 0; i < scores.Count; i++)
        //{
        //    Debug.Log(scores[i].Key + " " + scores[i].Value);
        //}
    }

    /// <summary>
    /// Check a high score file exists
    /// </summary>
    bool CheckScoreFile()
    {
        if (File.Exists(gameName + "Scores.dat"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetScore(int _score)
    {
        playerScore = _score;
    }

    public bool CheckIfHighScore(int _score)
    {
        if (scores[10].Value < _score)
        {
            playerScore = _score;
            return true;
            //do something with asking for name
        }
        else
            return false;
    }
}
