using UnityEngine;

public class FireFlame : MonoBehaviour
{

    [SerializeField]
    public GameObject VFX;

    public void Fire()
    {
        GameObject left = GameObject.Instantiate(VFX, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
    }
}
