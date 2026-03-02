using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Boy
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] Animator animator;

        public void SetFloat(string parameter, float value)
        {
            animator.SetFloat(parameter, value);
        }

        public float GetFloat(string parameter)
        {
            return animator.GetFloat(parameter);
        }

        public void SetBool(string parameter, bool value)
        {
            animator.SetBool(parameter, value);
        }
    }
}
