using PGroup;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public GameplayController gameplayController;

    [Header("Follow Setting")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;   // ระยะที่หยุด

    [Header("Animation")]
    public Animator animator;
    public string walkParam = "isWalk"; // bool ใน Animator

    [SerializeField] private Scenario scenario;

    private void Start()
    {
        if (!VRNetworkController.Instance.inspector && !scenario.IsHost)
        {
            player = gameplayController.player;
            scenario.SetFollower(player);
        }
    }
    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // เดินตาม
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            dir.y = 0;
            dir = dir.normalized;

            // หันหน้าไปหาผู้เล่น
            if (dir != Vector3.zero)
                transform.forward = dir;

            // เปิดอนิเมชั่นเดิน
            if (animator != null)
                animator.SetBool(walkParam, true);
        }
        else
        {
            // หยุด
            if (animator != null)
                animator.SetBool(walkParam, false);
        }
    }
}
