using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
  
    public Transform player;
    public float nodeRadius;
    public Vector2 gridWorldSize;
    public bool displayGridGiz;
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }   
    }
    void CreateGrid()
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y/2;
        grid = new Node[gridSizeX, gridSizeY];
        for (int i=0;i<gridSizeX;i++)
        {
            for (int j=0;j<gridSizeY;j++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * 
                    (i * nodeDiameter + nodeRadius) + Vector3.up * (j * nodeDiameter + nodeRadius);
                grid[i, j] = new Node(worldPoint,i,j);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 _worldPos)
    {
        float percentX = (_worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (_worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
    public List<Node> GetNeighbours(Node _current)
    {
        List<Node> neighbours = new List<Node>();
        for (int i=-1;i<=1;i++)
        {
            for (int j=-1;j<=1;j++)
            {
                if (i==0&&j==0)
                {
                    continue;
                }
                int checkX = _current.gridX + i;
                int checkY = _current.gridY + j;
                if (checkX >=0&&checkX<gridSizeX && checkY>=0&&checkY<gridSizeY)
                {

                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        {
            if (grid != null && displayGridGiz)
            {

                foreach (Node n in grid)
                {
                   
                    Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}

