using System;
using Unity.Entities;
using UnityEngine;

namespace Flocking
{
    public class AuthoringFlockingCrawler : MonoBehaviour
    {
        public float InitialRayLength = 1f;
        public float MaxRayLength = 100f; // Default maximum ray length

        class Boulanger : Baker<AuthoringFlockingCrawler>
        {
            public override void Bake(AuthoringFlockingCrawler authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddSharedComponent(entity, new SharedComponentFlockingCrawler { MaxRayLength = authoring.MaxRayLength });
            }
        }
    }

    [Serializable]
    public struct SharedComponentFlockingCrawler : ISharedComponentData
    {
        public float InitialRayLength;
        public float MaxRayLength;
    }
}
