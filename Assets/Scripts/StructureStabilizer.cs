using UnityEngine;
using System.Collections;

public class StructureStabilizer : MonoBehaviour
{
    [SerializeField] private float stabilizationDelay = 2f;

    private void Start()
    {
        DisablePhysicsTemporarily();
    }

    private void DisablePhysicsTemporarily()
    {
        // Desactivar gravedad en todos los bloques de la estructura
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Esperar y luego activar la física
        StartCoroutine(EnablePhysicsAfterDelay());
    }

    private IEnumerator EnablePhysicsAfterDelay()
    {
        yield return new WaitForSeconds(stabilizationDelay);

        // Activar gravedad y física
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
