using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveTcpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public TCPController tcpController;
    public Vector3 translateDirection;
    public Vector3 rotateAxis;

    private bool _isHeld;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
    }

    private void Update()
    {
        if (_isHeld)
        {
            tcpController.RotateObject(rotateAxis);
            tcpController.TranslateObject(translateDirection);
        }
    }
}
