using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Pointer : MonoBehaviour
{
    public float defaultLength = 3.0f;

    public EventSystem eventSystem = null;
    public StandaloneInputModule inputModule = null;    
    public VRInput VRInput = null;
    public LayerMask interactableMask = 0;
    public UnityAction<Vector3, bool> OnPointerUpdate = null;
 
    private LineRenderer lineRenderer = null;
    private GameObject currentObject = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        VRInput.OnControllerSource += UpdateOrigin;
    }

    private void OnDestroy()
    {
        VRInput.OnControllerSource += UpdateOrigin;
    }

    private void UpdateOrigin(OVRInput.Controller controller, Transform controllerAnchor)
    {
        bool hasController = controller != OVRInput.Controller.None;

        switch(controller) {
            case OVRInput.Controller.RTrackedRemote:
            case OVRInput.Controller.LTrackedRemote:
                lineRenderer.enabled = true;
                break;
            case OVRInput.Controller.Touchpad:
                lineRenderer.enabled = false;
                break;
            default:
                hasController = false;
                break;
        }

        transform.SetParent(controllerAnchor, false);
        gameObject.SetActive(hasController);
    }

    private void Update()
    {
        UpdatePointer();
    }

    private void UpdatePointer()
    {
        Vector3 endPosition = GetEnd();

        UpdateLength(endPosition);
        UpdatePointerStatus(endPosition);
    }

    private void UpdateLength(Vector3 endPosition)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void UpdatePointerStatus(Vector3 hitPoint)
    {
        bool hit = false;
    
        if (currentObject != null)
            hit = interactableMask == (interactableMask | (1 << currentObject.layer));
        
        if (OnPointerUpdate != null)
            OnPointerUpdate(hitPoint, hit);
    }

    private Vector3 GetEnd()
    {
        float distance = GetDistance();
        Vector3 endPosition = CalculateEnd(defaultLength);
        
        if (distance != 0.0f)
            endPosition = CalculateEnd(distance);
      
        return endPosition;
    }

    private float GetDistance()
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        List<RaycastResult> results = new List<RaycastResult>();

        eventData.position = inputModule.inputOverride.mousePosition;
        eventSystem.RaycastAll(eventData, results);

        RaycastResult closestResult = FindFirstRaycast(results);
        float distance = closestResult.distance;
        currentObject = closestResult.gameObject;

        distance = Mathf.Clamp(distance, 0.0f, defaultLength);

        return distance;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (!result.gameObject)
                continue;
            
            return result;
        }

        return new RaycastResult();
    }

    private Vector3 CalculateEnd(float length)
    {
        return transform.position + transform.forward * length;
    }
}
