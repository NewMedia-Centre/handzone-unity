using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonInteraction : MonoBehaviour
{
    public Image RedDot; // Reference to the red rectangle (reticle)
    public float raycastDistance = 10f;
    public LayerMask buttonLayer; // Set this to the layer containing your UI buttons
    public LineRenderer raycastLine; // Reference to the LineRenderer component

    private GameObject currentButton;
    private float hoverTime = 0f;
    private bool isHovering = false;

    private void Start()
    {
        // Initialize the LineRenderer
        raycastLine = GetComponent<LineRenderer>();

        // Initialize the buttonLayer in the Start method
        buttonLayer = LayerMask.GetMask("UI");
    }

    private void Update()
    {
        // Cast a ray from the camera center to detect UI elements
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, buttonLayer))
        {
            // Update the LineRenderer positions
            raycastLine.SetPosition(0, ray.origin);
            raycastLine.SetPosition(1, hit.point);

            // Check if the hit object is a UI button
            if (hit.collider.CompareTag("Button"))
            {
                if (!isHovering)
                {
                    isHovering = true;
                    currentButton = hit.collider.gameObject;
                    hoverTime = 0f;

                    Debug.Log("Hovering over a button: " + currentButton.name);
                }
                else
                {
                    hoverTime += Time.deltaTime;

                    if (hoverTime >= 1f)
                    {
                        // Hovered for 1 second, trigger the button click
                        ExecuteEvents.Execute(currentButton, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    }
                }
            }
            else
            {
                isHovering = false;
                currentButton = null;
                hoverTime = 0f;

                Debug.Log("No longer hovering over a button.");
            }
        }
        else
        {
            isHovering = false;
            currentButton = null;
            hoverTime = 0f;

            // Set LineRenderer positions to show a raycast line even when not hitting anything
            raycastLine.SetPosition(0, ray.origin);
            raycastLine.SetPosition(1, ray.origin + ray.direction * raycastDistance);
        }
    }
}
