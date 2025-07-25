using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IntervalSpawner : UpgradableSpawner
{
    [SerializeField] private float spawnRate;
    private Coroutine spawnRoutine;
    private void Start() => StartSpawning();

    private void StartSpawning()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine(spawnRate));
    }

    private IEnumerator SpawnRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            SpawnToPoint();
        }
    }
    

}
