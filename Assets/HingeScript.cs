using UnityEngine;

public class HingeReset : MonoBehaviour
{
    private HingeJoint hinge;

    [SerializeField] private float posicionInicial = -45f;

    float anguloActual;

    void Start()
    {
        // Coge el HingeJoint del mismo objeto automáticamente
        hinge = GetComponent<HingeJoint>();

        hinge.useSpring = true;

        anguloActual = posicionInicial;
    }

    void FixedUpdate()
    {
        anguloActual = Mathf.Lerp(
            anguloActual,
            posicionInicial,
            Time.fixedDeltaTime * 3f
        );

        JointSpring spring = hinge.spring;
        spring.targetPosition = anguloActual;
        hinge.spring = spring;
    }

    public void ResetBrazo()
    {
        anguloActual = posicionInicial;
    }

    public void SetPosicion(float angulo)
    {
        anguloActual = angulo;
    }
}