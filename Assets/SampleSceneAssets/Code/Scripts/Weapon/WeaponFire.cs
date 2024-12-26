using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    public float fireRate = 0.2f; // Time between shots
    private float nextFireTime = 0f; // Tracks when the player can fire again

    public Camera playerCamera; // The camera used for aiming
    public float maxRange = 100f; // Maximum range of the weapon

    public float bulletSpreadAngle = 1f; // Spread angle in degrees

    public float bulletDamage = 25f; // Damage per shot

    public ParticleSystem muzzleFlash; // Drag your muzzle flash particle system here


    void FixedUpdate()
    {
        // Check if the player is firing and the cooldown has passed
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            FireWeapon();
            nextFireTime = Time.time + fireRate; // Set the next allowed fire time
        }
    }

    void FireWeapon()
    {
        // Play the muzzle flash effect
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Generate random spread within a cone
        Vector3 randomSpread = Random.insideUnitCircle * Mathf.Tan(bulletSpreadAngle * Mathf.Deg2Rad);
        Vector3 spreadDirection = playerCamera.transform.forward + playerCamera.transform.TransformDirection(randomSpread);

        // Raycast with the spread direction
        Ray ray = new Ray(playerCamera.transform.position, spreadDirection);
        RaycastHit hit;

        // Check if the ray hits anything within the range
        if (Physics.Raycast(ray, out hit, maxRange))
        {
            Debug.Log("Hit: " + hit.collider.name); // Log the name of the object hit

            // Check if the object hit has a Health component
            Health targetHealth = hit.collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(bulletDamage);
            }

            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f); // Visualize the ray
        }
        else
        {
            Debug.Log("Missed"); // No hit detected
        }
    }
}
