using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VRInput : BaseInput
{
    [Tooltip("OVRCamera Anchors")]
    public Transform leftControllerAnchor;
    public Transform rightControllerAnchor;
    public Transform centerEyeAnchor;

    public Camera eventCamera = null;
    public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
    public static UnityAction<OVRInput.Controller, Transform> OnControllerSource = null;

    private Dictionary<OVRInput.Controller, Transform> controllerCollection = null;
    private OVRInput.Controller controller = OVRInput.Controller.All;
    
    protected override void Awake()
    {
        GetComponent<BaseInputModule>().inputOverride = this;
        controllerCollection = CreateControllerCollection();
    }

    private void Update() {
        UpdateControllerSource();    
    }

    private void UpdateControllerSource()
    {
        OVRInput.Controller activeController = OVRInput.GetActiveController();

        if (controller == activeController)
            return;
        
        controller = activeController;
        
        if (OnControllerSource != null)
            OnControllerSource(controller, GetControllerAnchor());
    }

    private Transform GetControllerAnchor()
    {
        Transform controllerAnchor = null;
        controllerCollection.TryGetValue(controller, out controllerAnchor);

        if (controllerAnchor == null)
            controllerAnchor = centerEyeAnchor;

        return controllerAnchor;
    }

    public override bool GetMouseButton(int button)
    {
        return OVRInput.Get(clickButton, controller);
    }

    public override bool GetMouseButtonDown(int button)
    {
        return OVRInput.GetDown(clickButton, controller);
    }

    public override bool GetMouseButtonUp(int button)
    {
        return OVRInput.GetUp(clickButton, controller);
    }

    public override Vector2 mousePosition
    {
        get
        {
            return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
        }
    }

    private Dictionary<OVRInput.Controller, Transform> CreateControllerCollection()
    {
        return new Dictionary<OVRInput.Controller, Transform>()
        {
            { OVRInput.Controller.LTrackedRemote, leftControllerAnchor },
            { OVRInput.Controller.RTrackedRemote, rightControllerAnchor },
            { OVRInput.Controller.Touchpad, centerEyeAnchor }
        };
    }
}
