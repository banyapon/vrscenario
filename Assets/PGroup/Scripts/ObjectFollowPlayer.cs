using UnityEngine;

namespace PGroup
{
    public class ObjectFollowPlayer : MonoBehaviour
    {
        public Transform player;
        public Vector3 isFollowPlayer;
        public float smoothTime = 0.2f;
        private Vector3 velocity;
        public Vector3 offset;

        private void Start()
        {
            player = Camera.main.transform.parent.parent;
        }
        private void LateUpdate()
        {
            if (isFollowPlayer == Vector3.zero) return;

            Vector3 current = transform.position;
            Vector3 target = player.position + offset;

            Vector3 followTarget = new Vector3(
                isFollowPlayer.x == 1 ? target.x : current.x,
                isFollowPlayer.y == 1 ? target.y : current.y,
                isFollowPlayer.z == 1 ? target.z : current.z
                );

            transform.position = Vector3.SmoothDamp(
                current,
                followTarget,
                ref velocity,
                smoothTime
            );
        }
    }
}
