#if UNITY_EDITOR

using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Flocking
{
    // Demonstrate taking some data available in editor about GameObjects and save in
    // a runtime format suitable for Component system updates.
    // - Playback first attached animation clip (only expect one)
    // - Record positions and rotations at specified rate
    // - Store samples into DynamicBuffer
    public class AuthoringTransformRecorder : MonoBehaviour
    {
        [Range(2, 120)] public int SamplesPerSecond = 60;

        class Boulanger : Baker<AuthoringTransformRecorder>
        {
            public override void Bake(AuthoringTransformRecorder authoring)
            {
                var animationClips = AnimationUtility.GetAnimationClips(authoring.gameObject);
                var animationClip = animationClips[0];
                var lengthSeconds = animationClip.length;
                var sampleRate = 1.0f / authoring.SamplesPerSecond;
                var frameCount = (int)(lengthSeconds / sampleRate);
                if (frameCount < 2) // Minimum two frames of animation to capture.
                {
                    return;
                }

                var s = 0.0f;

                var blobBuilder = new BlobBuilder(Allocator.Temp);
                ref var transformSamplesBlob = ref blobBuilder.ConstructRoot<ComponentTransformSamples>();
                var translationSamples = blobBuilder.Allocate(ref transformSamplesBlob.TranslationSamples, frameCount);
                var rotationSamples = blobBuilder.Allocate(ref transformSamplesBlob.RotationSamples, frameCount);

                for (int i = 0; i < frameCount; i++)
                {
                    animationClip.SampleAnimation(authoring.gameObject, s);

                    translationSamples[i] = authoring.gameObject.transform.position;
                    rotationSamples[i] = authoring.gameObject.transform.rotation;

                    s += sampleRate;
                }

                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ComponentSampledAnimationClip
                {
                    FrameCount = frameCount,
                    SampleRate = sampleRate,
                    CurrentTime = 0.0f,
                    FrameIndex = 0,
                    TimeOffset = 0,
                    TransformSamplesBlob = blobBuilder.CreateBlobAssetReference<ComponentTransformSamples>(Allocator.Persistent)
                });

                blobBuilder.Dispose();
            }
        }
    }
}

#endif
