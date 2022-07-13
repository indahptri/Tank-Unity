using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int PlayerNumber = 1;       
    public Rigidbody Shell; // Prefab of the shell
    public Transform FireTransform; // A child of the tank where the shells are spawned    
    public Slider AimSlider; // A child of the tank that displays the current launch force          
    public AudioSource ShootingAudio; 
    public AudioClip ChargingClip;     
    public AudioClip FireClip;         
    public float MinLaunchForce = 15f; // The force given to the shell if the fire button is not held
    public float MaxLaunchForce = 30f; // The force given to the shell if the fire button is held for the max charge time
    public float MaxChargeTime = 0.75f; // How long the shell can charge for before it is fired at max force


    private string FireButton; // The input axis that is used for launching shells         
    private float CurrentLaunchForce; // The force that will be given to the shell when the fire button is released 
    private float ChargeSpeed; // How fast the launch force increases based on the max charge time        
    private bool Fired; // Whether or not the shell has been launched with this button press               

    public TutorialPopup Tutorial;

    private void OnEnable()
    {
        // When the tank is turned on -> Reset the launch force and the UI
        CurrentLaunchForce = MinLaunchForce;
        AimSlider.value = MinLaunchForce;
    }


    private void Start()
    {
        FireButton = "Fire" + PlayerNumber;

        // The rate that the launch force charges up = The range of possible forces divided by the max charge time
        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
    }


    // Track the current state of the fire button and make decisions based on the current launch force
    private void Update()
    {
        // Set the aim slider's default value to the min launch force
        AimSlider.value = MinLaunchForce;

        // If the current launch force has exceed the max force and the shell isn't fired yet
        if (CurrentLaunchForce >= MaxLaunchForce && !Fired)
        {
            // Set the current launch force's value to the max launch force and launch the shell
            CurrentLaunchForce = MaxLaunchForce;
            Fire();
        }
        // Else if the fire button is just pressed
        else if (Input.GetButtonDown(FireButton))
        {
            // Reset the fire flag and the current launch force
            Fired = false;
            CurrentLaunchForce = MinLaunchForce;

            // Change the audio clip to the charging clip and play it
            ShootingAudio.clip = ChargingClip;
            ShootingAudio.Play();
        }
        // Else if the fire button is being pressed and the shell isn't fired yet
        else if (Input.GetButton(FireButton) && !Fired)
        {
            // Increment the current launch force with the charge speed multiplied by the time taken from first pressed
            CurrentLaunchForce += ChargeSpeed * Time.deltaTime;

            // Update the aim slider's value
            AimSlider.value = CurrentLaunchForce;
        }
        // Else if the fire button is released and the shell isn't fired yet
        else if (Input.GetButtonUp(FireButton) && !Fired)
        {
            Fire();
        }

        if (Fired)
        {
            Tutorial.HasShoot = true;
        }
    }


    // Instantiate and launch the shell
    private void Fire()
    {
        Fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody
        Rigidbody ShellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction
        ShellInstance.velocity = CurrentLaunchForce * FireTransform.forward;

        // Change the audio clip to the firing clip and play it
        ShootingAudio.clip = FireClip;
        ShootingAudio.Play();

        // Reset the launch force
        CurrentLaunchForce = MinLaunchForce;
    }
}