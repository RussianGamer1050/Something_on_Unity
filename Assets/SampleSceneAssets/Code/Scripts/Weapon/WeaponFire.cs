using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    public float fireRate = 0.2f; // Time between shots
    private float nextFireTime = 0f; // Tracks when the player can fire again

    public Camera playerCamera; // The camera used for aiming
    public float maxRange = 100f; // Maximum range of the weapon

    public float bulletSpreadAngle = 1f; // Spread angle in degrees

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
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f); // Visualize the ray
        }
        else
        {
            Debug.Log("Missed"); // No hit detected
        }
    }
}
