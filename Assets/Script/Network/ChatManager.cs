// using System;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
//
// namespace Network
// {
//     public class ChatManager : MonoBehaviour
//     {
//         public TextMeshProUGUI chatHistory;
//         public TMP_InputField inputField;
//         public void Update()
//         {
//
//             if(Input.GetKey(KeyCode.Return))
//             {
//                 SetChat();
//             }
//         }
//
//         private void SetChat()
//         {
//             chatHistory.text = inputField.text;
//         }
//     }
// }