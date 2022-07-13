using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float StartingHealth = 100f;          
    public Slider Slider;                        
    public Image FillImage;                      
    public Color FullHealthColor = Color.green;  
    public Color ZeroHealthColor = Color.red;    
    public GameObject ExplosionPrefab;
    

    private AudioSource ExplosionAudio;          
    private ParticleSystem ExplosionParticles;   
    private float CurrentHealth;  
    private bool Dead;            


    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it
        ExplosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab
        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required
        ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        CurrentHealth = StartingHealth;
        Dead = false;

        SetHealthUI(); // Update the health slider's value and color
    }


    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount; // Adjust the tank's current health

        SetHealthUI();

        // Check if the tank is alive or dead
        if (CurrentHealth <= 0f && !Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        Slider.value = CurrentHealth; // Adjust the value of the slider

        // Adjust the color of the slider
        // Interpolate the color between the choosen colours based on the current percentage of the starting health
        FillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, CurrentHealth / StartingHealth);
    }


    private void OnDeath()
    {
        Dead = true;

        // Move the instantiated explosion prefab to the tank's position
        ExplosionParticles.transform.position = transform.position;
        ExplosionParticles.gameObject.SetActive(true);

        // Play the effects and audio for the death of the tank
        ExplosionParticles.Play();
        ExplosionAudio.Play();

        gameObject.SetActive(false); // Deactivate the dead tank
    }
}