using UnityEngine;

public class ParticleEffect : MonoBehaviour, IPoolable<ParticleEffect>
{
    #region IPoolable
    public PoolData<ParticleEffect> poolData { get; set; }
    #endregion

    [SerializeField] protected ParticleSystem[] particles = null;

    [SerializeField] protected float lifetime = 1.0f;
    protected float currentLifetime = 0.0f;

    public virtual void Explode()
    {
        gameObject.SetActive(true);
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
    }

    protected virtual void Update()
    {
        currentLifetime = currentLifetime + Time.deltaTime < lifetime ? currentLifetime + Time.deltaTime : lifetime;
        if(currentLifetime == lifetime)
        {
            Reset();
        }
    }

    protected virtual void Reset()
    {
        poolData.ReturnPool(this);
        currentLifetime = 0.0f;
        gameObject.SetActive(false);
    }
}
