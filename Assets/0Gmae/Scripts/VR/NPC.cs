using UnityEngine;

namespace Boy
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] Animator animator;

        public void SetFloat(string parameter, float value)
        {
            animator.SetFloat(parameter, value);
        }

        public void SetBool(string parameter, bool value)
        {
            animator.SetBool(parameter, value);
        }
    }
}
