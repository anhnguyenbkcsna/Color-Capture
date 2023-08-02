using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class RpcTest : NetworkBehaviour
    {
        public TextMeshProUGUI chatLog;
        public string MessageName = "MessageChannel";
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer && IsOwner)
            { 
                if (NetworkManager.Singleton.IsClient)
                {
                    // SendMessageServerRpc("Send message");
                }
            }
        }

        #region Custom named message
        // private void OnClientConnectedCallback(ulong obj)
        // {
        //     SendMessage(Guid.NewGuid());
        // }
        // private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
        // {
        //     var receivedMessageContent = new ForceNetworkSerializeByMemcpy<Guid>(new Guid());
        //     messagePayload.ReadValueSafe(out receivedMessageContent);   
        //     if (IsServer)
        //     {
        //         Debug.Log($"Sever received GUID ({receivedMessageContent.Value}) from client ({senderId})");
        //     }
        //     else
        //     {
        //         Debug.Log($"Client received GUID ({receivedMessageContent.Value}) from the server.");
        //     }
        // }
        //
        // private void SendMessage(Guid inGameId)
        // {
        //     var messageContent = new ForceNetworkSerializeByMemcpy<Guid>(inGameId);
        //     var writer = new FastBufferWriter(1100, Allocator.Temp);
        //     var customMessagingManager = NetworkManager.CustomMessagingManager;
        //     using (writer)
        //     {
        //         writer.WriteValueSafe(messageContent);
        //         if (IsServer)
        //         {
        //             // This is a server-only method that will broadcast the named message.
        //             // Caution: Invoking this method on a client will throw an exception!
        //             customMessagingManager.SendNamedMessageToAll(MessageName, writer);
        //         }
        //         else
        //         {
        //             // This is a client or server method that sends a named message to one target destination
        //             // (client to server or server to client)
        //             customMessagingManager.SendNamedMessage(MessageName, NetworkManager.ServerClientId, writer);
        //         }
        //     }
        // }
        #endregion
        private void UpdateChatLog(string message)
        {
            // Parse
            if (message != "")
            {
                chatLog.text = chatLog.text + "\n" + message;
            }
        }
        [ClientRpc]
        private void UpdateMessageClientRpc(string message)
        {
            UpdateChatLog(message);
        }
        [ServerRpc]
        public void SendMessageServerRpc(string message)
        {
            UpdateMessageClientRpc(message);
        }
        [ClientRpc]
        private void TestClientRpc(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
            {
                TestServerRpc(value + 1, sourceNetworkObjectId);
            }
        }

        [ServerRpc]
        private void TestServerRpc(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            TestClientRpc(value, sourceNetworkObjectId);
        }
    }
}
