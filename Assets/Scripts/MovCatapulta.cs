using UnityEngine;

public class MovCatapulta : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private float VelocidadMov    = 3f;
    [SerializeField] private float VelocidadRot    = 3f;
    [SerializeField] private float MaxVelocidad    = 5f;
    [SerializeField] private float MaxVelocidadRot = 4f;

    [SerializeField] private HingeJoint hinge;
    [SerializeField] private Transform  ejeRotacion;

    [SerializeField] private float tiempoRecarga      = 2f;
    [SerializeField] private float tiempoEnfriamiento = 5f;

    [SerializeField] private GameObject prefabBola;
    [SerializeField] private Transform  spawnBola;

    // El spring está siempre activo una vez iniciado.
    // Su target position en el Inspector es el ángulo de lanzamiento.
    // Los limits en el Inspector definen la posición de carga.
    // useLimits = true  → brazo bloqueado en posición de carga
    // useLimits = false → spring empuja libremente hacia el ángulo de lanzamiento

    private enum Estado { EnReposo, Recargando, Lista, Enfriando }
    private Estado estado = Estado.EnReposo;
    private float  timer;

    void Start()
    {
        rb    = GetComponent<Rigidbody>();
        hinge ??= GetComponentInChildren<HingeJoint>();
        hinge.useSpring = false;
        hinge.useLimits = false;
    }

    void Update()
    {
        if (estado == Estado.Recargando)
        {
            timer += Time.deltaTime;
            if (timer >= tiempoRecarga)
            {
                Instantiate(prefabBola, spawnBola.position, spawnBola.rotation);
                timer  = 0f;
                estado = Estado.Lista;
            }
            return;
        }

        if (estado == Estado.Enfriando)
        {
            timer += Time.deltaTime;
            if (timer >= tiempoEnfriamiento)
            {
                timer  = 0f;
                estado = Estado.EnReposo;
            }
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (estado == Estado.EnReposo)
        {
            hinge.useSpring = true;
            hinge.useLimits = true;
            timer  = 0f;
            estado = Estado.Recargando;
        }
        else if (estado == Estado.Lista)
        {
            hinge.useLimits = false;
            timer  = 0f;
            estado = Estado.Enfriando;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
                rb.AddForce(transform.forward * VelocidadMov);
            if (Input.GetKey(KeyCode.S))
                rb.AddForce(-transform.forward * VelocidadMov);
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
                transform.RotateAround(ejeRotacion.position, ejeRotacion.up, -VelocidadRot);
            if (Input.GetKey(KeyCode.D))
                transform.RotateAround(ejeRotacion.position, ejeRotacion.up,  VelocidadRot);
        }

        if (rb.linearVelocity.magnitude > MaxVelocidad)
            rb.linearVelocity = rb.linearVelocity.normalized * MaxVelocidad;
        if (rb.angularVelocity.magnitude > MaxVelocidadRot)
            rb.angularVelocity = rb.angularVelocity.normalized * MaxVelocidadRot;
    }
}
