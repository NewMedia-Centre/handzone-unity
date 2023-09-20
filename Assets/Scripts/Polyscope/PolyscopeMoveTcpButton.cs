using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveTcpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
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
        UR_EthernetIPClient.ClearSendBuffer?.Invoke();
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name + " GameObject");
    }

    private void Update()
    {
        if (_isHeld)
        {
            tcpController.TranslateObject(translateDirection);
            tcpController.RotateObject(rotateAxis);
            
            RobotTranslator.MovePolyscopeTCP?.Invoke(translateDirection, rotateAxis);
        }
    }
}