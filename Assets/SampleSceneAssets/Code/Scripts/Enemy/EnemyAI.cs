using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform[] coverPoints; // Assign cover points in the Inspector
    private NavMeshAgent agent;
    private Transform currentCoverPoint;
    private bool isInCover = false;

    private Transform playerTransform;
    private SphereCollider detectionCollider; // Detection collider reference
    public float detectionRadius = 10f;
    public float expandedRadius = 20f;
    private bool playerDetected = false;

    private static Dictionary<Transform, bool> coverPointStatus = new Dictionary<Transform, bool>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeCoverPointStatus();

        detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<SphereCollider>();
            detectionCollider.isTrigger = true;
            detectionCollider.radius = detectionRadius;
        }
    }
    void Update()
    {
        if (playerDetected && playerTransform != null)
        {
            // Rotate to face the player while moving
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0; // Ignore vertical rotation
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (!isInCover && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            isInCover = true;
            Debug.Log(gameObject.name + " is now in cover.");
        }
    }

    void InitializeCoverPointStatus()
    {
        // Ensure the dictionary is initialized with all cover points
        foreach (Transform coverPoint in coverPoints)
        {
            if (!coverPointStatus.ContainsKey(coverPoint))
            {
                coverPointStatus.Add(coverPoint, false); // Mark all cover points as unoccupied
            }
        }
    }

    void MoveToNearestCover()
    {
        Transform nearestCover = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform coverPoint in coverPoints)
        {
            if (coverPointStatus[coverPoint]) continue; // Skip if the cover point is occupied

            float distance = Vector3.Distance(transform.position, coverPoint.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestCover = coverPoint;
            }
        }

        if (nearestCover != null)
        {
            currentCoverPoint = nearestCover;
            coverPointStatus[currentCoverPoint] = true; // Mark the selected cover point as occupied
            agent.SetDestination(currentCoverPoint.position);
        }
        else
        {
            Debug.LogWarning("No available cover points!");
        }
    }

    private void OnDestroy()
    {
        // Free up the cover point when this enemy is destroyed
        if (currentCoverPoint != null && coverPointStatus.ContainsKey(currentCoverPoint))
        {
            coverPointStatus[currentCoverPoint] = false;
        }
    }

    // Player detection logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerDetected)
        {
            playerDetected = true;
            playerTransform = other.transform; // Store reference to the player
            Debug.Log(gameObject.name + " detected the player!");

            // Expand detection radius
            detectionCollider.radius = expandedRadius;

            // Move to the nearest cover
            MoveToNearestCover();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
            playerTransform = null;
            Debug.Log(gameObject.name + " lost sight of the player.");

            // Restore original detection radius
            detectionCollider.radius = detectionRadius;
        }
    }
}
