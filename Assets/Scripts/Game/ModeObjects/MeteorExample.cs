using UnityEngine;

public class MeteorExample : BaseObject
{
    // понадобился для инициализации в пуле
    // можно указать тут логику самого метеора или его части

    void OnCollisionEnter(Collision collision)
    {
        this.ReturnToPool();
    }
}
