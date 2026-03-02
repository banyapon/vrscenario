using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using Boy;
using System.Collections.Generic;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> UserName =
        new NetworkVariable<FixedString64Bytes>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        UserName.OnValueChanged += OnUserNameChanged;

        if (IsOwner)
        {
            string localName = APIManager.Instance.userEmail;
            SetUserName(localName);
        }

        if (UserName.Value.Length > 0)
        {
            OnUserNameChanged(default, UserName.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        UserName.OnValueChanged -= OnUserNameChanged;
    }

    public void SetUserName(string newName)
    {
        if (IsOwner)
        {
            Debug.Log($"newName {newName}");
            SubmitNameServerRpc(newName);
        }
    }

    [ServerRpc]
    void SubmitNameServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;

        Debug.Log($"Server mapped ClientId {senderId} to {name}");

        UserName.Value = name;

        CCTVController cCTV = CCTVController.Instance;
        if (cCTV == null) return;
        cCTV.SetUserName(senderId, name);
    }

    private void OnUserNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        Debug.Log($"Name Updated: {newValue}");
    }
}