using Unity.Entities;
using UnityEngine;


namespace Flocking
{
    public class AuthoringFlockingDanger : MonoBehaviour
    {
        public class Boulanger : Baker<AuthoringFlockingObstacle>
        {
            public override void Bake(AuthoringFlockingObstacle authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentFlockingDanger());
            }
        }
    }

    public struct ComponentFlockingDanger : IComponentData
    {
    }
}
