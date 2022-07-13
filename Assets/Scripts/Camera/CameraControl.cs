using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float DampTime = 0.2f; // Approximate time for the camera to refocus                
    public float ScreenEdgeBuffer = 4f; // Space between the top or bottom most target with the screen edge          
    public float MinSize = 7f; // The smallest orthographic size of the camera
    [HideInInspector] public Transform[] Targets; // Number of targets the camera needs to capture


    private Camera Camera;                        
    private float ZoomSpeed; // Reference speed for the smooth damping of the orthographic size                     
    private Vector3 MoveVelocity; // Reference velocity for the smooth damping of the position                
    public Vector3 DesiredPosition;              


    private void Awake()
    {
        Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move(); // Move the camera based on desired position
        Zoom(); // Change the size of the camera based on the required size
    }

    
    private void Move()
    {
        FindAveragePosition(); // Find the average position of the targets

        // Create a smooth transition of the camera zoom to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition, ref MoveVelocity, DampTime);
    }

    
    private void FindAveragePosition()
    {
        Vector3 averagePosition = new Vector3();
        int numberOfTargets = 0;

        // Go through all the targets and add their positions
        for (int i = 0; i < Targets.Length; i++)
        {
            // Check if the target is inactive
            if (!Targets[i].gameObject.activeSelf)
                continue;
            
            averagePosition += Targets[i].position; // Add target position value to the average position value
            numberOfTargets++; // Increment the number of targets in the average
        }

        if (numberOfTargets > 0)
            averagePosition /= numberOfTargets;

        averagePosition.y = transform.position.y; // Keep the same y value of the camera position

        DesiredPosition = averagePosition;
    }

    
    private void Zoom()
    {
        // Find the required size based on the desired position
        float requiredSize = FindRequiredSize();

        //  Create a smooth transition of the camera to the required size
        Camera.orthographicSize = Mathf.SmoothDamp(Camera.orthographicSize, requiredSize, ref ZoomSpeed, DampTime);
    }


    private float FindRequiredSize()
    {
        // Find the position of the camera rig's movement in its local space
        Vector3 desiredLocalPosition = transform.InverseTransformPoint(DesiredPosition);

        float size = 0f; // Start the camera size as 0

        // Go through all the targets
        for (int i = 0; i < Targets.Length; i++)
        {
            // Check if the target is inactive
            if (!Targets[i].gameObject.activeSelf)
                continue;

            // Find the position of the target in the camera's local space
            Vector3 targetLocalPosition = transform.InverseTransformPoint(Targets[i].position);

            // Find the position of the target from the desired position of the camera's local space
            Vector3 desiredPositionToTarget = targetLocalPosition - desiredLocalPosition;

            // Choose the largest out of the current size and the distance of the tank  top or bottom from the camera
            size = Mathf.Max (size, Mathf.Abs (desiredPositionToTarget.y));

            // Choose the largest out of the current size and the calcultae size based on the tank being to the left or right of the camera
            size = Mathf.Max (size, Mathf.Abs (desiredPositionToTarget.x) / Camera.aspect);
        }
        
        size += ScreenEdgeBuffer; // Add the edge buffer to the size

        size = Mathf.Max(size, MinSize); // Make sure the camera's size is not below minimum

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = DesiredPosition; // Set the camera's position to the desired position

        Camera.orthographicSize = FindRequiredSize(); // Find and set the required size of the camera
    }
}