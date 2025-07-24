using Interfaces;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerSettings : ICharacterData
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float rotationSpeed = 720f;

        // Реализация ICharacterData
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float JumpForce => jumpForce;
        public float RotationSpeed => rotationSpeed;
    }
}