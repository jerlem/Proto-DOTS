using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringFlockingLife : MonoBehaviour
    {
        class Boulanger : Baker<AuthoringFlockingLife>
        {
            public override void Bake(AuthoringFlockingLife authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentFlockingLife { });
            }
        }
    }

    public struct ComponentFlockingLife : IComponentData
    {
        public bool BIsMarkedForDeath;
    }
}
