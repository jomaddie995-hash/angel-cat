
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // Z-axis movement (Forward / Backward)
        if (Input.GetKey(KeyCode.S))
            moveZ = 1f;
        else if (Input.GetKey(KeyCode.W))
            moveZ = -1f;

        // X-axis movement (Left / Right)
        if (Input.GetKey(KeyCode.A))
            moveX = 1f;
        else if (Input.GetKey(KeyCode.D))
            moveX = -1f;

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Jump (Y-axis)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // Detect ground
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}