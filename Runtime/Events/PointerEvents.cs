using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Registers pointer events on an object with a collider/trigger.
///
/// Requirements: 
/// - Physics Raycaster or Physics 2D Raycaster on camera object
/// - EventSystem with InputSystemUIInputModule in scene
/// 
/// </summary>
public class PointerEvents : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public UnityEvent<PointerEventData> onPointerClick;
    public UnityEvent<PointerEventData> onPointerEnter;
    public UnityEvent<PointerEventData> onPointerExit;
    public UnityEvent<PointerEventData> onPointerDown;
    public UnityEvent<PointerEventData> onPointerUp;

    static bool requirementsChecked = false;

    private void Update()
    {
        if (!requirementsChecked)
        {
            CheckRequirements();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp.Invoke(eventData);
    }

    void CheckRequirements()
    {
        if (!Camera.main)
        {
            Debug.LogWarning("No camera in scene. Pointer events will not work");
        }
        else
        {
            PhysicsRaycaster pr3d = Camera.main.GetComponent<PhysicsRaycaster>();
            Physics2DRaycaster pr2d = Camera.main.GetComponent<Physics2DRaycaster>();

            if (pr3d == null && pr2d == null)
            {
                Debug.LogWarning("Camera has no raycaster. Pointer events will not work");
            }
        }

        InputSystemUIInputModule inputModule = FindAnyObjectByType<InputSystemUIInputModule>();
        if (!inputModule)
        {
            Debug.LogWarning("No Event System in scene. Pointer events will not work");
        }

        requirementsChecked = true;
    }
}
