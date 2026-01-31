using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NPCWaypointController : MonoBehaviour
{
    [Header("NPC Walk Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private BoxCollider pointA;
    [SerializeField] private BoxCollider pointB;

    [Header("Wait Settings")]
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;

    private BoxCollider currentTarget;
    private Rigidbody rb;
    private bool isWaiting = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
  
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;

 
        if (pointA != null && pointB != null)
        {
            currentTarget = pointB;
            pointA.gameObject.SetActive(false);
            pointB.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (isWaiting || pointA == null || pointB == null) return;

        MoveNPC();
    }

    private void MoveNPC()
    {
        Vector3 targetPos = currentTarget.bounds.center;
        Vector3 currentPos = rb.position;

 
        float directionX = (targetPos.x > currentPos.x) ? 1f : -1f;
        Vector3 direction = new Vector3(directionX, 0, 0);
        
        rb.MovePosition(currentPos + direction * walkSpeed * Time.fixedDeltaTime);


        transform.localScale = new Vector3(directionX, 1, 1);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!isWaiting && other.CompareTag("Point") || !isWaiting && other.CompareTag("Player"))
        {
            StartCoroutine(WaitAndSwitchSequence());
        }
    }

    private IEnumerator WaitAndSwitchSequence()
    {
        isWaiting = true;
        rb.linearVelocity = Vector3.zero;


        currentTarget.gameObject.SetActive(false);

  
        float waitDuration = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitDuration);


        currentTarget = (currentTarget == pointA) ? pointB : pointA;


        currentTarget.gameObject.SetActive(true);

        isWaiting = false;
    }

    void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.bounds.center, pointB.bounds.center);
        Gizmos.DrawWireCube(pointA.bounds.center, pointA.bounds.size);
        Gizmos.DrawWireCube(pointB.bounds.center, pointB.bounds.size);
    }
}