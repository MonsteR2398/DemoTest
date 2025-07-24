using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolableMono<T> : MonoBehaviour, IPoolable<T> where T : MonoBehaviour
{
    private IObjectPool<T> _pool;

    public void SetPoolOwner(IObjectPool<T> pool)
    {
        _pool = pool;
    }

    public virtual void OnGetFromPool() { }
    public virtual void OnReturnToPool() { }
    public void ReturnToPool() => _pool?.Release(this as T);
    public void ReturnAfterSeconds(float seconds) => StartCoroutine(ReturnDelayed(seconds));

    private System.Collections.IEnumerator ReturnDelayed(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReturnToPool();
    }
}