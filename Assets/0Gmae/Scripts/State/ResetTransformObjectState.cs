using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boy
{
    public class ResetTransformObjectState : MonoBehaviour
    {
        public Transform[] targetList;
        State state;
        List<ResetTransformData> resetTransformDatas = new List<ResetTransformData>();

        private void Awake()
        {
            state = GetComponent<State>();
            if (state == null) return;

            foreach (var t in targetList)
            {
                if (t == null) continue;

                ResetTransformData resetTransformData = new ResetTransformData();

                resetTransformData.target = t;
                resetTransformData.position = t.localPosition;
                resetTransformData.eulerAngles = t.localEulerAngles;
                resetTransformData.scale = t.localScale;

                resetTransformDatas.Add(resetTransformData);
            }

            state.onExit += ResetTransform;
        }

        private void OnDestroy()
        {
            if (state == null) return;
            state.onExit -= ResetTransform;
        }

        void ResetTransform()
        {
            foreach (var data in resetTransformDatas)
            {
                if (data == null) continue;
                if (data.target == null) continue;

                data.target.localPosition = data.position;
                data.target.localEulerAngles = data.eulerAngles;
                data.target.localScale = data.scale;
            }
        }
    }

    [Serializable]
    public class ResetTransformData
    {
        public Transform target;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
    }
}

