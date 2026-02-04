using System;
using UnityEngine;

namespace Boy
{
    public class HookController : MonoBehaviour
    {
        public Vector3 offset = new Vector3(0, 0.5f, -0.2f);
        public Hook[] hooks;

        Player player;
        Transform playerTrans;
        private void Awake()
        {
            player = Player.Instance;
            if(player) playerTrans = player.transform;
        }

        //private void OnEnable()
        //{
        //    foreach (var hook in hooks)
        //    {
        //        hook.gameObject.SetActive(true);
        //    }
        //}

        //private void OnDisable()
        //{
        //    foreach (var hook in hooks)
        //    {
        //        if (hook == null) continue;
        //        hook.gameObject.SetActive(false);
        //    }
        //}

        private void Update()
        {
            if (!CheckPlayerTransform()) return;
            transform.localPosition = playerTrans.position + playerTrans.rotation * offset;
            transform.localEulerAngles = playerTrans.eulerAngles;
        }

        public void SetHookEvent(Action action)
        {
            foreach (var hook in hooks)
            {
                hook.checker.OnEnter += action;
            }
        }

        bool CheckPlayerTransform()
        {
            if (player == null)
            {
                player = Player.Instance;
                return false;
            }

            if (playerTrans == null && player)
            {
                playerTrans = player.transform;
                return false;
            }

            return true;
        }
    }
}
