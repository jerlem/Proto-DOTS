using System;
using Unity.Entities;
using UnityEngine;


namespace Flocking
{
    public class AuthoringFlockingObstacle : MonoBehaviour
    {
        public class Boulanger : Baker<AuthoringFlockingObstacle>
        {
            public override void Bake(AuthoringFlockingObstacle authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentFlockingObstacle());
            }
        }
    }

    public struct ComponentFlockingObstacle : IComponentData
    {
    }
}
