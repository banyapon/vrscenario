using UnityEngine;
using UnityEngine.XR;

public class VRInput : MonoBehaviour
{
    [Header("Left Hand")]
    public bool triggerLeft;
    public bool gripLeft;
    public bool primaryLeft;
    public bool secondaryLeft;
    public bool thumbstickClickLeft;
    public Vector2 thumbstickLeft;
    public bool menuLeft;

    [Header("Right Hand")]
    public bool triggerRight;
    public bool gripRight;
    public bool primaryRight;
    public bool secondaryRight;
    public bool thumbstickClickRight;
    public Vector2 thumbstickRight;
    public bool menuRight;

    InputDevice leftDevice;
    InputDevice rightDevice;

    void Start()
    {
        InitDevices();
    }

    void Update()
    {
        if (!leftDevice.isValid || !rightDevice.isValid)
            InitDevices();

        UpdateLeftHand();
        UpdateRightHand();
    }

    void InitDevices()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void UpdateLeftHand()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerLeft);
        leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out gripLeft);
        leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryLeft);
        leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryLeft);
        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out thumbstickClickLeft);
        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickLeft);
        leftDevice.TryGetFeatureValue(CommonUsages.menuButton, out menuLeft);
    }

    void UpdateRightHand()
    {
        rightDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerRight);
        rightDevice.TryGetFeatureValue(CommonUsages.gripButton, out gripRight);
        rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryRight);
        rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryRight);
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out thumbstickClickRight);
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickRight);
        rightDevice.TryGetFeatureValue(CommonUsages.menuButton, out menuRight);
    }
}
