using Unity.Entities;
using Unity.Mathematics;

namespace Flocking
{
    public struct ComponentSampledAnimationClip : IComponentData
    {
        public float SampleRate;
        public int FrameCount;

        // Playback State
        public float CurrentTime;
        public int FrameIndex;
        public float TimeOffset;

        public BlobAssetReference<ComponentTransformSamples> TransformSamplesBlob;
    }

    public struct ComponentTransformSamples
    {
        public BlobArray<float3> TranslationSamples;
        public BlobArray<quaternion> RotationSamples;
    }
}
