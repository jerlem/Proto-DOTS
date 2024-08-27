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

        [Header("Movement Settings")]
        public float MaxSpeed = 10f;
        public float Acceleration = 600f;
        public float SprintMultiplier = 2f;
        public float DampingCoefficient = 1f;

        public float JumpForce = 10f;

        [Header("Mouse Settings")]
        public float VerticalSensivity = 3f;
        public float HorizontalSensivity = 3f;
        public float RotationMaxY = 50f;
        public float RotationMinY = -50f;
    }

}