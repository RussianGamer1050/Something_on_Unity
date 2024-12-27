using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    public float fireRate = 0.2f; // Time between shots
    private float nextFireTime = 0f; // Tracks when the player can fire again

    // Aiming variables
    public Camera playerCamera; // The camera used for aiming
    public float normalFOV = 60f; // Default field of view
    public float aimFOV = 40f; // Field of view when aiming
    public float aimSpeed = 10f; // Speed of FOV transition
    private bool isAiming = false; // Track aiming state

    // Weapon position variables
    public Transform weaponTransform; // Reference to the weapon
    public Vector3 normalPosition; // Default weapon position
    public Vector3 aimPosition; // Position of the weapon when aiming

    public float maxRange = 100f; // Maximum range of the weapon

    // Recoil settings
    public float recoilAmount = 0.1f; // Amount of weapon movement for recoil
    public float recoilRotation = 2f; // Amount of camera rotation for recoil
    public float recoilRecoverySpeed = 5f; // Speed of returning to normal position

    private Vector3 originalWeaponPosition; // Default weapon position
    private Vector3 currentRecoil; // Current recoil offset
    private Vector3 currentRecoilRotation; // Current recoil rotation

    private Vector3 weaponTargetPosition; // Final target position of the weapon (aim + recoil)
    private Vector3 recoilOffset; // Current recoil offset


    public float bulletSpreadAngle = 1f; // Spread angle in degrees

    public float bulletDamage = 25f; // Damage per shot

    // Bullet amount variables
    public int magazineSize = 30; // Maximum bullets in a magazine
    public int currentAmmo; // Current bullets in the magazine
    public float reloadTime = 4f; // Time taken to reload
    private bool isReloading = false; // To prevent shooting during reload

    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip shootingSound; // The shooting sound clip
    public AudioClip lastShotSound;
    public AudioClip reloadSound;

    public ParticleSystem muzzleFlash; // Drag your muzzle flash particle system here

    private void Start()
    {
        currentAmmo = magazineSize; // Start with a full magazine
        originalWeaponPosition = weaponTransform.localPosition; // Save default weapon position
    }

    void Update()
    {
        HandleAiming();

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

        // Smoothly recover from recoil
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        currentRecoilRotation = Vector3.Lerp(currentRecoilRotation, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);

        // Apply the final position to the weapon
        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponTargetPosition, Time.deltaTime * aimSpeed);

        // Apply recoil to the camera
        playerCamera.transform.localRotation *= Quaternion.Euler(currentRecoilRotation);
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

        // Play the shooting sound
        if ((shootingSound != null && audioSource != null) && currentAmmo == 1)
        {
            audioSource.PlayOneShot(lastShotSound);
        }
        else
        {
            audioSource.PlayOneShot(shootingSound);
        }

        // Apply recoil effect
        ApplyRecoil();

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

        if (shootingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        // Delay the reload process
        Invoke(nameof(FinishReload), reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = magazineSize; // Refill the magazine
        isReloading = false;
        Debug.Log("Reload complete. Ammo refilled: " + currentAmmo);
    }

    void HandleAiming()
    {
        if (Input.GetButtonDown("Fire2")) // Right mouse button
        {
            isAiming = true;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
        }

        // Smoothly transition camera FOV
        float targetFOV = isAiming ? aimFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimSpeed);

        // Calculate the target weapon position
        Vector3 aimOffset = isAiming ? aimPosition : normalPosition;
        weaponTargetPosition = aimOffset + recoilOffset;
    }

    void ApplyRecoil()
    {
        // Add positional recoil
        recoilOffset += Vector3.back * recoilAmount;

        // Add rotational recoil (camera pitch)
        currentRecoilRotation += Vector3.right * recoilRotation;
    }


}
