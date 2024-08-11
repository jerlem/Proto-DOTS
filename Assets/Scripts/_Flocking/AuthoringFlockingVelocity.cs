using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Flocking
{
    public class AuthoringFlockingVelocity : MonoBehaviour
    {
        public float3 CurrentVelocity_L_T_1;
        public float3 PreviousPosition_L;

        class Boulanger : Baker<AuthoringFlockingVelocity>
        {
            public override void Bake(AuthoringFlockingVelocity authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentFlockingVelocity
                {
                    CurrentVelocity_L_T_1 = authoring.CurrentVelocity_L_T_1,
                    PreviousPosition_L = authoring.PreviousPosition_L
                });
            }
        }
    }

    public struct ComponentFlockingVelocity : IComponentData
    {
        public float3 CurrentVelocity_L_T_1;
        public float3 PreviousPosition_L;
    }
}
