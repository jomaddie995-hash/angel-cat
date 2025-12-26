using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -8f);
    public float followSpeed = 5f;

    void Start()
    {
        if (target != null)
        {
            // Snap camera to correct position at start
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // Remove TransformDirection to keep the offset global
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

}
