using UnityEngine;

namespace Boy
{
    public class ClearRigidbodyState : MonoBehaviour
    {
        public Rigidbody[] rigidbodyList;
        State state;

        private void Awake()
        {
            state = GetComponent<State>();
            if (state == null) return;
            state.onExit += Clear;
        }

        private void OnDestroy()
        {
            if (state == null) return;
            state.onExit -= Clear;
        }

        void Clear()
        {
            foreach (var rb in rigidbodyList)
            {
                if (rb == null) continue;
                if (rb.isKinematic) continue;

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
