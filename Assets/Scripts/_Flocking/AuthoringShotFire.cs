using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Flocking
{
    public class AuthoringShotFire : MonoBehaviour
    {
        class Baker : Baker<AuthoringShotFire>
        {
            public override void Bake(AuthoringShotFire authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentShotFire { });
            }
        }
    }

    public struct ComponentShotFire : IComponentData
    {
        public float3 Origin;
        public float3 Destination;
    }
}