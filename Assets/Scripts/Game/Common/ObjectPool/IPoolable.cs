using UnityEngine.Pool;

public interface IPoolable<T> where T : class
{
    void OnGetFromPool();
    void OnReturnToPool();
    void SetPoolOwner(IObjectPool<T> pool);
}