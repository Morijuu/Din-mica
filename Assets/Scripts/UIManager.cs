using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject CanvasMenu;
    public GameObject CanvasInicio;
    public GameObject CanvasOpciones;

    private void Start()
    {
        CanvasInicio.SetActive(true);
        CanvasMenu.SetActive(false);
        CanvasOpciones.SetActive(false);
    }

    public void MostrarMenuPrincipal()
    {
        CanvasInicio.SetActive(false);
        CanvasMenu.SetActive(true);
        CanvasOpciones.SetActive(false);

    }

    public void Opciones()
    {
        CanvasInicio.SetActive(false);
        CanvasMenu.SetActive(false);
        CanvasOpciones.SetActive(true);

    }
}