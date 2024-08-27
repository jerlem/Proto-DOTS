// ComponentConfig.cs
using Unity.Entities;

public struct ComponentConfig : IComponentData
{
    public int ITeamsCount;

    public Entity EntityBotPrefab;
}