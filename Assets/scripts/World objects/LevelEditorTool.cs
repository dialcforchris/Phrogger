using UnityEngine;
using System.Collections;

public enum EditorToolBehavior
{
    SPAWN,
    TILE_SPRITE,
}

public class LevelEditorTool : MonoBehaviour
{
    [SerializeField] private bool active = false;
    [SerializeField] private Camera cam = null;

    [SerializeField] private WorldObject spawn = null;
    [SerializeField] private GameObject spawnParent = null;
    [SerializeField] private string spawnName = string.Empty;

    [SerializeField] private Sprite tileSprite = null;

    [SerializeField] private EditorToolBehavior toolBehavior = EditorToolBehavior.SPAWN;


    private void Awake()
    {
        
    }

    private void Update ()
    {
        if (active)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                cam.transform.position += Vector3.up;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                cam.transform.position += Vector3.left;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                cam.transform.position += Vector3.down;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                cam.transform.position += Vector3.right;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                transform.position += Vector3.up;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                transform.position += Vector3.left;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                transform.position += Vector3.down;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                transform.position += Vector3.right;
            }

            switch (toolBehavior)
            {
                case EditorToolBehavior.SPAWN:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        WorldObject _obj = (WorldObject)Instantiate(spawn, transform.position, Quaternion.identity);
                        Tile _tile = TileManager.instance.GetTile(transform.position);
                        _obj.name = spawnName + _tile.IndexName();
                        _obj.transform.SetParent(_tile.transform);
                        //_tile.Place(_obj);
                    }
                    break;
                }
                case EditorToolBehavior.TILE_SPRITE:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Tile _tile = TileManager.instance.GetTile(transform.position);
                            GameObject _obj = (GameObject)Instantiate(spawnParent, transform.position, Quaternion.identity);
                            _obj.name = spawnName + _tile.IndexName();
                            _obj.transform.SetParent(_tile.transform);
                            //_tile.UpdateSprite(tileSprite);
                    }
                    break;
                }
            }
        }
    }
}
