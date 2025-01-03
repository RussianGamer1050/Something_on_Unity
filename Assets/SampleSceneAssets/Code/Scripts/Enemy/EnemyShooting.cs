using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public int damage = 10;
    public float fireRate = 1f;
    public float range = 15f;
    public Transform firePoint;

    public LineRenderer lineRenderer; // LineRenderer for visualizing shots
    public float lineDuration = 0.1f; // How long the line should be visible

    private float nextFireTime;
    private Transform playerTransform;

    void Update()
    {
        if (playerTransform != null && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        if (playerTransform == null) return;

        // Calculate the direction to the player
        Vector3 directionToPlayer = (playerTransform.position - firePoint.position).normalized;

        RaycastHit hit;
        Vector3 targetPoint = firePoint.position + directionToPlayer * range;

        if (Physics.Raycast(firePoint.position, directionToPlayer, out hit, range))
        {
            targetPoint = hit.point;

            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerHealth>()?.TakeDamage(damage);
                Debug.Log("Enemy hit the player!");
            }
        }

        StartCoroutine(DrawShotLine(targetPoint));
    }

    private System.Collections.IEnumerator DrawShotLine(Vector3 targetPoint)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, targetPoint);
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(lineDuration);
            lineRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
        }
    }
}
    