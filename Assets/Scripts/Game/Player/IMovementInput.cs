using UnityEngine;

namespace Assets.Scripts.Game.Player
{
    public interface IMovementInput
    {
        Vector2 GetMovementDirection();
    }
}
