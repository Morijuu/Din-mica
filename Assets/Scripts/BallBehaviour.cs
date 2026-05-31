using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 4f;

    private void Start()
    {
        // Destruir la bola después de 4 segundos
        Destroy(gameObject, destroyDelay);
    }
}
