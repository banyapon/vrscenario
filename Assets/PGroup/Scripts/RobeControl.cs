using Obi;
using System.Collections;
using UnityEngine;

namespace PGroup
{
    public class RobeControl : MonoBehaviour
    {
        [SerializeField] private Transform startFollow;
        [SerializeField] private Transform endFollow;

        [SerializeField] private Transform start;
        [SerializeField] private Transform end;

        private bool following;

        private void OnEnable()
        {
            StartCoroutine(DelayStart());
        }
        private void OnDisable()
        {
            following = false;
        }
        private void Update()
        {
            if (!following) return;
            if (startFollow == null || endFollow == null) return;
            if (startFollow.gameObject.activeInHierarchy == false || endFollow.gameObject.activeInHierarchy == false) return;
            start.position = startFollow.position;
            end.position = endFollow.position;
        }

        private IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(.1f);
            following = true;
        }
    }
}
