using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Header("Клип")]
    public AudioClip clip;

    [Header("Это основной луп зоны? (главная мелодия)")]
    public bool isLoop;

    [Header("ШАГИ ПАТТЕРНА (повторяются каждый цикл зоны). Пример: 0, 2, 3.5")]
    public double[] oneShotBeats;

    [Header("ИНТЕРВАЛЫ (дополнительно/вместо): каждые N бит, со сдвигом")]
    public bool useIntervals;
    public double[] intervals;
    [Tooltip("Сдвиг старта для интервалов, в битах (можно дробные)")]
    public double intervalOffsetBeats = 0.0;
}
