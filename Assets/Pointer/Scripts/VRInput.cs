using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VRInput : BaseInput
{
    [Tooltip("OVRCamera Anchors")]
    public GameObject leftControllerAnchor;
    public GameObject rightControllerAnchor;
    public GameObject centerEyeAnchor;

    public Camera eventCamera = null;
    public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
    public static UnityAction<OVRInput.Controller, GameObject> OnControllerSource = null;

    private Dictionary<OVRInput.Controller, GameObject> controllerCollection = null;
    private OVRInput.Controller controller = OVRInput.Controller.None;
    
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
        
        if (activeController == controller)
            return;

        controller = activeController;
        
        GameObject controllerAnchor = null;
        controllerCollection.TryGetValue(controller, out controllerAnchor);

        if (controllerAnchor == null)
            controllerAnchor = centerEyeAnchor;
    
        if (OnControllerSource != null)
            OnControllerSource(controller, controllerAnchor);
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

    private Dictionary<OVRInput.Controller, GameObject> CreateControllerCollection()
    {
        return new Dictionary<OVRInput.Controller, GameObject>()
        {
            { OVRInput.Controller.LTrackedRemote, leftControllerAnchor },
            { OVRInput.Controller.RTrackedRemote, rightControllerAnchor },
            { OVRInput.Controller.Touchpad, centerEyeAnchor }
        };
    }
}
