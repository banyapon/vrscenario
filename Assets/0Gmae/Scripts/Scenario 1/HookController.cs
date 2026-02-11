using Obi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boy
{
    public class HookController : MonoBehaviour
    {
        public Vector3 offset = new Vector3(0, 0.5f, -0.2f);
        public Hook[] hooks;
        public ObiRopeExtrudedRenderer[] ropeRenderers;

        List<Material> ropeMaterials = new();
        Player player;
        Transform playerTrans;
        Color originalColor;
        private void Awake()
        {
            player = Player.Instance;
            if(player) playerTrans = player.transform;

            SetRopeMaterials();
        }

        private void OnEnable()
        {
            foreach (var hook in hooks)
            {
                hook.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var hook in hooks)
            {
                if (hook == null) continue;
                hook.gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            SetRopeMaterials();
            foreach (var hook in hooks)
            {
                if (hook == null) continue;
                hook.ShowModel();
            }

            originalColor.a = 1;
            foreach (var r in ropeMaterials)
            {
                r.color = originalColor;
            }
        }
        public void Hide()
        {
            SetRopeMaterials();
            foreach (var hook in hooks)
            {
                if (hook == null) continue;
                hook.HideModel();
            }

            originalColor.a = 0;
            foreach (var r in ropeMaterials)
            {
                r.color = originalColor;
            }
        }

        public void SetRopeMaterials()
        {
            if (ropeMaterials.Count != 0) return;
            foreach (var r in ropeRenderers)
            {
                ropeMaterials.Add(r.material);
                originalColor = r.material.color;
            }
        }

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
