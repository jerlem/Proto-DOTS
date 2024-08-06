//AuthoringBot.cs
using Unity.Entities;
using UnityEngine;

public class AuthoringBot : MonoBehaviour
{
    // public GameObject Prefab;
    class Baker : Baker<AuthoringBot>
    {
        public override void Bake(AuthoringBot authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent<ComponentBot>(entity);
        }
    }
}
