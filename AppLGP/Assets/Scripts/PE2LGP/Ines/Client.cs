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

public class Client : MonoBehaviour {
	public GameObject leftCanvas;
	public GameObject rightCanvas;
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
			// Debug.Log("receivedddd");
			if (serverMessage == "Erro")
			{
				// Debug.Log("erroooooo");
				text.text = "Erro a traduzir frase, tente outra.";
				//text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
				frase_pensar.gameObject.SetActive(false);
				leftCanvas.SetActive(true);
				rightCanvas.SetActive(true);
				sentence.gameObject.SetActive(true);
        		button.gameObject.SetActive(true);
				// replay_button.gameObject.SetActive(true);
				toggle.gameObject.SetActive(true);
				mouthing_toggle.gameObject.SetActive(true);
				toggle_hand.gameObject.SetActive(true);
				animator.SetBool("Pensar", false);
			}
			else{
				Animate();
			}
			received = false;
		}

        if (sent){
            sentence.gameObject.SetActive(false);
            button.gameObject.SetActive(false);
			replay_button.gameObject.SetActive(false);
			toggle.gameObject.SetActive(false);
			mouthing_toggle.gameObject.SetActive(false);
			toggle_hand.gameObject.SetActive(false);
            sent = false; 
			// animator.SetLayerWeight(animator.GetLayerIndex ("idle_animate"), 1);
			// animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 0);
			animator.SetBool("Pensar", true);
			frase_pensar.gameObject.SetActive(true);
			frase_pensar.text = sentence.text;
			//frase_pensar.rectTransform.sizeDelta = new Vector2(frase_pensar.preferredWidth, frase_pensar.preferredHeight);
        }
	} 

	public void Animate() {
		// animator.SetLayerWeight(animator.GetLayerIndex ("idle_animate"), 1);
		// animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 0);
		frase_pensar.text = "";
		sentence.text = "";
		//sentence.gameObject.SetActive(false);
		button.gameObject.SetActive(false);
		replay_button.gameObject.SetActive(false);
		toggle.gameObject.SetActive(false);
		mouthing_toggle.gameObject.SetActive(false);
		toggle_hand.gameObject.SetActive(false);
		try{
			//mainAnimation.Animate(serverMessage);
			mainAnimation.Animate(sentence.text);
		}
		catch{
			// Debug.Log("ERRO");
			text.text = "Erro a traduzir frase, tente outra.";
			//text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
			leftCanvas.SetActive(true);
			rightCanvas.SetActive(true);
			frase_pensar.gameObject.SetActive(false);
			sentence.gameObject.SetActive(true);
			button.gameObject.SetActive(true);
			// replay_button.gameObject.SetActive(true);
			toggle.gameObject.SetActive(true);
			mouthing_toggle.gameObject.SetActive(true);
			toggle_hand.gameObject.SetActive(true);
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
				// Debug.Log(www.error);
				text.text = "Servidor não está ligado";
				//text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
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

		//StartCoroutine(Upload());
		sent = true;    
	} 

	IEnumerator Upload() {
		byte[] myData = Encoding.UTF8.GetBytes(sentence.text);
		UnityWebRequest www = UnityWebRequest.Put(URL, myData);
		www.method = "POST";
		// www.method = "POST"; //hack to send POST to server instead of PUT
		// www.SetRequestHeader("Content-Type", "application/json");
		// www.SetRequestHeader("Access-Control-Expose-Headers", "Authorization, ETag");
		// www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		// www.SetRequestHeader("Access-Control-Allow-Origin", "*");
		// www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
		// www.SetRequestHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

		
		// www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
		yield return www.SendWebRequest();

		if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
			// Debug.Log(www.error);
			text.text = "Servidor não conseguiu responder";
			//text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
		}
		else {
			// Debug.Log("Client sent his message - should be received by server");
			// Debug.Log("POST successful!");
			// Debug.Log("Received: " + www.downloadHandler.text);
			serverMessage = www.downloadHandler.text;
			received = true;
		}
	}
}