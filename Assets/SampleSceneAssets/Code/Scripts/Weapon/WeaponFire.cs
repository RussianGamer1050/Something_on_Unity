using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    public float fireRate = 0.2f; // Time between shots
    private float nextFireTime = 0f; // Tracks when the player can fire again

    public Camera playerCamera; // The camera used for aiming
    public float maxRange = 100f; // Maximum range of the weapon

    public float bulletSpreadAngle = 1f; // Spread angle in degrees

    public float bulletDamage = 25f; // Damage per shot

    // Bullet amount variables
    public int magazineSize = 30; // Maximum bullets in a magazine
    public int currentAmmo; // Current bullets in the magazine
    public float reloadTime = 2f; // Time taken to reload
    private bool isReloading = false; // To prevent shooting during reload


    public ParticleSystem muzzleFlash; // Drag your muzzle flash particle system here

    private void Start()
    {
        currentAmmo = magazineSize; // Start with a full magazine
    }

    void Update()
    {
        // Check if the player is firing and the cooldown has passed
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            FireWeapon();
            nextFireTime = Time.time + fireRate; // Set the next allowed fire time
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void FireWeapon()
    {
        // Check if player reloading
        if (isReloading)
        {
            Debug.Log("Reloading..."); // Prevent firing during reload
            return;
        }

        // Check if player has no ammo
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Reload to continue firing.");
            return;
        }

        // Play the muzzle flash effect
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        currentAmmo--; // Decrease ammo count
        Debug.Log("Bullets left: " + currentAmmo);

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

    void Reload()
    {
        if (isReloading || currentAmmo == magazineSize) return; // Avoid redundant reloads

        Debug.Log("Reloading...");
        isReloading = true;

        // Delay the reload process
        Invoke(nameof(FinishReload), reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = magazineSize; // Refill the magazine
        isReloading = false;
        Debug.Log("Reload complete. Ammo refilled: " + currentAmmo);
    }

}
