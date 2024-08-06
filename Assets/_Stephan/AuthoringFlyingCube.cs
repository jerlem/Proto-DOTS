using Unity.Entities;
using UnityEngine;

public class TagFlyingCubeAuthoring : MonoBehaviour
{
    public class Baker : Baker<TagFlyingCubeAuthoring>
    {
        public override void Bake(TagFlyingCubeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TagFlyingCube());
        }
    }
}