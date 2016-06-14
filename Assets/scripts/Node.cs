using UnityEngine;
using System.Collections;

public class Node 
{
  
    public int gridX;
    public int gridY;
    public Vector2 worldPos;
    public Node parent;
  

    public Node( Vector2 _worldPos,int _gridX, int _gridY)
    {
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
  
  
}
