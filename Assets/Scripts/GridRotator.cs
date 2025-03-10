using UnityEngine;

public class GridRotator : MonoBehaviour
{
    public float rotationSpeed = 50f; // Speed of rotation
    private Vector3 rotationDirection = Vector3.zero; // Direction of rotation

    void Update()
    {
        // Rotate continuously while a button is held
        if (rotationDirection != Vector3.zero)
        {
            transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    // Functions for starting rotation in a direction
    public void RotateRight() { rotationDirection = Vector3.up; }
    public void RotateLeft() { rotationDirection = Vector3.down; }
    public void RotateUp() { rotationDirection = Vector3.left; }
    public void RotateDown() { rotationDirection = Vector3.right; }

    // Function to stop rotation
    public void StopRotation() { rotationDirection = Vector3.zero; }
}
