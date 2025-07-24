using UnityEngine;

namespace Interfaces
{
    //Управление физикой движения
    public interface IMovementController
    {
        void Move(Vector3 direction);
        void SetMoveSpeed(float speed);
        void ApplyGravity(float multiplier);
        bool IsGrounded { get; }
    }

//Управление вращением
    public interface IRotationController
    {
        void Rotate(Quaternion rotation);
    }
    
    //Анимации
    public interface IAnimationController
    {
        void SetBool(string param, bool value);
        void SetTrigger(string param);
        void SetFloat(string param, float value);
        float GetFloat(string param);
    }

//Получение ввода
    public interface IInputProvider
    {
        Vector3 GetMoveInputRaw();
        bool JumpPressed { get; }
        bool RunPressed { get; }
    }

    public interface ICalculator
    {
        Vector3 CalculateMoveDirection(Vector3 direction, float speedMultiplier = 1f);
        Quaternion CalculateRotateDirection(Vector3 direction, float speedMultiplier = 1f);
    }

//Доступ к данным персонажа
    public interface ICharacterData
    {
        float WalkSpeed { get; }
        float RunSpeed { get; }
        float RotationSpeed { get; }
        float JumpForce { get; }
    }
}