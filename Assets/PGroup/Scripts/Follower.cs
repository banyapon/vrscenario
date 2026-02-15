using PGroup;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public GameplayController gameplayController;

    [Header("Follow Setting")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;   // ระยะที่หยุด

    [Header("Animation")]
    public Animator animator;
    public string walkParam = "isWalk"; // bool ใน Animator

    private void Start()
    {
        player = gameplayController.player;
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
