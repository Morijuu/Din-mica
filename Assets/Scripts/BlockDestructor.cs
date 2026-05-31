using UnityEngine;
using System.Collections;

public class BlockDestructor : MonoBehaviour
{
    private bool hasScored = false;
    private bool isActive = false;
    [SerializeField] private float activationDelay = 2f;

    private void Start()
    {
        StartCoroutine(ActivateAfterDelay());
    }

    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        isActive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasScored && isActive && collision.gameObject.CompareTag("Suelo"))
        {
            hasScored = true;
            GameManager.Instance.AddPoints(10);

            Destroy(gameObject, 5f);
        }
    }
}
