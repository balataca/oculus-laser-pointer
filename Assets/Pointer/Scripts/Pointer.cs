using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public float defaultLength = 3.0f;
  
    public EventSystem eventSystem = null;
    public StandaloneInputModule inputModule = null;

    public LayerMask layerMask;
    [HideInInspector]
    public Vector3 endPosition;
    [HideInInspector]
    public bool hasCollided = false;

    private LineRenderer lineRenderer = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLength();
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, GetEnd());
    }

    private Vector3 GetEnd()
    {
        float distance = GetCanvasDistance();
        RaycastHit hit = CreateForwardRaycast();
        endPosition = CalculateEnd(defaultLength);
        hasCollided = false;
        
        if (hit.collider)
        {
            endPosition = hit.point;
            hasCollided = true;
        }
        else if (distance != 0.0f)
        {
            endPosition = CalculateEnd(distance);
            hasCollided = true;
        }

        return endPosition;
    }

    private float GetCanvasDistance()
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        List<RaycastResult> results = new List<RaycastResult>();

        eventData.position = inputModule.inputOverride.mousePosition;
        eventSystem.RaycastAll(eventData, results);

        RaycastResult closestResult = FindFirstRaycast(results);
        float distance = closestResult.distance;

        distance = Mathf.Clamp(distance, 0.0f, defaultLength);

        return distance;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (!result.gameObject)
            {
                continue;
            }

            return result;
        }

        return new RaycastResult();
    }

    private RaycastHit CreateForwardRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLength, layerMask);

        return hit;
    }

    private Vector3 CalculateEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }
}
