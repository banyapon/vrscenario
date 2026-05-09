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
    [SerializeField] private GameObject walkingSound;

    void Update()
    {
        if (player == null) return;
        if (Player.Instance == null) return;

        if (!VRNetworkController.Instance.inspector && !scenario.IsHost)
        {
            player.position = Player.Instance.transform.position;
        }
        else if (PlayerPrefs.GetString("PlayerRole") == "Single mode")
        {
            player.position = Player.Instance.transform.position;
        }

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
            {
                animator.SetBool(walkParam, true);
                walkingSound.SetActive(true);
            }
        }
        else
        {
            // หยุด
            if (animator != null)
            {
                animator.SetBool(walkParam, false);
                walkingSound.SetActive(false);
            }
        }
    }
}
