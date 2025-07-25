using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IHasChance<T>
{
    T Value { get; }
    float Chance { get; }
}


[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Spawn System/Spawner Config")]
public class SpawnerConfig : ScriptableObject
{
    [System.Serializable]
    public class RarityDropChance : IHasChance<Rarity>
    {
        public Rarity rarity;
        [Range(0, 100)] public float chance;
        public Rarity Value => rarity;
        public float Chance => chance;
    }

    [System.Serializable]
    public class VariantDropChance : IHasChance<Variant>
    {
        public Variant variant;
        [Range(0, 100)] public float chance;
        public Variant Value => variant;
        public float Chance => chance;
    }

        [System.Serializable]
    public class EggsDropChance : IHasChance<Egg>
    {
        public Egg egg;
        [Range(0, 100)] public float chance;
        public Egg Value => egg;
        public float Chance => chance;
    }

    public List<RarityDropChance> rarityChances;
    public List<VariantDropChance> variantChances;
    public List<EggsDropChance> eggs;


    public Egg GetRandomEgg()
    {
        Egg eggPrefab = GetRandomByChance(eggs.Cast<IHasChance<Egg>>().ToList(), null);
        if (eggPrefab == null)
        {
            Debug.LogError("No egg prefabs available!");
            return null;
        }
        //eggPrefab.objectId = eggPrefab.Id.ToString() + eggPrefab.Rarity + eggPrefab.Variant;
        return eggPrefab;

    }
    
    public Rarity GetRandomRarity() =>
        GetRandomByChance(rarityChances.Cast<IHasChance<Rarity>>().ToList(), Rarity.Common);
    public Variant GetRandomVariant() =>
        GetRandomByChance(variantChances.Cast<IHasChance<Variant>>().ToList(), Variant.Default);

    private T GetRandomByChance<T>(List<IHasChance<T>> chances, T defaultValue = default)
    {
        if (chances == null || chances.Count == 0)
            return defaultValue;

        float totalChance = Enumerable.Sum(chances, entry => entry.Chance);
        float randomValue = Random.Range(0, totalChance);
        float currentChance = 0;

        foreach (var entry in chances)
        {
            currentChance += entry.Chance;
            if (randomValue <= currentChance)
                return entry.Value;
        }
        return defaultValue;
    }   
}