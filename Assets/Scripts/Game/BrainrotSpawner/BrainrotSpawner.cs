using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BrainrotSpawner : MonoBehaviour
{
    public AnimationCurve baseSizeCurve;
    // public List<BrainrotDropChance> brainrots;
    [SerializeField] private SpawnerConfig spawnerConfig;
    [SerializeField] private bool _nowEnemy;
    [Range(1, 10)] public float chanceMultiplier = 1f;
    private static readonly float minSize = 2f;
    private static readonly float maxSize = 8f;

    [System.Serializable]
    public class BrainrotDropChance : IHasChance<Brainrot>
    {
        public Brainrot brainrot;
        [Range(0, 100)] public float chance;
        public Brainrot Value => brainrot;
        public float Chance => chance;
    }

    private float GetRandomSize()
    {
        float random = Random.value;
        random = Mathf.Pow(random, 1f / chanceMultiplier);
        float sizeFactor = baseSizeCurve.Evaluate(random);
        return Mathf.Lerp(minSize, maxSize, sizeFactor);
    }

    public Brainrot Spawn(Variant variant = Variant.Default, bool needSave = true)
    {
        Brainrot brainrot = Instantiate((Brainrot)spawnerConfig.GetRandomItem(), transform.position, Quaternion.identity);
        if (_nowEnemy) brainrot.NowEnemy = _nowEnemy;
        brainrot.Data.Rarity = spawnerConfig.GetRandomRarity();
        if(variant != Variant.Default)
            brainrot.Data.Variant = variant;
        brainrot.OnSpawnToGround();
        float size = GetRandomSize();
        brainrot.SetSize(size);
        if (needSave)
        {
            LoadInitializer.Instance.SaveLoadManager.AddBrainrotData(brainrot.Data);
            LoadInitializer.Instance.SaveLoadManager.SaveBrainrotsData();
        }
        return brainrot;
    }
}
