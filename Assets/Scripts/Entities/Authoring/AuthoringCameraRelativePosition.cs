using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;


public class AuthoringCameraRelativePosition : MonoBehaviour
{
    class Boulanger : Baker<AuthoringCameraRelativePosition>
    {
        public override void Bake(AuthoringCameraRelativePosition authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new ComponentCameraRelativePosition { });
        }
    }
}

public struct ComponentCameraRelativePosition : IComponentData
{
    public float3 Position;
}

