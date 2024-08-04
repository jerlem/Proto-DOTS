using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FPSTemplate
{
    /// <summary>
    /// Scriptable for Player Control
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptableAnimation", menuName = "ScriptableObjects/PlayerCharacterSettings")]
    public class ScriptablePlayerCharacterSettings : ScriptableObject
    {
        [SerializeField]
        public float ForwardSpeed = 1f;
        public float SideSpeed = 1f;
        public float BackSpeed = 1f;

        public float MaxSpeed = 10f;
        public float Acceleration = 1f;
        public float Deceleration = 1f;
        
        public float DampSpeed = 1f;
        public float RotationSpeed = 1f;

        public float RotationMaxY = 90f;
        public float RotationMinY = -90f;

        public float JumpForce = 10f;

    }

}