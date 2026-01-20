using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boy
{
    public class GrabInState : MonoBehaviour
    {
        public Transform[] grabObjectList;
        State state;
        List<GrabParentData> grabParentDatas = new List<GrabParentData>();

        private void Awake()
        {
            state = GetComponent<State>();
            if (state == null) return;

            foreach (var grab in grabObjectList)
            {
                if (grab == null) continue;

                GrabParentData grabParentData = new GrabParentData();
                grabParentData.parent = grab.parent;
                grabParentData.grab = grab;
                grabParentDatas.Add(grabParentData);
            }

            state.onExit += ResetParent;
        }

        private void OnDestroy()
        {
            if (state == null) return;
            state.onExit -= ResetParent;
        }

        void ResetParent()
        {
            foreach (var data in grabParentDatas)
            {
                if (data == null) continue;
                if (data.grab == null) continue;

                data.grab.SetParent(data.parent);
            }
        }
    }

    [Serializable]
    public class GrabParentData
    {
        public Transform parent;
        public Transform grab;
    }
}
