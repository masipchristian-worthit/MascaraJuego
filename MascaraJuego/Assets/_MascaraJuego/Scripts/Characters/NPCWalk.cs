using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NPCWaypointController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator anim; // AHORA ES VISIBLE EN EL INSPECTOR
    
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
        
        // Intentamos buscarlo si se te olvidó asignarlo en el Inspector
        if (anim == null) 
            anim = GetComponentInChildren<Animator>(); // Busca en hijos también

        if (anim == null)
            Debug.LogError("¡FALTA EL ANIMATOR! Asígnalo en el Inspector del NPCWaypointController.");

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

        // Dirección
        float directionX = (targetPos.x > currentPos.x) ? 1f : -1f;
        Vector3 direction = new Vector3(directionX, 0, 0);

        rb.MovePosition(currentPos + direction * walkSpeed * Time.fixedDeltaTime);

        // Escala (Flip) manteniendo tamaño original
        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(scaleX * directionX, transform.localScale.y, transform.localScale.z);

        // Animación
        if(anim != null) 
        {
            anim.SetBool("isMoving", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isPoint = other.CompareTag("Point");
        bool isPlayer = other.CompareTag("Player");

        if (!isWaiting && (isPoint || isPlayer))
        {
            StartCoroutine(WaitAndSwitchSequence());
        }
    }

    private IEnumerator WaitAndSwitchSequence()
    {
        isWaiting = true;
        
        // Detener física
        // (Usa 'velocity' si usas Unity anterior a la versión 6)
        rb.linearVelocity = Vector3.zero; 

        // Detener animación
        if(anim != null) 
        {
            anim.SetBool("isMoving", false);
        }

        currentTarget.gameObject.SetActive(false);

        float waitDuration = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitDuration);

        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        currentTarget.gameObject.SetActive(true);

        isWaiting = false;
    }
    
    // (Gizmos omitidos para brevedad, no afectan la lógica)
    void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.bounds.center, pointB.bounds.center);
    }
}