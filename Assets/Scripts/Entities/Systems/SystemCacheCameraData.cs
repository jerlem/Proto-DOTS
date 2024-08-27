using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class SystemCacheCameraData : SystemBase
{
    public static float3 CameraPosition;
    public static float3 CameraForward;

    protected override void OnUpdate()
    {
        // Ensure Camera.main is not null
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Transform cameraTransform = mainCamera.transform;
            //CameraPosition = cameraTransform.position;
            //CameraForward = cameraTransform.forward;

            //Debug.Log("Main Camera Transform: " + mainCamera.transform);
            //Debug.Log("Main Camera Local Position: " + mainCamera.transform.localPosition);
            //Debug.Log("Main Camera Local Rotation: " + mainCamera.transform.localRotation);
            //Debug.Log("Main Camera Local Forward: " + mainCamera.transform.localRotation * Vector3.forward);

            CameraPosition = mainCamera.transform.TransformPoint(mainCamera.transform.localPosition);
            //Debug.Log("Camera Position (World Space): " + CameraPosition);

            CameraForward = mainCamera.transform.TransformDirection(mainCamera.transform.localRotation * Vector3.forward);
            //Debug.Log("Camera Forward (World Space): " + CameraForward);
        }
        else
        {
            // Handle the case where Camera.main is null
            //Debug.LogWarning("Main camera is not found. Ensure that a camera is tagged as 'MainCamera'.");
        }
    }
}
