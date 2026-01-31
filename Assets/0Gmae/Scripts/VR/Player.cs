using Boy;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Camera camera;

    [Header("Locomotion")]
    public GameObject turn;
    public GameObject move;
    public GameObject teleportation;
    public ClimbProvider climbProvider;
    public GameObject gravity;
    public GameObject jump;
    
    bool defaultEnableGravityOnClimbEnd;
    float defaultSlopeLimit;
    CharacterController characterController;
    [HideInInspector] public VRInput vRInput;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        vRInput = GetComponent<VRInput>();
        characterController = GetComponent<CharacterController>();
        defaultSlopeLimit = characterController.slopeLimit;
        defaultEnableGravityOnClimbEnd = climbProvider.enableGravityOnClimbEnd;
    }

    public void Teleport(Transform transform)
    {
        if (transform == null) return;
        Teleport(transform.localPosition, transform.localEulerAngles);
    }

    public void Teleport(Vector3 position, Vector3 rotate)
    {
        transform.position = position;
        transform.eulerAngles = rotate;
    }

    public void StartClimbDownSilo()
    {
        SetGravityOnClimbEnd(false);

        //SetTurn(false);
        SetMove(false);
        SetGravity(false);
    }

    public void StopClimbDownSilo()
    {
        ResetGravityOnClimbEnd();

        //SetTurn(true);
        SetMove(true);
        SetGravity(true);
    }

    public void SetGravityOnClimbEnd(bool enable)
    {
        climbProvider.enableGravityOnClimbEnd = enable;
    }

    public void ResetGravityOnClimbEnd()
    {
        climbProvider.enableGravityOnClimbEnd = defaultEnableGravityOnClimbEnd;
    }

    public void SetSlopeLimit(float value)
    {
        characterController.slopeLimit = value;
    }
    public void ResetSlopeLimit()
    {
        characterController.slopeLimit = defaultSlopeLimit;
    }

    #region Set Locomotion

    public void SetTurn(bool enable)
    {
        turn.SetActive(enable);
    }

    public void SetMove(bool enable)
    {
        move.SetActive(enable);
    }

    public void SetTeleportation(bool enable)
    {
        teleportation.SetActive(enable);
    }

    public void SetJump(bool enable)
    {
        jump.SetActive(enable);
    }

    public void SetGravity(bool enable)
    {
        gravity.SetActive(enable);
    }
    #endregion
}
