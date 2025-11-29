using UnityEngine;

public class TestRotate : MonoBehaviour
{
    private Vector3 _eulerAngles;
    private Quaternion _rotation;
    public Transform pivot;

    private void Update()
    {
        transform.RotateAround(pivot.position, Vector3.up, 2 * Time.deltaTime);
    }
}
