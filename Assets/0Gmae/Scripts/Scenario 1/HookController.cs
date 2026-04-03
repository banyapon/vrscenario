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

            InitRopeMaterials();
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
            InitRopeMaterials();
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
            InitRopeMaterials();
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

        void InitRopeMaterials()
        {
            if (ropeMaterials.Count != 0) return;
            ropeMaterials.Clear();

            foreach (var r in ropeRenderers)
            {
                if (r == null) continue;

                Material matInstance = new Material(r.material);

                r.material = matInstance;

                ropeMaterials.Add(matInstance);

                originalColor = matInstance.color;
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
        private void OnDestroy()
        {
            foreach (var mat in ropeMaterials)
            {
                if (mat != null) Destroy(mat);
            }

            ropeMaterials.Clear();
        }
    }
}
