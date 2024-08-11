using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringRodStewartPieceOfEight : MonoBehaviour
    {
        public GameObject Prefab;
        public int numSegments = 8; // number of segments in the rod
        public float segmentLength = 1.0f; // length of each segment
        public float rodRadius = 0.5f; // radius of the rod

        class Boulanger : Baker<AuthoringRodStewartPieceOfEight>
        {
            public override void Bake(AuthoringRodStewartPieceOfEight authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new ComponentRodStewartPieceOfEight
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace),
                    NumSegments = authoring.numSegments,
                    SegmentLength = authoring.segmentLength,
                    RodRadius = authoring.rodRadius,
                });
            }
        }
    }

    public struct ComponentRodStewartPieceOfEight : IComponentData
    {
        public Entity Prefab;
        public int NumSegments;
        public float SegmentLength;
        public float RodRadius;
    }
}
