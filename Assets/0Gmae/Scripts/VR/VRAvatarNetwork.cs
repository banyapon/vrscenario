using Unity.Netcode;
using UnityEngine;

public class VRAvatarNetwork : NetworkBehaviour
{
    public static VRAvatarNetwork Local;

    public float smoothSpeed = 15;

    [Header("Anchors")]
    public Transform originBase;
    public Transform head;

    [Space(10)]
    public Transform leftHand;
    public Transform rightHand;

    [Space(10)]
    public Transform leftController;
    public Transform rightController;

    NetworkVariable<PoseData> originBasePos = new(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<PoseData> headPose = new(writePerm: NetworkVariableWritePermission.Owner);

    NetworkVariable<PoseData> leftHandPose = new(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<PoseData> rightHandPose = new(writePerm: NetworkVariableWritePermission.Owner);

    NetworkVariable<PoseData> leftControllerPose = new(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<PoseData> rightControllerPose = new(writePerm: NetworkVariableWritePermission.Owner);

    NetworkVariable<bool> leftHandActive = new(true, writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> rightHandActive = new(true, writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> leftControllerActive = new(true, writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> rightControllerActive = new(true, writePerm: NetworkVariableWritePermission.Owner);

    bool networkAlive;
    public override void OnNetworkSpawn()
    {
        networkAlive = true;
        if (IsOwner)
        {
            Local = this;
            MeshRenderer[] renderers = originBase.GetComponentsInChildren<MeshRenderer>();
            foreach (var ren in renderers)
            {
                ren.enabled = false;
            }

            SkinnedMeshRenderer[] skinnedMeshRenderer = originBase.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var ren in skinnedMeshRenderer)
            {
                ren.enabled = false;
            }
        }
        else
        {
            leftHandActive.OnValueChanged += OnLeftHandActiveChanged;
            rightHandActive.OnValueChanged += OnRightHandActiveChanged;
            leftControllerActive.OnValueChanged += OnLeftControllerActiveChanged;
            rightControllerActive.OnValueChanged += OnRightControllerActiveChanged;

            OnLeftHandActiveChanged(false, leftHandActive.Value);
            OnRightHandActiveChanged(false, rightHandActive.Value);
            OnLeftControllerActiveChanged(false, leftControllerActive.Value);
            OnRightControllerActiveChanged(false, rightControllerActive.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        networkAlive = false;
        if (Local == this) Local = null;

        leftHandActive.OnValueChanged -= OnLeftHandActiveChanged;
        rightHandActive.OnValueChanged -= OnRightHandActiveChanged;
        leftControllerActive.OnValueChanged -= OnLeftControllerActiveChanged;
        rightControllerActive.OnValueChanged -= OnRightControllerActiveChanged;
    }

    void Update()
    {
        if (!networkAlive) return;
        if (!IsSpawned) return;

        if (IsOwner)
        {
            if (NetworkManager == null || !NetworkManager.IsListening)
                return;

            originBasePos.Value = ToPose(originBase);
            headPose.Value = ToPose(head);

            leftHandPose.Value = ToPose(leftHand);
            rightHandPose.Value = ToPose(rightHand);

            leftControllerPose.Value = ToPose(leftController);
            rightControllerPose.Value = ToPose(rightController);

            leftHandActive.Value = leftHand.gameObject.activeSelf;
            rightHandActive.Value = rightHand.gameObject.activeSelf;
            leftControllerActive.Value = leftController.gameObject.activeSelf;
            rightControllerActive.Value = rightController.gameObject.activeSelf;
        }
        else
        {
            ApplyPose(originBase, originBasePos.Value);
            ApplyPose(head, headPose.Value);

            ApplyPose(leftHand, leftHandPose.Value);
            ApplyPose(rightHand, rightHandPose.Value);

            ApplyPose(leftController, leftControllerPose.Value);
            ApplyPose(rightController, rightControllerPose.Value);
        }
    }

    #region Active Callbacks

    void OnLeftHandActiveChanged(bool _, bool value)
        => SetActiveSafe(leftHand, value);

    void OnRightHandActiveChanged(bool _, bool value)
        => SetActiveSafe(rightHand, value);

    void OnLeftControllerActiveChanged(bool _, bool value)
        => SetActiveSafe(leftController, value);

    void OnRightControllerActiveChanged(bool _, bool value)
        => SetActiveSafe(rightController, value);

    void SetActiveSafe(Transform t, bool active)
    {
        if (t == null) return;
        if (t.gameObject.activeSelf == active) return;
        t.gameObject.SetActive(active);
    }

    #endregion

    PoseData ToPose(Transform t) => new PoseData
    {
        position = t.localPosition,
        rotation = t.localRotation
    };

    void ApplyPose(Transform t, PoseData p)
    {
        if (!t.gameObject.activeInHierarchy) return;

        t.localPosition = Vector3.Lerp(
            t.localPosition, p.position, Time.deltaTime * smoothSpeed);

        t.localRotation = Quaternion.Slerp(
            t.localRotation, p.rotation, Time.deltaTime * smoothSpeed);
    }
}
