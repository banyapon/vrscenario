using PGroup;
using System;
using UnityEngine;

namespace PGroup
{
    public class Thermalscan : MonoBehaviour
    {
        public Action<GameObject, GameObject> OnEnter;
        public Action<GameObject, GameObject> OnExit;
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            OnEnter?.Invoke(gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            OnExit?.Invoke(gameObject, other.gameObject);
        }
    }
}
