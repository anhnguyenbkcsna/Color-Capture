using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class RpcTest : NetworkBehaviour
    {
        public TextMeshProUGUI chatLog;
        public override void OnNetworkSpawn()
        {
            if (!IsServer && IsOwner)
            { 
                if (NetworkManager.Singleton.IsClient)
                {
                    SendMessageServerRpc("Send message");
                }
            }
        }

        public void UpdateChatLog(string message)
        {
            if (message != "")
            {
                chatLog.text = message;
            }
        }
        [ClientRpc]
        public void UpdateMessageClientRpc(string message)
        {
            UpdateChatLog(message);
        }
        [ServerRpc]
        public void SendMessageServerRpc(string message)
        {
            UpdateMessageClientRpc(message);
        }
        [ClientRpc]
        public void TestClientRpc(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
            {
                TestServerRpc(value + 1, sourceNetworkObjectId);
            }
        }

        [ServerRpc]
        public void TestServerRpc(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            TestClientRpc(value, sourceNetworkObjectId);
        }
    }
}
