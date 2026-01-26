using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Locomotion")]
    public GameObject turn;
    public GameObject move;
    public GameObject teleportation;
    public ClimbProvider climbProvider;
    public GameObject gravity;
    public GameObject jump;
    
    bool defaultEnableGravityOnClimbEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
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
        climbProvider.enableGravityOnClimbEnd = false;

        //SetTurn(false);
        SetMove(false);
        SetGravity(false);
    }

    public void StopClimbDownSilo()
    {
        climbProvider.enableGravityOnClimbEnd = defaultEnableGravityOnClimbEnd;

        //SetTurn(true);
        SetMove(true);
        SetGravity(true);
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
