using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask TankMask; // Used to filter what the explosion affects -> should be set to Players
    public ParticleSystem ExplosionParticles; // Reference to the particles that will play on explosion       
    public AudioSource ExplosionAudio; // Reference to the audio that will play on explosion             
    public float MaxDamage = 100f; // Amount of damage done if the explosion is centred on a tank                 
    public float ExplosionForce = 1000f; // Amount of force added to a tank at the centre of the explosion           
    public float MaxLifeTime = 3f; // Time before the shell is removed (in secs)                
    public float ExplosionRadius = 5f; // Maximum distance away from the explosion tanks can be and are still affected             


    private void Start()
    {
        Destroy(gameObject, MaxLifeTime); // If it isn't destroyed by then -> Destroy the shell after it's lifetime
    }


    // Find all the tanks in an area around the shell and damage them
    private void OnTriggerEnter(Collider other)
    {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius and filter what the explosion affects
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask); // OverlapSphere -> Creates an imaginary sphere, similar to Raycast

        // Go through all the colliders collected
        for (int i = 0; i < colliders.Length; i++)
        {
            // Find the Rigidbody
            Rigidbody targetRb = colliders[i].GetComponent<Rigidbody>();

            // If it doesn't have a Rigidbody -> Go to next collider
            if (!targetRb)
                continue;

            // Add explosion force
            targetRb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

            // Find the TankHealth script associated to the Rigidbody
            TankHealth targetHealth = targetRb.GetComponent<TankHealth>();

            // If there is no TankHealth script attached to the game object -> Go to next collider
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target takes based on the distance to the shell
            float damage = CalculateDamage(targetRb.position);

            // Deal damage to the tank by passing the damage to the TakeDamage function in the TankHealth script
            targetHealth.TakeDamage(damage);
        }

        // Unparent the particles from the shell
        ExplosionParticles.transform.parent = null;


        // Play the particle system and the explosion audio
        ExplosionParticles.Play();
        ExplosionAudio.Play();

        // Once the particles finishes -> Destroy the game object they collide with
        Destroy(ExplosionParticles.gameObject, ExplosionParticles.duration);

        // Destroy the shell
        Destroy(gameObject);
    }


    // Calculate the amount of damage a target should take based on it's position
    private float CalculateDamage(Vector3 targetPosition)
    {
        // Create a vectore to store the distance from shell to tank
        Vector3 ExplosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target
        float ExplosionDistance = ExplosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance the target is away
        float RelativeDistance = (ExplosionRadius - ExplosionDistance) / ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage
        float damage = RelativeDistance * MaxDamage;

        // Make sure the minimum damage is always 0
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}