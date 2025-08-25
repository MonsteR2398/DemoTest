using TMPro;
using UnityEngine;
using System.Collections;
using System;

public class BillboardUIEvents : MonoBehaviour, IObjectTextDisplay
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private int timeToSpawnLegendary = 60;
    [SerializeField] private int timeToSpawnMythic = 120;
    [SerializeField] private Vector3 _displayOffset = Vector3.zero;
    
    private int currentTimeToSpawnLegendary;
    private int currentTimeToSpawnMythic;
    

    private Coroutine timerCoroutine;

    public Vector3 DisplayOffset => _displayOffset;

    private void OnEnable()
    {
        TextDisplayEvents.RaiseDisplayEnabled(this);
        InitializeTimers();
        StartTimer();
    }

    private void OnDisable()
    {
        TextDisplayEvents.RaiseDisplayDisabled(this);
        StopTimer();
    }

    private void InitializeTimers()
    {
        currentTimeToSpawnLegendary = timeToSpawnLegendary;
        currentTimeToSpawnMythic = timeToSpawnMythic;
    }

    public Vector3 GetPosition() => transform.position;

    private void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(UpdateTimer());
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private IEnumerator UpdateTimer()
    {
        var wait = new WaitForSeconds(1);
        
        while (true)
        {
            yield return wait;
            UpdateTimers();
        }
    }

    private void UpdateTimers()
    {
        UpdateLegendaryTimer();
        UpdateMythicTimer();
        
        UpdateUIText();
    }

    private void UpdateLegendaryTimer()
    {
        currentTimeToSpawnLegendary--;
        
        if (currentTimeToSpawnLegendary <= 0)
        {
            currentTimeToSpawnLegendary = timeToSpawnLegendary;
            TextDisplayEvents.RaiseLegendaryEggCanBeSpawned(true);
        }
    }

    private void UpdateMythicTimer()
    {
        currentTimeToSpawnMythic--;
        
        if (currentTimeToSpawnMythic <= 0)
        {
            currentTimeToSpawnMythic = timeToSpawnMythic;
            TextDisplayEvents.RaiseMythicEggCanBeSpawned(true);
        }
    }

    private void UpdateUIText()
    {
        if (targetText != null)
            targetText.text = $"Legendary egg: {currentTimeToSpawnLegendary}s\nMythic egg: {currentTimeToSpawnMythic}s";
    }

    public string GetTextOnDisplay() => targetText.text;
    public bool IsAlwaysVisible() => true;
    public bool ShouldOrientToCamera() => false;
}
