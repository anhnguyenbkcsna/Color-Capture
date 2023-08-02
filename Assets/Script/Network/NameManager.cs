using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class NameManager : MonoBehaviour
    {
        public TMP_InputField inputField;
        public Button btn;
        public TextMeshProUGUI welcomeText;
        private void Update()
        {
            if (inputField.text != "" && Input.GetKeyDown(KeyCode.Return))
            {
                OnButtonClick();
            }
        }

        public void OnButtonClick()
        {
            PlayerPrefs.SetString("Name", inputField.text);
            // Get the player's name = PlayerPrefs.GetString("Name");
            inputField.gameObject.SetActive(false);
            btn.gameObject.SetActive(false);
            welcomeText.text = "Welcome " + inputField.text + "!";
            welcomeText.gameObject.SetActive(true);
        }   
    }
}