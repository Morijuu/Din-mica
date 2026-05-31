using UnityEngine;
using UnityEngine.UI;

public class LoadingBarUI : MonoBehaviour
{
    [SerializeField] private Image loadingBar;
    [SerializeField] private float loadingDuration = 2f;
    [SerializeField] private float coolingDuration = 5f;

    private MovCatapulta catapulta;
    private float currentProgress = 0f;
    private bool isLoading = false;
    private bool isCooling = false;

    private void Start()
    {
        catapulta = FindFirstObjectByType<MovCatapulta>();

        if (loadingBar != null)
        {
            loadingBar.fillAmount = 0f;
        }
    }

    private void Update()
    {
        if (catapulta == null) return;

        // Detectar si está en estado de carga o enfriamiento observando el estado interno
        // Usaremos reflection para acceder al estado privado
        var stateField = typeof(MovCatapulta).GetField("estado",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var timerField = typeof(MovCatapulta).GetField("timer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tiempoRecargaField = typeof(MovCatapulta).GetField("tiempoRecarga",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tiempoEnfriamientoField = typeof(MovCatapulta).GetField("tiempoEnfriamiento",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (stateField != null && timerField != null)
        {
            var estado = stateField.GetValue(catapulta);
            float timer = (float)timerField.GetValue(catapulta);
            float tiempoRecarga = (float)tiempoRecargaField.GetValue(catapulta);
            float tiempoEnfriamiento = (float)tiempoEnfriamientoField.GetValue(catapulta);

            string stateString = estado.ToString();

            if (stateString == "Recargando")
            {
                currentProgress = timer / tiempoRecarga;
            }
            else if (stateString == "Enfriando")
            {
                currentProgress = timer / tiempoEnfriamiento;
            }
            else
            {
                currentProgress = 0f;
            }

            if (loadingBar != null)
            {
                loadingBar.fillAmount = currentProgress;
            }
        }
    }
}
