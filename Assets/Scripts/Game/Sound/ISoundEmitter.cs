using UnityEngine;

public interface ISoundEmitter
{
    AudioClip Clip { get; }
    float NextPlayTime { get; set;}
    float Interval { get; set; }
    float phaseOffset { get; set; }
}