using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    public static WorldUIManager Instance;
    
    [Header("Настройки")]
    public Transform player;
    public float checkDistance = 5f;
    public Canvas worldCanvas;

    [Header("Префабы UI")]
    public GameObject infoDisplayPrefab;
    public GameObject progressDisplayPrefab;
    public GameObject buyDisplayPrefab;

    private List<WorldObjectUI> allObjects = new();
    private Dictionary<WorldObjectUI, GameObject> activeDisplays = new();

    void Awake() => Instance = this;

    void Update()
    {
        UpdateVisibleObjects();
    }

    public void RegisterObject(WorldObjectUI obj)
    {
        if (!allObjects.Contains(obj))
            allObjects.Add(obj);
    }

    public void UnregisterObject(WorldObjectUI obj)
    {
        allObjects.Remove(obj);
        if (activeDisplays.TryGetValue(obj, out var display))
        {
            Destroy(display);
            activeDisplays.Remove(obj);
        }
    }

    void UpdateVisibleObjects()
    {
        var toRemove = new List<WorldObjectUI>();
        
        foreach (var kvp in activeDisplays)
        {
            if (!ShouldShow(kvp.Key))
            {
                Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var obj in toRemove)
            activeDisplays.Remove(obj);

        foreach (var obj in allObjects)
        {
            if (ShouldShow(obj))
                UpdateOrCreateDisplay(obj);
        }
    }

    bool ShouldShow(WorldObjectUI obj)
    {
        if (obj.alwaysVisible) return true;
        float dist = Vector3.Distance(player.position, obj.transform.position);
        return dist <= checkDistance;
    }

    void UpdateOrCreateDisplay(WorldObjectUI obj)
    {
        if (!activeDisplays.ContainsKey(obj))
        {
            GameObject prefab = GetPrefabForObject(obj);
            if (prefab == null) return;

            var display = Instantiate(prefab, worldCanvas.transform);
            activeDisplays.Add(obj, display);
            display.GetComponent<UIDisplayController>().Initialize(obj);
        }

        var displayObj = activeDisplays[obj];
        displayObj.transform.position = obj.GetUIPosition();
        displayObj.transform.LookAt(player);
        displayObj.transform.Rotate(0, 180, 0);
    }

    GameObject GetPrefabForObject(WorldObjectUI obj)
    {
        if (obj.showBuy && obj.GetComponent<IBuyable>() != null)
            return buyDisplayPrefab;
        if (obj.showProgress && obj.GetComponent<IProgressable>() != null)
            return progressDisplayPrefab;
        return infoDisplayPrefab;
    }
}