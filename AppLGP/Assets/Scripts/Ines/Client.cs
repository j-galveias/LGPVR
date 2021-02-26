using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour {
	Animator animator;
	public GameObject replay_button;
    public InputField sentence;
	public Text frase_pensar;
    public Text text;
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
	#endregion  	
	// Use this for initialization 	
	void Start () {
		mainAnimation = character.GetComponent<MainAnimation>();
		animator = character.GetComponent<Animator>();
		ConnectToTcpServer();  
	}  	
	// Update is called once per frame
	void Update () {
        if (!string.IsNullOrEmpty(sentence.text) && socketReady)
            button.interactable = true; 
        else
            button.interactable = false; 

        if (received){
            // text.text = serverMessage;
			// text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
			if (serverMessage == "Erro a traduzir frase, tente outra.")
			{
				text.text = serverMessage;
				text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
				sentence.gameObject.SetActive(true);
        		button.gameObject.SetActive(true);
				replay_button.gameObject.SetActive(true);
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
            sent = false; 
			animator.SetLayerWeight(animator.GetLayerIndex ("idle_pensar"), 1);
			animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 0);
			animator.SetBool("Pensar", true);
			frase_pensar.text = sentence.text;
			frase_pensar.rectTransform.sizeDelta = new Vector2(frase_pensar.preferredWidth, frase_pensar.preferredHeight);
        }
	} 

	public void Animate() {
		frase_pensar.text = "";
		sentence.text = "";
		sentence.gameObject.SetActive(false);
		button.gameObject.SetActive(false);
		mainAnimation.Animate(serverMessage);
	}

	void OnApplicationQuit()
	{
		try
		{
			NetworkStream stream = socketConnection.GetStream(); 
			byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes("closing"); 				
			// Write byte array to socketConnection stream.                 
			stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
			clientReceiveThread.Abort();
			socketConnection.Close();
			Debug.Log("closeddd");
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
		}
	}

	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer () { 		
		try {  	
			socketConnection = new TcpClient("3.139.233.62", 8000);
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();
			socketReady = true;	
		} 		
		catch (Exception e) {
			ConnectToTcpServer();
			Debug.Log("On client connect exception " + e);	
		} 	
	}  
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try { 			
			Byte[] bytes = new Byte[1024];             
			while (true) {
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) {
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
                        var incommingData = new byte[length]; 						
						Array.Copy(bytes, 0, incommingData, 0, length); 						
						// Convert byte array to string message. 						
						serverMessage = Encoding.UTF8.GetString(incommingData); 						
						Debug.Log("server message received as: " + serverMessage);
                        received = true;
					}
				}
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public void SendMessage() { 
		text.text = "";        
		if (socketConnection == null) {             
			return;         
		}  		
		try {
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				string clientMessage = sentence.text; 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");
                sent = true;     
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	} 
}