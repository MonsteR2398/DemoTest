using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager<T> where T : MonoBehaviour, IPoolable<T>
{
    private readonly IObjectPool<T> _pool;

    public ObjectPoolManager(T prefab, int defaultCapacity = 10, int maxSize = 100)
    {
        _pool = new ObjectPool<T>(
            createFunc: () =>
            {
                if (prefab == null)
                {
                    Debug.LogError("Attempted to instantiate null prefab");
                    return null;
                }
                
                var obj = Object.Instantiate(prefab);
                obj.SetPoolOwner(_pool);
                return obj;
            },
            actionOnGet: obj =>
            {
                obj.gameObject.SetActive(true);
                obj.OnGetFromPool();
            },
            actionOnRelease: obj =>
            {
                obj.OnReturnToPool();
                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: Object.Destroy,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public T Get() => _pool.Get();

    public void ReturnToPool(T obj) => _pool.Release(obj);

    public void Clear() => _pool.Clear();
}
