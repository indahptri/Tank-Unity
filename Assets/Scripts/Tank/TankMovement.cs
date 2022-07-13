using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int PlayerNumber = 1;         
    public float Speed = 10f;          
    public float TurnSpeed = 180f;  
    public AudioSource MovementAudio;
    public AudioClip EngineIdling;       
    public AudioClip EngineDriving;      
    public float PitchRange = 0.2f; // Pitch range of the audio

    
    private string PlayerMovement; // Represents back and forth movement    
    private string PlayerTurn; // Represents left and right turn        
    private Rigidbody rb;         
    private float MovementInput;    
    private float TurnInput;        
    private float OriginalPitch;

    public TutorialPopup Tutorial;


    private void Awake()
    {
        // Initializes the rigidbody everytime the game starts
        rb = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        // Prevents physics forces to affect the tank
        rb.isKinematic = false;

        // Reset the inputs for movement and turn to 0 to disable auto movement
        MovementInput = 0f;
        TurnInput = 0f;
    }


    private void OnDisable ()
    {
        // Allows physics forces to affect the tank
        rb.isKinematic = true;
    }


    private void Start()
    {
        PlayerMovement = "Vertical" + PlayerNumber;
        PlayerTurn = "Horizontal" + PlayerNumber;

        OriginalPitch = MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Storing the player's input to the variables
        MovementInput = Input.GetAxis(PlayerMovement);
        TurnInput = Input.GetAxis(PlayerTurn);

        EngineAudio(); // Make sure the correct audio clip is playes based on the tank movement

        if(MovementInput != 0)
        {
            Tutorial.HasMoved = true;
        }

        if (TurnInput != 0)
        {
            Tutorial.HasTurned = true;
        }
    }

    
    private void EngineAudio()
    {
        // Check if the tank is moving or not from the input values

        if (Mathf.Abs(MovementInput) < 0.1f && Mathf.Abs(TurnInput) < 0.1f) // Mathf.Abs -> Get the absolute value (positive number) of the movement
        {
            if (MovementAudio.clip == EngineDriving)
            {
                MovementAudio.clip = EngineIdling;
                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange); // Randomize the pitch of the audio
                MovementAudio.Play();
            }
        }
        else
        {
            if (MovementAudio.clip == EngineIdling)
            {
                MovementAudio.clip = EngineDriving;
                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange); // Randomize the pitch of the audio
                MovementAudio.Play();
            }
        }

    }


    private void FixedUpdate()
    {
        Move(); // Adjust the tank's position based on the player's input
        Turn(); // Adjust the tank's rotation based on the player's input
    }

    
    private void Move()
    {
        // Create a new Vector 3 to store the movement based on input, speed and time between frames
        Vector3 movement = transform.forward * MovementInput * Speed * Time.deltaTime;

        // Apply the movement to current position of the rigidbody
        rb.MovePosition(transform.position + movement);
    }

    
    private void Turn()
    {
        // Create float value to store the degree of rotation based on input, speed and time between frames
        float turn = TurnInput * TurnSpeed * Time.deltaTime;

        // Apply the rotation to the Y-Axis
        Quaternion turnRotation = Quaternion.Euler(0, turn, 0);

        // Apply the rotation to the current rotation of the rigidbody
        rb.MoveRotation(transform.rotation * turnRotation);
    }
}