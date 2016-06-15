using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ParticleData
{
    public string particleName;
    public ParticleEffect particlePrefab;
}

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager singleton = null;
    public static ParticleManager instance { get { return singleton; } }

    [SerializeField] private ParticleData[] particleData = null;

    private Dictionary<string, ObjectPool<ParticleEffect>> particleDictionary = new Dictionary<string, ObjectPool<ParticleEffect>>();

    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(this);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            for(int i = 0; i < particleData.Length; ++i)
            {
                particleDictionary.Add(particleData[i].particleName, new ObjectPool<ParticleEffect>(particleData[i].particlePrefab, 5));
            }
        }
    }

    public ParticleEffect GetParticle(string _name)
    {
        ObjectPool<ParticleEffect> pool;
        particleDictionary.TryGetValue(_name, out pool);
        if(pool != null)
        {
            return pool.GetPooledObject();
        }
        return null;
    } 
}
