using UnityEngine;
using System.Collections;

public class Node 
{
    public bool walkable;
    public int gCost;
    public int hCost;
    public int gridX;
    public int gridY;
    public Vector2 worldPos;
    public Node parent;
    int heapIndex;


    public Node(bool _walkable, Vector2 _worldPos,int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value;}
    }
    public int CompareTo(Node n)
    {
        int compare = fCost.CompareTo(n.fCost);
        if (compare==0)
        {
            compare = hCost.CompareTo(n.hCost);
        }
        return -compare;
    }
}
