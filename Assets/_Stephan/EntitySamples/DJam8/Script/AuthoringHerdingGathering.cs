using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringHerdingGathering : MonoBehaviour
    {
        public GameObject Prefab;
        public float InitialRadius;
        public int Count;

        class Boulanger : Baker<AuthoringHerdingGathering>
        {
            public override void Bake(AuthoringHerdingGathering authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentHerdingGathering
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Renderable|TransformUsageFlags.WorldSpace),
                    Count = authoring.Count,
                    InitialRadius = authoring.InitialRadius
                });
            }
        }
    }

    public struct ComponentHerdingGathering : IComponentData
    {
        public Entity Prefab;
        public float InitialRadius;
        public int Count;
    }
}
