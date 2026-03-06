using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using Boy;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<bool> _isInspector = new NetworkVariable<bool>(
    false,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);

    public bool IsInspector
    {
        get => _isInspector.Value;
        set => _isInspector.Value = value;
    }

    //public NetworkVariable<FixedString64Bytes> UserName =
    //    new NetworkVariable<FixedString64Bytes>(
    //        default,
    //        NetworkVariableReadPermission.Everyone,
    //        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        _isInspector.OnValueChanged += OnRoleChanged;

        //UserName.OnValueChanged += OnUserNameChanged;

        //if (IsOwner)
        //{
        //    string localName = APIManager.Instance.userEmail;
        //    SetUserName(localName);
        //}

        //if (UserName.Value.Length > 0)
        //{
        //    OnUserNameChanged(default, UserName.Value);
        //}
    }

    public override void OnNetworkDespawn()
    {
        _isInspector.OnValueChanged -= OnRoleChanged;
        //UserName.OnValueChanged -= OnUserNameChanged;
    }
    void OnRoleChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Inspector: {newValue}");
    }

    //public void SetUserName(string newName)
    //{
    //    if (IsOwner)
    //    {
    //        Debug.Log($"newName {newName}");
    //        SubmitNameServerRpc(newName);
    //    }
    //}

    //[ServerRpc]
    //void SubmitNameServerRpc(string name, ServerRpcParams rpcParams = default)
    //{
    //    ulong senderId = rpcParams.Receive.SenderClientId;

    //    Debug.Log($"Server mapped ClientId {senderId} to {name}");

    //    UserName.Value = name;

    //    CCTVController cCTV = CCTVController.Instance;
    //    if (cCTV == null) return;
    //    cCTV.SetUserName(senderId, name);
    //}

    //private void OnUserNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    //{
    //    Debug.Log($"Name Updated: {newValue}");
    //}
}