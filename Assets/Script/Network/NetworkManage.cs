using System;
using Unity.Netcode;
using UnityEngine;
using TMPro;

namespace Network
{
    public class NetworkManage : MonoBehaviour
    {
        public TMP_InputField inputField;
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }

            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            {
                // Spawn map
                GUILayout.Button("Test button");
            }

            GUILayout.EndArea();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                var message = inputField.text;
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<RpcTest>().SendMessageServerRpc(inputField.text);
                // Debug.Log(inputField.text);
            }
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }
    }
}