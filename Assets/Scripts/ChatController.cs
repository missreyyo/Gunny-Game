using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ChatController : MonoBehaviour
{
    public Text chatText;
    public static ChatController instance;
    public InputField chatInputField;
    Queue<string> chat = new Queue<string>();
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
             AddChatServerRpc(Random.value.ToString());
    }
    [ServerRpc]
    public void AddChatServerRpc(string chatString){
        chat.Enqueue(chatString);
        if(chat.Count > 10) chat.Dequeue();
        chatText.text = string.Join("\n",chat);
    }
    public void SendMessageFromUI(string message){
        chatInputField.text = "";
        AddChatServerRpc(message);
        chatInputField.Select();
        
    }
}
