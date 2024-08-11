using Unity.Entities;
using UnityEngine;


public class AuthoringDebugSphere : MonoBehaviour
{
    //public GameObject Prefab;

    class Boulanger : Baker<AuthoringDebugSphere>
    {
        public override void Bake(AuthoringDebugSphere authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new ComponentDebugSphere
            {
                //Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Renderable|TransformUsageFlags.WorldSpace)
            });
        }
    }
}

public struct ComponentDebugSphere : IComponentData
{
    //public Entity Prefab;
}

