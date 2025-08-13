using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;


public class SaveLoadManager : MonoBehaviour
{
    public List<EggData> EggsData { get; } = new();
    public List<BrainrotData> BrainrotsData { get; } = new();



    public void AddEggData(EggData data) => EggsData.Add(data);
    public void AddBrainrotData(BrainrotData data) => BrainrotsData.Add(data);
    
    public void RemoveEgg(EggData eggData)
    {
        EggsData.Remove(eggData);
        SaveEggsData();
    }
    
    public void RemoveEggByUniqueId(string uniqueId)
    {
        var egg = EggsData.FirstOrDefault(e => e.UniqueId == uniqueId);
        if (egg != null)
        {
            EggsData.Remove(egg);
            SaveEggsData();
        }
    }

    public void RemoveBrainrotByUniqueId(string uniqueId)
    {
        var brainrot = BrainrotsData.FirstOrDefault(e => e.UniqueId == uniqueId);
        if (brainrot != null)
        {
            BrainrotsData.Remove(brainrot);
            SaveEggsData();
        }
    }

// Дубляж
    public void SaveEggsData()
    {
        YG2.saves.SaveInt("Eggs", EggsData.Count);

        for (int i = 0; i < EggsData.Count; i++)
        {
            YG2.saves.SaveInt("Egg_" + i + "_Id", EggsData[i].Id);
            YG2.saves.SaveString("Egg_" + i + "_UniqueId", EggsData[i].UniqueId);
            YG2.saves.SaveInt("Egg_" + i + "_Rarity", (int)EggsData[i].Rarity);
            YG2.saves.SaveInt("Egg_" + i + "_Variant", (int)EggsData[i].Variant);
            YG2.saves.SaveLong("Egg_" + i + "_RemainingTime", EggsData[i].RemainingTime);
            YG2.saves.SaveFloat("Egg_" + i + "_SpawnTimer", EggsData[i].SpawnTimer);
            YG2.saves.SaveInt("Egg_" + i + "_HasSpawned", EggsData[i].HasSpawned ? 1 : 0);
            YG2.saves.SaveInt("Egg_" + i + "_HasActiveTimer", EggsData[i].HasActiveTimer ? 1 : 0);
            YG2.saves.SaveFloat("Egg_" + i + "_PositionX", EggsData[i].Position.x);
            YG2.saves.SaveFloat("Egg_" + i + "_PositionY", EggsData[i].Position.y);
            YG2.saves.SaveFloat("Egg_" + i + "_PositionZ", EggsData[i].Position.z);
        }
        YG2.SaveProgress();
    }
    public void SaveBrainrotsData()
    {
        YG2.saves.SaveInt("Brainrots", BrainrotsData.Count);

        for (int i = 0; i < BrainrotsData.Count; i++)
        {
            YG2.saves.SaveInt("Brainrot_" + i + "_Id", BrainrotsData[i].Id);
            YG2.saves.SaveString("Brainrot_" + i + "_UniqueId", BrainrotsData[i].UniqueId);
            YG2.saves.SaveInt("Brainrot_" + i + "_Rarity", (int)BrainrotsData[i].Rarity);
            YG2.saves.SaveInt("Brainrot_" + i + "_Variant", (int)BrainrotsData[i].Variant);
            YG2.saves.SaveInt("Brainrot_" + i + "_Income", BrainrotsData[i].Income);
            YG2.saves.SaveFloat("Brainrot_" + i + "_Size", BrainrotsData[i].Size);
            YG2.saves.SaveInt("Brainrot_" + i + "_HasSpawned", BrainrotsData[i].HasSpawned ? 1: 0);
            YG2.saves.SaveFloat("Brainrot_" + i + "_PositionX", BrainrotsData[i].Position.x);
            YG2.saves.SaveFloat("Brainrot_" + i + "_PositionY", BrainrotsData[i].Position.y);
            YG2.saves.SaveFloat("Brainrot_" + i + "_PositionZ", BrainrotsData[i].Position.z);
            Debug.Log(BrainrotsData[i].HasSpawned);

        }
        YG2.SaveProgress();
    }
    public void LoadEggsData()
    {
        int count = YG2.saves.LoadInt("Eggs");
        for (int i = 0; i < count; i++)
        {
            EggData data = new();
            data.Id = YG2.saves.LoadInt("Egg_" + i + "_Id");
            data.UniqueId = YG2.saves.LoadString("Egg_" + i + "_UniqueId");
            data.Rarity = (Rarity)YG2.saves.LoadInt("Egg_" + i + "_Rarity");
            data.Variant = (Variant)YG2.saves.LoadInt("Egg_" + i + "_Variant");
            data.HasSpawned = YG2.saves.LoadInt("Egg_" + i + "_HasSpawned") == 1;
            data.RemainingTime = YG2.saves.LoadLong("Egg_" + i + "_RemainingTime");
            data.SpawnTimer = YG2.saves.LoadFloat("Egg_" + i + "_SpawnTimer");
            data.HasActiveTimer = YG2.saves.LoadInt("Egg_" + i + "_HasActiveTimer") == 1;
            data.Position = new Vector3(
                YG2.saves.LoadFloat("Egg_" + i + "_PositionX"),
                YG2.saves.LoadFloat("Egg_" + i + "_PositionY"),
                YG2.saves.LoadFloat("Egg_" + i + "_PositionZ"));
            EggsData.Add(data);
        }
    }

    public void LoadBrainrotsData()
    {
        int count = YG2.saves.LoadInt("Brainrots");
        for (int i = 0; i < count; i++)
        {
            BrainrotData data = new();
            data.Id = YG2.saves.LoadInt("Brainrot_" + i + "_Id");
            data.UniqueId = YG2.saves.LoadString("Brainrot_" + i + "_UniqueId");
            data.Rarity = (Rarity)YG2.saves.LoadInt("Brainrot_" + i + "_Rarity");
            data.Variant = (Variant)YG2.saves.LoadInt("Brainrot_" + i + "_Variant");
            data.Income = YG2.saves.LoadInt("Brainrot_" + i + "_Income");
            data.HasSpawned = YG2.saves.LoadInt("Brainrot_" + i + "_HasSpawned") == 1;
            data.Size = YG2.saves.LoadFloat("Brainrot_" + i + "_Size");
            data.Position = new Vector3(
                YG2.saves.LoadFloat("Brainrot_" + i + "_PositionX"),
                YG2.saves.LoadFloat("Brainrot_" + i + "_PositionY"),
                YG2.saves.LoadFloat("Brainrot_" + i + "_PositionZ"));
            Debug.Log(data.HasSpawned);
            BrainrotsData.Add(data);
        }
    }
    
    public EggData FindEggData(string uniqueId)
    {
        return EggsData.FirstOrDefault(egg => 
            egg.UniqueId == uniqueId || 
            egg.UniqueId == uniqueId.Replace("Egg_", ""));
    }
}
