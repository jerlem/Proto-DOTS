using System;
using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringHerdingSettings : MonoBehaviour
    {
        public float CellRadius = 8.0f;
        public float SeparationWeight = 1.0f;
        public float AlignmentWeight = 1.0f;
        public float TargetWeight = 2.0f;
        public float ObstacleAversionDistance = 30.0f;
        public float MoveSpeed = 25.0f;

        class Boulanger : Baker<AuthoringHerdingSettings>
        {
            public override void Bake(AuthoringHerdingSettings authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace);
                AddSharedComponent(entity, new SharedComponentHerdingSettings
                {
                    CellRadius = authoring.CellRadius,
                    SeparationWeight = authoring.SeparationWeight,
                    TargetWeight = authoring.TargetWeight,
                    DangerAversionDistance = authoring.ObstacleAversionDistance,
                    MoveSpeed = authoring.MoveSpeed
                });
            }
        }
    }

    [Serializable]
    public struct SharedComponentHerdingSettings : ISharedComponentData
    {
        public float CellRadius;
        public float SeparationWeight;
        public float AlignmentWeight;
        public float TargetWeight;
        public float DangerAversionDistance;
        public float AvoidanceRadius;
        public float MoveSpeed;
    }
}

