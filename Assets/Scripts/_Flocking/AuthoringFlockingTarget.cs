using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringFlockingTarget : MonoBehaviour
    {
        class Boulanger : Baker<AuthoringFlockingTarget>
        {
            public override void Bake(AuthoringFlockingTarget authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentFlockingTarget());
            }
        }
    }

    public struct ComponentFlockingTarget : IComponentData
    {
    }
}
