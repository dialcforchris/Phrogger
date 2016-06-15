using UnityEngine;

public class Email : MonoBehaviour, IPoolable<Email>
{
    #region IPoolable
    public PoolData<Email> poolData { get; set; }
    #endregion

    public void Initialise()
    {
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
