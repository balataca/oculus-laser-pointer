using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color enterColor = Color.white;
    [SerializeField] private Color downColor = Color.white;
    [SerializeField] private UnityEvent OnClick = new UnityEvent();

    private MeshRenderer meshRenderer = null;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        meshRenderer.material.color = enterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        meshRenderer.material.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        meshRenderer.material.color = downColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        meshRenderer.material.color = enterColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke();
    }
}
