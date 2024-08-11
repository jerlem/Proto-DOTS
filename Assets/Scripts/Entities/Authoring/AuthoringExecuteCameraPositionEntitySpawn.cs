using Unity.Entities;
using UnityEngine;


public class AuthoringExecuteCameraPositionEntitySpawn : MonoBehaviour
{
    class Boulanger : Baker<AuthoringExecuteCameraPositionEntitySpawn>
    {
        public override void Bake(AuthoringExecuteCameraPositionEntitySpawn authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new ComponentExecuteCameraPositionEntitySpawn{});
        }
    }
}

public struct ComponentExecuteCameraPositionEntitySpawn : IComponentData { }

