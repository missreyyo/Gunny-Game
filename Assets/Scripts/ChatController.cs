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


   // [ServerRpc]
    public void AddChat(string chatString,ulong id){
        chat.Enqueue("Player "+id + ":" + chatString);
        if(chat.Count > 10) chat.Dequeue();
        chatText.text = string.Join("\n",chat);
    }
 
}
