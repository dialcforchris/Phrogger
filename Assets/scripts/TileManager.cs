using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    [SerializeField] private bool generateGrid = false;
    [SerializeField] private Tile tile = null;
    [SerializeField] private Tile[] tiles;
    public static TileManager instance = null;
    [SerializeField] private int gridSizeX = 0, gridSizeY = 0;
    [SerializeField] private float size = 1;

    [SerializeField] private GameObject parentObject = null;

    public Cubicle c;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (generateGrid)
        {
            CreateGrid();
        }
    }
  
    private void CreateGrid()
    {
        tiles = new Tile[gridSizeX * gridSizeY];
        GameObject _objSource = Instantiate(parentObject);
        _objSource.name = "Tiles";
        for (int i=0;i<gridSizeX;i++)
        {
            GameObject _obj = Instantiate(parentObject);
            _obj.transform.SetParent(_objSource.transform);
            _obj.name = "Column" + i.ToString();
            float _x = ((i + (size / 2.0f)) - (gridSizeX / 2.0f)) * size;

            for (int j=0;j<gridSizeY;j++)
            {
                float _y = ((j+(size/2.0f)) - (gridSizeY / 2.0f)) * size;
                tiles[(j * gridSizeX) + i] = (Tile)Instantiate(tile, new Vector3(_x,_y,0), Quaternion.identity);
                tiles[(j * gridSizeX) + i].transform.SetParent(_obj.transform);
                tiles[(j * gridSizeX) + i].name = "Row" + j.ToString();
                tiles[(j * gridSizeX) + i].Initialise(i, j);
            }
        }
    }

    public void CreateTileReference(int _x, int _y, Tile _tile)
    {
        if (!generateGrid)
        {
            tiles[(_y * gridSizeX) + _x] = _tile;
        }
    }

    public Tile GetTile(Vector2 _pos)
    {
        _pos.x = Mathf.Floor(_pos.x) + 0.5f;
        _pos.y = Mathf.Round(_pos.y);// + 0.5f;

        int _x = (int)(((_pos.x / size) + (gridSizeX / 2.0f)) - (size / 2.0f));
        int _y = (int)(((_pos.y / size) + (gridSizeY / 2.0f)) - (size / 2.0f));
        return tiles[(_y * gridSizeX) + _x];
    }
}

