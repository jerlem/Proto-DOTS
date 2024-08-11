using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringFlockingGathering : MonoBehaviour
    {
        public GameObject Prefab;
        public float InitialRadius;
        public int Count;

        class Boulanger : Baker<AuthoringFlockingGathering>
        {
            public override void Bake(AuthoringFlockingGathering authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentFlockingGathering
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Renderable|TransformUsageFlags.WorldSpace),
                    Count = authoring.Count,
                    InitialRadius = authoring.InitialRadius
                });
            }
        }
    }

    public struct ComponentFlockingGathering : IComponentData
    {
        public Entity Prefab;
        public float InitialRadius;
        public int Count;
    }
}
