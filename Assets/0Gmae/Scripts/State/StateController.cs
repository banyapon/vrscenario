using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace Boy
{
    public class StateController : NetworkBehaviour
    {
        bool isStateChanging;

        public int currentState = -1;

        [Header("States")]
        public List<State> states = new List<State>();

        Scenario scenario;

        // ===== Network =====
        NetworkVariable<int> syncedState =
            new NetworkVariable<int>(
                -1,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner
            );

        #region Unity Lifecycle

        private void Awake()
        {
            foreach (var state in states)
            {
                if (!state) continue;
                state.gameObject.SetActive(true);
                state.gameObject.SetActive(false);
            }

            scenario = GetComponent<Scenario>();
        }

        public override void OnNetworkSpawn()
        {
            syncedState.OnValueChanged += OnSyncedStateChanged;

            if (IsOwner) ApplyState(0);
        }

        public override void OnNetworkDespawn()
        {
            syncedState.OnValueChanged -= OnSyncedStateChanged;
        }

        private void Update()
        {
            if (CheckIndexValid(currentState))
                GetState(currentState).StateUpdate();
        }

        #endregion

        #region Network Sync

        void OnSyncedStateChanged(int oldValue, int newValue)
        {
            if (IsOwner) return;

            ApplyState(newValue);
        }

        bool CanWriteNet()
        {
            return IsOwner &&
                   IsSpawned &&
                   NetworkManager != null &&
                   NetworkManager.IsListening;
        }

        void WriteNetState()
        {
            if (CanWriteNet())
                syncedState.Value = currentState;
        }

        #endregion

        #region State Core Logic

        public State GetState(int index)
        {
            return states[index];
        }

        public bool CheckIndexValid(int index)
        {
            return index >= 0 && index < states.Count;
        }

        void ApplyState(int stateIndex)
        {
            if (currentState == stateIndex) return;

            if (CheckIndexValid(currentState))
                GetState(currentState).StateExit();

            currentState = stateIndex;

            if (CheckIndexValid(currentState))
                GetState(currentState).StateEnter();

            WriteNetState();
        }

        #endregion

        #region Public Controls (VR Only)

        public void GotoState(State targetState)
        {
            if (!IsOwner) return;

            int targetIndex = -1;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].gameObject == targetState.gameObject)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex == -1)
            {
                Debug.LogWarning("State not found");
                return;
            }

            GotoState(targetIndex);
        }

        public void GotoState(int stateIndex)
        {
            if (!IsOwner) return;

            ApplyState(stateIndex);
        }

        public void NextState(float waitTime = 0)
        {
            if (!IsOwner) return;
            if (isStateChanging) return;

            StartCoroutine(NextStateLogic(waitTime));
        }

        IEnumerator NextStateLogic(float waitTime)
        {
            isStateChanging = true;

            yield return new WaitForSeconds(waitTime);

            int next = currentState + 1;
            if (next >= states.Count)
                RestartScene();
            else
                GotoState(next);

            yield return new WaitForSeconds(0.1f);
            isStateChanging = false;
        }

        public void BackState(float waitTime = 0)
        {
            if (!IsOwner) return;
            if (isStateChanging) return;

            StartCoroutine(BackStateLogic(waitTime));
        }

        IEnumerator BackStateLogic(float waitTime)
        {
            isStateChanging = true;

            yield return new WaitForSeconds(waitTime);

            int next = currentState - 1;
            if (states.Count > 0 && next >= 0)
                GotoState(next);

            yield return new WaitForSeconds(0.1f);
            isStateChanging = false;
        }

        public void RestartScene()
        {
            GotoState(0);
        }

        public void BackToLobby()
        {
            if (!IsOwner) return;

            scenario.RequestDestroy();
        }

        #endregion


        public void OwnerReportStateValue(State state, bool pass, bool first)
        {
            if (!IsOwner) return;

            int index = states.IndexOf(state);
            if (index < 0) return;

            ReportStateValueServerRpc(index, pass, first);
        }

        [ServerRpc]
        private void ReportStateValueServerRpc(int index, bool pass, bool first)
        {
            states[index].ApplyValue(pass, first);
        }
    }
}
