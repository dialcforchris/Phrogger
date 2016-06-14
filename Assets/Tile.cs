using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private int index;
    private List<WorldObject> objects = new List<WorldObject>();

    public void Initialise(int _index)
    {
        index = _index;
    }

    public void Place(WorldObject _obj)
    {
        objects.Add(_obj);
    }
    public void Remove(WorldObject _obj)
    {
        for(int i = 0; i < objects.Count; ++i)
        {
            if(objects[i] == _obj)
            {
                objects.RemoveAt(i);
            }
        }
    }



}
