using System;
using UnityEngine;

namespace PGroup
{
    public class PlaceObject : MonoBehaviour
    {
        public string triggerTag = "Hand";
        public GameObject triggerTarget;

        public Action<PlaceObject, GameObject> OnEnter;
        public Action<PlaceObject, GameObject> OnExit;

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (IsValidTarget(other)) OnEnter?.Invoke(this,other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            if (IsValidTarget(other)) OnExit?.Invoke(this, other.gameObject);
        }

        private bool IsValidTarget(Collider other)
        {
            if (triggerTarget != null)
            {
                return other.gameObject == triggerTarget;
            }

            if (!string.IsNullOrEmpty(triggerTag))
            {
                return other.CompareTag(triggerTag);
            }

            return true;
        }
    }
}
