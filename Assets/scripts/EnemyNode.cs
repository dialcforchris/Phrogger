using UnityEngine;
using System.Collections;

public enum DirectionState
{
    UP = 0,
    DOWN,
    LEFT,
    RIGHT,
    DIRECTION_COUNT
}

[System.Serializable]
public struct DirectionChance
{
    public DirectionState state;
    [Range(0.0f, 100.0f)]public float chance;
}

public class EnemyNode : MonoBehaviour
{
    //[SerializeField] private DirectionChance[] directionChances = null;

    public DirectionState NewDirection()
    {
    //    float _rand = Random.Range(0.0f, 100.0f);
    //    float _value = 0.0f;
    //    for(int i = 0; i < directionChances.Length; ++i)
    //    {
    //        _value += directionChances[i].chance;
    //        if (_value > _rand)
    //        {
    //            return directionChances[i].state;
    //        }
    //    }
        return DirectionState.DIRECTION_COUNT;
    }
}
