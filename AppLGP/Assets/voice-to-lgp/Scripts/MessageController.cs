using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageController : MonoBehaviour
{
    public PhotonView photonView;
    public TMP_Text message;

    public void SendLgpMessage()
    {
        string[] split = message.text.Split('<');
        string msg = split[0];
        if (msg.Contains("Ajuda Preciso"))
        {
            msg = "Preciso de ajuda";
        }
        else if (msg.Contains("cadeira Quanto custa"))
        {
            msg = "Quanto custa esta cadeira?";
        }
        else if (msg.Contains("Lisboa ir Preciso"))
        {
            msg = "Preciso de ir a Lisboa";
        }
        else if (msg.Contains("Conhecer"))
        {
            msg = "Prazer em conhecê-lo";
        }
        photonView.RPC("ReceiveTextToSpeech", RpcTarget.Others, msg);
        message.text = "";
    }

    public void SendTextMessage(string msg)
    {
        if (msg.Contains("Quer"))
        {
                msg= msg.Replace("Quer", "Querer");
        }
        photonView.RPC("ReceiveTextToLgp", RpcTarget.Others, msg);
    }
}
