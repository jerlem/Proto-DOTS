using System;
using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringFlockingSettings : MonoBehaviour
    {
        public float CellRadius = 8.0f;
        public float SeparationWeight = 1.0f;
        public float AlignmentWeight = 1.0f;
        public float TargetWeight = 2.0f;
        public float ObstacleAversionDistance = 30.0f;
        public float MoveSpeed = 25.0f;

        class Boulanger : Baker<AuthoringFlockingSettings>
        {
            public override void Bake(AuthoringFlockingSettings authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace);
                AddSharedComponent(entity, new SharedComponentFlockingSettings
                {
                    CellRadius = authoring.CellRadius,
                    SeparationWeight = authoring.SeparationWeight,
                    AlignmentWeight = authoring.AlignmentWeight,
                    TargetWeight = authoring.TargetWeight,
                    DangerAversionDistance = authoring.ObstacleAversionDistance,
                    MoveSpeed = authoring.MoveSpeed
                });
            }
        }
    }

    [Serializable]
    public struct SharedComponentFlockingSettings : ISharedComponentData
    {
        public float CellRadius;
        public float SeparationWeight;
        public float AlignmentWeight;
        public float TargetWeight;
        public float DangerAversionDistance;
        public float MoveSpeed;
    }
}
