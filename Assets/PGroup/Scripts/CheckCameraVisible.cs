using UnityEngine;

namespace PGroup
{
    public class CheckMainCameraVisible : MonoBehaviour
    {
        [SerializeField] private HighGroundGameplay gameplay;
        Renderer rend;
        Camera cam;
        bool isDone;

        void Start()
        {
            rend = GetComponent<Renderer>();
            cam = Camera.main;
        }

        void Update()
        {
            bool visible = IsVisibleFromMainCamera();
            if (visible && !isDone)
            {
                isDone = true;
                gameplay.LookAtAccident();
            }
        }

        bool IsVisibleFromMainCamera()
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(rend.bounds.center);

            if (viewportPos.z < 0) return false;
            if (viewportPos.x < 0 || viewportPos.x > 1) return false;
            if (viewportPos.y < 0 || viewportPos.y > 1) return false;

            Ray ray = new Ray(cam.transform.position,
                              rend.bounds.center - cam.transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
                return hit.transform == transform;

            return false;
        }
    }
}
