using UnityEngine;

// Controla el movimiento de la base de la catapulta (WASD) y el ciclo de disparo
// del brazo mediante un HingeJoint con spring.
//
// Ciclo de estados del brazo:
//   EnReposo → [Space] → Volviendo → (@-14°) → Lista → [Space] → Lanzando → (@30°) → EnReposo → ...
public class MovCatapulta : MonoBehaviour
{
    Rigidbody rb;

    // ─────────────────────────────────────────
    // MOVIMIENTO / ROTACIÓN DE LA BASE
    // ─────────────────────────────────────────

    [SerializeField] private float VelocidadMov    = 3f;   // fuerza de avance/retroceso (W/S)
    [SerializeField] private float VelocidadRot    = 3f;   // torque de giro (A/D)
    [SerializeField] private float MaxVelocidad    = 5f;   // tope de velocidad lineal
    [SerializeField] private float MaxVelocidadRot = 4f;   // tope de velocidad angular

    // ─────────────────────────────────────────
    // HINGE JOINT (BRAZO DE LA CATAPULTA)
    // ─────────────────────────────────────────

    // Referencia al HingeJoint del brazo. Si no se asigna en el Inspector,
    // el script lo busca automáticamente en los hijos del objeto.
    [SerializeField] private HingeJoint hinge;

    // Ángulo en grados que reporta hinge.angle en cada posición clave.
    // Estos valores se obtienen observando hinge.angle en Play Mode.
    [SerializeField] private float anguloListo       = -14f; // brazo en posición de disparo (cargado)
    [SerializeField] private float anguloLanzado     =  30f; // brazo tras completar el lanzamiento

    // Rigidez del spring (N·m/grado): cuánta fuerza aplica el muelle por cada grado
    // que el brazo está alejado del objetivo. Más alto = movimiento más rápido/brusco.
    [SerializeField] private float fuerzaCarga       = 3000f; // fuerza al volver a -14° (movimiento orgánico)
    [SerializeField] private float fuerzaLanzamiento = 8000f; // fuerza al lanzar (más potente y rápido)

    // Amortiguación del spring: frena las oscilaciones al llegar al ángulo objetivo.
    // Muy bajo → el brazo rebota. Muy alto → el brazo llega lento y sin inercia.
    [SerializeField] private float amortiguacion     = 100f;

    // Margen en grados para considerar que el brazo "llegó" al ángulo objetivo.
    // Si el brazo no entra nunca en este margen, el timeout evita que el estado se quede bloqueado.
    [SerializeField] private float umbralAngulo      =   5f;

    // Tiempo máximo (segundos) antes de forzar la transición de Lanzando → EnReposo.
    // Necesario por si el brazo no alcanza anguloLanzado exacto (p.ej. por colisiones).
    [SerializeField] private float tiempoMaxLanzando =   2f;

    // ─────────────────────────────────────────
    // MÁQUINA DE ESTADOS
    // ─────────────────────────────────────────

    // Cada estado representa una fase del ciclo de la catapulta.
    // Solo se aceptan inputs de Space en EnReposo y Lista para evitar
    // que el jugador interrumpa un movimiento en curso.
    private enum Estado
    {
        EnReposo,  // brazo parado (al inicio o tras un disparo), esperando input
        Volviendo, // spring activo, brazo moviéndose hacia anguloListo (-14°)
        Lista,     // brazo en -14°, listo para disparar con el siguiente Space
        Lanzando   // spring activo a máxima fuerza, brazo moviéndose hacia anguloLanzado (30°)
    }
    private Estado estado = Estado.EnReposo;

    // Acumula el tiempo transcurrido en estado Lanzando para el timeout de seguridad.
    private float tiempoLanzando = 0f;

    // Tiempo que el brazo lleva consecutivamente dentro del umbral de -14°.
    // La transición Volviendo → Lista solo se confirma cuando lleva tiempoEstable segundos
    // sin salirse, evitando falsos positivos por inercia al mover la base.
    private float tiempoEnPosicion = 0f;
    [SerializeField] private float tiempoEstable = 0.3f;

    // ─────────────────────────────────────────
    // INICIALIZACIÓN
    // ─────────────────────────────────────────

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Operador ??= : solo asigna si hinge es null (es decir, si no se asignó en el Inspector).
        // GetComponentInChildren busca en el objeto actual Y en todos sus hijos.
        hinge ??= GetComponentInChildren<HingeJoint>();
        if (hinge == null)
        {
            Debug.LogError("[Catapulta] HingeJoint no encontrado. Asígnalo en el Inspector.", this);
            enabled = false; // desactiva el script para no generar errores en cascada
            return;
        }

        // El spring se desactiva al inicio para que el brazo descanse en su posición
        // natural por física (gravedad), sin que el muelle lo force a ningún ángulo.
        // Se activa cuando el jugador pulsa Space por primera vez.
        hinge.useSpring = false;
    }

    // ─────────────────────────────────────────
    // HELPER: CONFIGURAR EL SPRING
    // ─────────────────────────────────────────

    // JointSpring es un struct (tipo valor), por eso hay que leerlo, modificarlo
    // y volver a asignarlo — no basta con modificar los campos directamente.
    void SetSpring(float target, float fuerza)
    {
        JointSpring s    = hinge.spring;   // copia del struct actual
        s.spring         = fuerza;         // rigidez del muelle
        s.damper         = amortiguacion;  // amortiguación
        s.targetPosition = target;         // ángulo objetivo en grados
        hinge.spring     = s;              // devuelve el struct modificado al joint
    }

    // ─────────────────────────────────────────
    // LÓGICA PRINCIPAL (INPUT + ESTADOS)
    // ─────────────────────────────────────────

    // Update corre una vez por frame. Se usa para input y para detectar
    // transiciones de estado (que no requieren precisión física de FixedUpdate).
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (estado)
            {
                case Estado.EnReposo:
                    // Space #1: activar el spring y mover el brazo a la posición de carga.
                    // fuerzaCarga (baja) produce un movimiento visible y orgánico hacia -14°.
                    estado = Estado.Volviendo;
                    hinge.useSpring = true;
                    SetSpring(anguloListo, fuerzaCarga);
                    break;

                case Estado.Lista:
                    // Space #2: disparar. El spring cambia de objetivo y de fuerza,
                    // impulsando el brazo rápidamente desde -14° hasta 30°.
                    estado = Estado.Lanzando;
                    SetSpring(anguloLanzado, fuerzaLanzamiento);
                    // TODO: soltar el proyectil aquí (desactivar su joint o activar su Rigidbody)
                    break;

                // En Volviendo y Lanzando el Space se ignora para no interrumpir el movimiento.
            }
        }

        // Detección de llegada a -14°: el brazo debe mantenerse dentro del umbralAngulo
        // durante tiempoEstable segundos consecutivos antes de confirmar la transición.
        // Si la inercia del movimiento lo saca del umbral, el contador se reinicia.
        if (estado == Estado.Volviendo)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(hinge.angle, anguloListo)) < umbralAngulo)
                tiempoEnPosicion += Time.deltaTime;
            else
                tiempoEnPosicion = 0f; // salió del umbral → reiniciar

            if (tiempoEnPosicion >= tiempoEstable)
            {
                tiempoEnPosicion = 0f;
                estado = Estado.Lista;
            }
        }

        // Vigilancia del lanzamiento: el spring empuja hacia 30°.
        // En cuanto llega (o pasa el tiempo máximo), se vuelve a EnReposo
        // y el spring se queda activo en 30° para mantener el brazo en esa posición.
        if (estado == Estado.Lanzando)
        {
            tiempoLanzando += Time.deltaTime;

            bool llegó   = Mathf.Abs(Mathf.DeltaAngle(hinge.angle, anguloLanzado)) < umbralAngulo;
            bool timeout = tiempoLanzando >= tiempoMaxLanzando; // evita quedarse bloqueado

            if (llegó || timeout)
            {
                tiempoLanzando = 0f;
                estado = Estado.EnReposo;
                // No se llama a SetSpring aquí: el spring permanece en (30°, fuerzaLanzamiento),
                // lo que mantiene el brazo fijo en la posición de lanzado hasta el próximo Space.
            }
        }
    }

    // ─────────────────────────────────────────
    // MOVIMIENTO DE LA BASE (FÍSICA)
    // ─────────────────────────────────────────

    // FixedUpdate corre en pasos fijos de física (por defecto 50 veces/segundo),
    // independientemente de los FPS. Toda fuerza aplicada a Rigidbodies debe ir aquí
    // para que la física sea consistente y no dependa del framerate.
    void FixedUpdate()
    {
        // W/S tienen prioridad: si se mueve hacia adelante/atrás no se puede girar.
        // Esto evita que la catapulta derrape al pulsar W+D simultáneamente.
        bool moviendo = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);

        if (moviendo)
        {
            // AddForce en transform.forward aplica la fuerza en la dirección local del objeto,
            // así la catapulta siempre avanza hacia donde apunta aunque esté girada.
            if (Input.GetKey(KeyCode.W))
                rb.AddForce(transform.forward * VelocidadMov);
            if (Input.GetKey(KeyCode.S))
                rb.AddForce(-transform.forward * VelocidadMov);
        }
        else
        {
            // AddTorque en Vector3.up gira el objeto sobre el eje Y mundial (vertical).
            if (Input.GetKey(KeyCode.A))
                rb.AddTorque(-Vector3.up * VelocidadRot);
            if (Input.GetKey(KeyCode.D))
                rb.AddTorque(Vector3.up * VelocidadRot);
        }

        // Limitadores de velocidad: si el Rigidbody supera el máximo, se escala
        // el vector de velocidad al módulo máximo sin cambiar su dirección.
        // (normalized devuelve el vector con módulo 1, luego se multiplica por el límite)
        if (rb.linearVelocity.magnitude > MaxVelocidad)
            rb.linearVelocity = rb.linearVelocity.normalized * MaxVelocidad;
        if (rb.angularVelocity.magnitude > MaxVelocidadRot)
            rb.angularVelocity = rb.angularVelocity.normalized * MaxVelocidadRot;
    }
}
