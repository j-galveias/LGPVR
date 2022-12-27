using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using TMPro;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using Photon.Pun;

public class Client : MonoBehaviour {
	public GameObject leftCanvas;
	public GameObject rightCanvas;
	public GameObject cylinder;
	public GameObject mirror;
	public string error = null;
	Animator animator;
	public Toggle toggle;
	public Toggle mouthing_toggle;
	public Toggle toggle_hand;
	public GameObject replay_button;
    public TMP_Text sentence;
	public TMP_Text frase_pensar;
    public TMP_Text text;
    public Button button;
	public GameObject character;
	public PhotonView photonView;
	#region private members 	
    private bool socketReady;
    private bool received;
    private bool sent;
    private string serverMessage;
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
	private MainAnimation mainAnimation;
	private string URL = "https://www.hlt.inesc-id.pt/tradutor"; // https://www.hlt.inesc-id.pt/tradutor http://3.15.150.72:49152
	#endregion  	
	// Use this for initialization 	
	void Start () {
		mainAnimation = character.GetComponent<MainAnimation>();
		animator = character.GetComponent<Animator>();
		replay_button.gameObject.SetActive(false);
		StartCoroutine(ConnectToServer());
	}  	
	// Update is called once per frame
	void Update () {
        if (socketReady)
            button.interactable = true; 
        else
            button.interactable = false; 

        if (received){
			if (serverMessage == "Erro")
			{
				text.text = "Erro a traduzir frase, tente outra.";
				frase_pensar.gameObject.SetActive(false);
				
				cylinder.SetActive(true);
				mirror.SetActive(true);
				animator.SetBool("Pensar", false);
			}
			else{
				Animate();
			}
			received = false;
		}

        if (sent){
			cylinder.SetActive(false);
			mirror.SetActive(false);
			
            sent = false; 
			
			animator.SetBool("Pensar", true);
			frase_pensar.gameObject.SetActive(true);
			frase_pensar.text = sentence.text;
        }
	}

	[PunRPC]
	public void ReceiveTextToLgp(string message)
    {
		Debug.Log("Message rECEIVED");
		sentence.text = message;
		SendMessage();
    }

	public void Animate() {
		
		frase_pensar.text = "";
		sentence.text = "";
		
		cylinder.SetActive(false);
		mirror.SetActive(false);
		try
		{
			mainAnimation.Animate(serverMessage);
		}
		catch{
			text.text = "Erro a traduzir frase, tente outra.";
			
			frase_pensar.gameObject.SetActive(false);
			
			cylinder.SetActive(true);
			mirror.SetActive(true);
			animator.SetBool("Pensar", false);
		}
	}

	/// <summary> 	
	/// Check server connection --> GET request		
	/// </summary> 	
	private IEnumerator ConnectToServer () {		

			UnityWebRequest www = UnityWebRequest.Get(URL);

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				text.text = "Servidor não está ligado";
			}
			else
			{
				Debug.Log("sever is connected");
				socketReady = true;
			}
	}  

	/// <summary> 	
	/// Send message to server using http put request.	
	/// </summary> 	
	public void SendMessage() { 
		text.text = "";

		StartCoroutine(Upload());
		sent = true;    
	} 

	IEnumerator Upload() {
		byte[] myData = Encoding.UTF8.GetBytes(sentence.text.Substring(0, 1) + sentence.text.Substring(1).ToLower());
		UnityWebRequest www = UnityWebRequest.Put(URL, myData);
		www.method = "POST";
		
		yield return www.SendWebRequest();

		if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
			text.text = "Servidor não conseguiu responder";
		}
		else {
			
			serverMessage = www.downloadHandler.text;
			received = true;
		}
	}
}