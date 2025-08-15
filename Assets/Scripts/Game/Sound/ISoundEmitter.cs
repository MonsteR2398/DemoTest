using UnityEngine;

public interface ISoundEmitter
{
    public AudioClip clip { get; }
    public double[] beats { get; } // Например {1, 3.5, 5, 10}
    public bool isLoop { get; } // Основной бит
}