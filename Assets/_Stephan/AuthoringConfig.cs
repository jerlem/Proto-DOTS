//AuthoringConfig.cs
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AuthoringConfig : MonoBehaviour
{
    //[Header("Bots")]
    //public int ITeamsCount;

    [Header("Prefabs")]
    public GameObject GOBot;

    class Baker : Baker<AuthoringConfig>
    {
        public override void Bake(AuthoringConfig authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            //int iTeamsCount = math.max(authoring.ITeamsCount, 1);
            AddComponent(entity, new ComponentConfig
            {
                //ITeamsCount = iTeamsCount,
                EntityBotPrefab = GetEntity(authoring.GOBot, TransformUsageFlags.Dynamic),
            });
        }
    }
}
