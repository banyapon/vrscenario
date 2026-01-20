using UnityEngine;

public class VRLocalXRBinder : MonoBehaviour
{
    public float reInitialize = 1;

    [SerializeField] private VRAvatarNetwork avatar;
    public Transform xrOriginBase;
    public Transform xrHead;
    [Space(10)]
    public Transform xrLeftHand;
    public Transform xrRightHand;
    [Space(10)]
    public Transform xrLeftController;
    public Transform xrRightController;

    bool isConnect;
    void Start()
    {
        VRNetworkController.Instance.onClientConnected += () =>
        {
            isConnect = true;
        };
        VRNetworkController.Instance.onClientDisconnected += () =>
        {
            isConnect = false;
            avatar = null;
        };
    }

    void Initialize()
    {
        avatar = VRAvatarNetwork.Local;
    }

    float time;
    private void Update()
    {
        if (!isConnect) return;
        if (avatar != null) return;
        if (time >= reInitialize)
        {
            time = 0;
            Initialize();
        }

        time += Time.deltaTime;
    }

    void LateUpdate()
    {
        if (!isConnect) return;
        if (avatar == null) return;
        if (!avatar.IsOwner) return;

        avatar.originBase.localPosition = xrOriginBase.localPosition;
        avatar.originBase.localRotation = xrOriginBase.localRotation;

        avatar.head.localPosition = xrHead.localPosition;
        avatar.head.localRotation = xrHead.localRotation;

        avatar.leftHand.localPosition = xrLeftHand.localPosition;
        avatar.leftHand.localRotation = xrLeftHand.localRotation;
        avatar.leftHand.gameObject.SetActive(xrLeftHand.gameObject.activeSelf);

        avatar.rightHand.localPosition = xrRightHand.localPosition;
        avatar.rightHand.localRotation = xrRightHand.localRotation;
        avatar.rightHand.gameObject.SetActive(xrRightHand.gameObject.activeSelf);

        avatar.leftController.localPosition = xrLeftController.localPosition;
        avatar.leftController.localRotation = xrLeftController.localRotation;
        avatar.leftController.gameObject.SetActive(xrLeftController.gameObject.activeSelf);

        avatar.rightController.localPosition = xrRightController.localPosition;
        avatar.rightController.localRotation = xrRightController.localRotation;
        avatar.rightController.gameObject.SetActive(xrRightController.gameObject.activeSelf);
    }

}
