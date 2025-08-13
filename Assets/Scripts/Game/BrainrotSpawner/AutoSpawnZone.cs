using UnityEngine;
using System.Collections.Generic;

public class AutoSpawnZone : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private int itemsCount = 10;
    [SerializeField] private float _minDistanceMultiplier = 1.5f;
    [SerializeField] private int _attempts = 100;

    private Collider _spawnArea;

    private void Awake()
    {
        _spawnArea = GetComponent<Collider>();
        if (_spawnArea == null)
            Debug.LogError("Collider не найден!");
    }

    void Start()
    {
        SpawnItems();
    }

public void SpawnItems()
{
    List<(Vector3 position, float radius)> spawnedItems = new List<(Vector3, float)>();
    var itemSpawner = GetComponent<BrainrotSpawner>();

        for (int i = 0; i < itemsCount; i++)
        {
            var brainrot = itemSpawner.Spawn(needSave: false);
            if (brainrot == null) continue;

            float itemRadius = brainrot.GetSize() * 0.5f;
            Vector3 randomPosition = GetRandomPosition(spawnedItems, itemRadius);

            if (randomPosition != Vector3.zero)
            {
                brainrot.transform.position = randomPosition;
                spawnedItems.Add((randomPosition, itemRadius));
                brainrot.OnSpawnToGround();
            }
            else
            {
                Destroy(brainrot.gameObject);
            }
    }
}

private Vector3 GetRandomPosition(List<(Vector3 position, float radius)> existingItems, float currentItemRadius)
{
    for (int i = 0; i < _attempts; i++)
    {
        Vector3 randomPoint = GetRandomPointInZone(currentItemRadius);
        if (randomPoint == Vector3.zero) continue;

        bool isValid = true;
        foreach (var existing in existingItems)
        {
            float requiredDistance = (currentItemRadius + existing.radius) * _minDistanceMultiplier;
            float requiredDistanceSquared = requiredDistance * requiredDistance;
            Vector3 difference = randomPoint - existing.position;
            float distanceSquared = Vector3.SqrMagnitude(difference);
            if (distanceSquared < requiredDistanceSquared)
            {
                isValid = false;
                break;
            }
        }
        if (isValid) return randomPoint;
    }
    return Vector3.zero;
}

private Vector3 GetRandomPointInZone(float objectRadius)
{
    Bounds bounds = _spawnArea.bounds;
    
    // Защита от слишком больших радиусов
    float safeRadius = Mathf.Min(objectRadius, bounds.size.x / 2, bounds.size.z / 2);
    
    float minX = bounds.min.x + safeRadius;
    float maxX = bounds.max.x - safeRadius;
    float minZ = bounds.min.z + safeRadius;
    float maxZ = bounds.max.z - safeRadius;

    if (minX > maxX || minZ > maxZ)
    {
        Debug.LogWarning($"Зона слишком мала для размещения объекта! Радиус: {safeRadius}, Размеры зоны: {bounds.size}");
        return Vector3.zero;
    }
    return new Vector3(
        Random.Range(minX, maxX), 
        bounds.max.y, 
        Random.Range(minZ, maxZ)
    );
}
}
