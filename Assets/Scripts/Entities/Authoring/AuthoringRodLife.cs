using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringRodLife : MonoBehaviour
    {
        public float InitRodLife = 10000.0f;
        class Boulanger : Baker<AuthoringRodLife>
        {
            public override void Bake(AuthoringRodLife authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentRodLife
                {
                    RodLife = authoring.InitRodLife
                });
            }
        }
    }

    public struct ComponentRodLife : IComponentData
    {
        public float RodLife;
    }
}
