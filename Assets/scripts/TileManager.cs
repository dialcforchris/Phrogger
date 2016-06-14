using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{

    public Tile tile;
    //public float nodeRadius;
    //public Vector2 gridWorldSize;
    //public bool displayGridGiz;
    Tile[,] tiles;
    //float nodeDiameter;
    [SerializeField] private int gridSizeX = 0, gridSizeY = 0;
    [SerializeField] private float size = 1;

    private void Awake()
    {
        //nodeDiameter = nodeRadius * 2;
        //gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        //gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    //public int MaxSize
    //{
    //    get { return gridSizeX * gridSizeY; }   
    //}
    private void CreateGrid()
    {
        //Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y/2;
        tiles = new Tile[gridSizeX, gridSizeY];
        for (int i=0;i<gridSizeX;i++)
        {
            for (int j=0;j<gridSizeY;j++)
            {
                float _x = ((i+(size/2.0f)) - (gridSizeX / 2.0f)) * size;
                float _y = ((j+(size/2.0f)) - (gridSizeY / 2.0f)) * size;
                //Vector3 worldPoint = worldBottomLeft + Vector3.right * 
                //    (i * nodeDiameter + nodeRadius) + Vector3.up * (j * nodeDiameter + nodeRadius);
                tiles[i, j] = (Tile)Instantiate(tile, new Vector3(_x,_y,0), Quaternion.identity);
                tiles[i, j].Initialise((j * gridSizeY) + i);
            }
        }
    }

    //public Tile GetTile(Vector3 _pos)
    //{
    //    return tiles[_pos.x, _pos.y];
    //}

    //public Node NodeFromWorldPoint(Vector2 _worldPos)
    //{
    //    float percentX = (_worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
    //    float percentY = (_worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
    //    percentX = Mathf.Clamp01(percentX);
    //    percentY = Mathf.Clamp01(percentY);
    //    int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
    //    int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
    //    return grid[x, y];
    //}
    //public List<Node> GetNeighbours(Node _current)
    //{
    //    List<Node> neighbours = new List<Node>();
    //    for (int i=-1;i<=1;i++)
    //    {
    //        for (int j=-1;j<=1;j++)
    //        {
    //            if (i==0&&j==0)
    //            {
    //                continue;
    //            }
    //            int checkX = _current.gridX + i;
    //            int checkY = _current.gridY + j;
    //            if (checkX >=0&&checkX<gridSizeX && checkY>=0&&checkY<gridSizeY)
    //            {

    //                neighbours.Add(grid[checkX, checkY]);
    //            }
    //        }
    //    }
    //    return neighbours;
    //}
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
    //    {
    //        if (grid != null && displayGridGiz)
    //        {

    //            foreach (Node n in grid)
    //            {
                   
    //                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));
    //            }
    //        }
    //    }
    //}
}

