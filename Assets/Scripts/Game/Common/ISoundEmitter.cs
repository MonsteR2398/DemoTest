using UnityEngine;

public interface ISoundEmitter
{
    AudioClip Clip { get; }
    int SoundInterval { get; set; }
}