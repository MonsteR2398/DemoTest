using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

[RequireComponent(typeof(SaveLoadManager))]
public class LoadInitializer : MonoBehaviour
{
    public static LoadInitializer Instance;
    [SerializeField] private List<Egg> eggPrefabs;
    [SerializeField] private List<Brainrot> brainrotPrefabs;

    public SaveLoadManager SaveLoadManager { get; private set; }

    void Awake()
    {
        YG2.saves.InitializeCache();
        Instance = this;
        SaveLoadManager = GetComponent<SaveLoadManager>();
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        InitEggs();
        InitBrainrots();
    }


    void InitEggs()
    {
        SaveLoadManager.LoadEggsData();
        var eggsData = SaveLoadManager.EggsData;
        for (int i = 0; i < eggsData.Count; i++)
        {


            Vector3 position = eggsData[i].Position;
            Egg prefab = eggPrefabs.FirstOrDefault(egg => egg.Data.Id == eggsData[i].Id);
            if (prefab == null)
            {
                Debug.LogError($"Загрузка не удалась, префаб яйца не найден в списке префабов!");
                continue;
            }
            // заменить на пул
            Egg egg = Instantiate(prefab, position, Quaternion.identity);
            egg.Data.Rarity = eggsData[i].Rarity;
            egg.Data.Variant = eggsData[i].Variant;
            egg.Data.HasActiveTimer = eggsData[i].HasActiveTimer;
            egg.Data.Position = position;
            egg.Data.RemainingTime = eggsData[i].RemainingTime;
            egg.Data.SpawnTimer = eggsData[i].SpawnTimer;
            if (!eggsData[i].HasSpawned)
            {
                InventorySystem.Instance.AddItem(egg, 1, false);
                egg.gameObject.SetActive(false);
                continue;
            }
            if (egg is ITimer timer)
                timer.ActivateTimer();
            egg.OnSpawnToGround();
        }
    }

    void InitBrainrots()
    {
        SaveLoadManager.LoadBrainrotsData();
        var brainrotsData = SaveLoadManager.BrainrotsData;
        for (int i = 0; i < brainrotsData.Count; i++)
        {
        Debug.Log(brainrotsData[i].Position);
            Vector3 position = brainrotsData[i].Position;
            Brainrot prefab = brainrotPrefabs.FirstOrDefault(brainrot => brainrot.Data.Id == brainrotsData[i].Id);
            if (prefab == null)
            {
                Debug.LogError($"Загрузка не удалась, префаб бреинрота не найден в списке префабов!");
                continue;
            }
            // заменить на пул
            Brainrot brainrot = Instantiate(prefab, position, Quaternion.identity);
            brainrot.Data.Rarity = brainrotsData[i].Rarity;
            brainrot.Data.Variant = brainrotsData[i].Variant;
            brainrot.Data.Income = brainrotsData[i].Income;
            brainrot.Data.Size = brainrotsData[i].Size;
            brainrot.Data.Position = position;

            brainrot.SetSize(brainrot.Data.Size);

            if (!brainrotsData[i].HasSpawned)
            {
                InventorySystem.Instance.AddItem(brainrot, 1, false);
                brainrot.gameObject.SetActive(false);
                continue;
            }

            brainrot.OnSpawnToGround();
        }
    }
}
