using UnityEngine;

public class MovCatapulta : MonoBehaviour
{
    Rigidbody rb;

    // =========================
    // MOVIMIENTO / ROTACIÓN
    // =========================
    [SerializeField] private float VelocidadMov = 3f;
    [SerializeField] private float VelocidadRot = 3f;

    [SerializeField] private float MaxVelocidad = 5f;
    [SerializeField] private float MaxVelocidadRot = 4f;

    // =========================
    // HINGE JOINT (BRAZO CATAPULTA)
    // =========================
    [SerializeField] private HingeJoint hinge;

    [SerializeField] private float objetivoHinge = -45f;
    [SerializeField] private float velocidadInterpolacion = 3f;

    private float anguloActual;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Si no lo asignas en inspector, lo busca
/*        if (hinge == null)
            hinge = GetComponentInChildren<HingeJoint>();

        // Activar spring
        hinge.useSpring = true;

        // Configurar spring base
        JointSpring spring = hinge.spring;

        spring.spring = 1500f;   // fuerza del muelle
        spring.damper = 80f;     // amortiguación
        hinge.spring = spring;*/
    }

    private void FixedUpdate()
    {
        bool moviendo =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S);

        // =========================
        // MOVIMIENTO TIENE PRIORIDAD
        // =========================
        if (moviendo)
        {
            if (Input.GetKey(KeyCode.W))
                rb.AddForce(transform.forward * VelocidadMov);

            if (Input.GetKey(KeyCode.S))
                rb.AddForce(-transform.forward * VelocidadMov);
        }
        else
        {
            // SOLO ROTAR SI NO SE MUEVE
            if (Input.GetKey(KeyCode.A))
                rb.AddTorque(-Vector3.up * VelocidadRot);

            if (Input.GetKey(KeyCode.D))
                rb.AddTorque(Vector3.up * VelocidadRot);
        }

        if (Input.GetKey(KeyCode.Space))
        {

        }


        // =========================
        // LIMITAR VELOCIDADES
        // =========================
        if (rb.linearVelocity.magnitude > MaxVelocidad)
            rb.linearVelocity = rb.linearVelocity.normalized * MaxVelocidad;

        if (rb.angularVelocity.magnitude > MaxVelocidadRot)
            rb.angularVelocity = rb.angularVelocity.normalized * MaxVelocidadRot;

        // =========================
        // HINGE JOINT SUAVE
        // =========================

        // Interpolación suave del ángulo
 /*       anguloActual = Mathf.Lerp(
            anguloActual,
            objetivoHinge,
            Time.fixedDeltaTime * velocidadInterpolacion
        );

        // Aplicar al spring del HingeJoint
        JointSpring spring = hinge.spring;
        spring.targetPosition = anguloActual;
        hinge.spring = spring;*/
    }
}