using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    private void Update()
    {
        // Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);

        // Look around
        float lookHorizontal = Input.GetAxis("Mouse X");
        float lookVertical = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, lookHorizontal * lookSpeed, Space.World);
        transform.Rotate(Vector3.left, lookVertical * lookSpeed, Space.Self);
    }
}
