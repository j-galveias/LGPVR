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

public class Client : MonoBehaviour {
	Animator animator;
	public Toggle toggle;
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
	private string URL = "http://3.139.64.204:80"; // https://www.hlt.inesc-id.pt/tradutor http://3.15.150.72:49152
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
        if (!string.IsNullOrEmpty(sentence.text) && socketReady)
            button.interactable = true; 
        else
            button.interactable = false; 

        if (received){
			Debug.Log("receivedddd");
			if (serverMessage == "Erro")
			{
				Debug.Log("erroooooo");
				text.text = "Erro a traduzir frase, tente outra.";
				text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
				frase_pensar.gameObject.SetActive(false);
				sentence.gameObject.SetActive(true);
        		button.gameObject.SetActive(true);
				// replay_button.gameObject.SetActive(true);
				toggle.gameObject.SetActive(true);
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
            sent = false; 
			// animator.SetLayerWeight(animator.GetLayerIndex ("idle_animate"), 1);
			// animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 0);
			animator.SetBool("Pensar", true);
			frase_pensar.gameObject.SetActive(true);
			frase_pensar.text = sentence.text;
			frase_pensar.rectTransform.sizeDelta = new Vector2(frase_pensar.preferredWidth, frase_pensar.preferredHeight);
        }
	} 

	public void Animate() {
		// animator.SetLayerWeight(animator.GetLayerIndex ("idle_animate"), 1);
		// animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 0);
		frase_pensar.text = "";
		sentence.text = "";
		sentence.gameObject.SetActive(false);
		button.gameObject.SetActive(false);
		replay_button.gameObject.SetActive(false);
		toggle.gameObject.SetActive(false);
		mainAnimation.Animate(serverMessage);
	}

	/// <summary> 	
	/// Check server connection --> GET request		
	/// </summary> 	
	private IEnumerator ConnectToServer () {		

			UnityWebRequest www = UnityWebRequest.Get(URL);

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log(www.error);
				text.text = "Servidor não está ligado";
				text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
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

		if(www.result == UnityWebRequest.Result.ConnectionError) {
			Debug.Log(www.error);
			text.text = "Servidor não conseguiu responder";
			text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
		}
		else {
			Debug.Log("Client sent his message - should be received by server");
			Debug.Log("POST successful!");
			Debug.Log("Received: " + www.downloadHandler.text);
			serverMessage = www.downloadHandler.text;
			received = true;
		}
	}
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //Simply return true no matter what
        return true;
    }
} 

class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
{
    // Encoded RSAPublicKey
    private static string PUB_KEY = "MIIDCzCCAfOgAwIBAgIUBHXWqXgGseaND7ny0RINKTtt980wDQYJKoZIhvcNAQEL"+
									"BQAwFTETMBEGA1UECAwKU29tZS1TdGF0ZTAeFw0yMTAzMDkxMTAxMDNaFw0yMjAz"+
									"MDkxMTAxMDNaMBUxEzARBgNVBAgMClNvbWUtU3RhdGUwggEiMA0GCSqGSIb3DQEB"+
									"AQUAA4IBDwAwggEKAoIBAQDQJP8PJrb1eo6mM+DSIp8HD2CUQEzI2Lo/MAJLB1iK"+
									"f9bVghClgcAUhRjZajjnYwbK62+Ucn4RCEt42KOujTSD9oPRFwsM8ghcmHkhhSwe"+
									"RyxjvEzVYmPTIbaZSFWx3ZeqQ8pMc9QijfDldk6jM0VHFL+GnNmGq67Zypk7F3pE"+
									"y9YPVSAYk7KMcPGPtDX/gMBRqhbf623l41RN9KVphUOvPiIlK35ZZd/Fur9G8Uxs"+
									"mMRYEtaLkJtd2VJkF0B8q6SJin3MbDj4iu+jk/ERsa0Jy6n+Zx0iJCzRcaBHzVkC"+
									"6KL/d4envYNxrZIHP98AaexAp41RIeQ0xw6csdQqViqPAgMBAAGjUzBRMB0GA1Ud"+
									"DgQWBBSfVJAvu86vgog15qZQTtkZbQpuBDAfBgNVHSMEGDAWgBSfVJAvu86vgog1"+
									"5qZQTtkZbQpuBDAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBCwUAA4IBAQAm"+
									"ZYanwQPn3pUq65cruVGpVoydvPNcvnYqhNnq7icxSkD+wkFaIqk8/BqyUm2kxs0F"+
									"entac9XdujvXOAqECtjFzIeJ8uHbtu3mDvxJ/E62lxVPArnVPEuOc70VCC+tN+Ql"+
									"p/SsPm3/Aj4GdjvZExjIIdlW/EwOcSMdaepiXliOom+/g0flw4+LolcjdC50JHiw"+
									"1UybiTkwWVrto3s8b8FfCy8DFDbSHLr7TpLF2JbNmwN/vN64DeKGrR46sZyKMBzy"+
									"x9dwz64Q6TvrdUj6r49epne38FC64bDlejjnV3minqxM8JblZOZzoajaBfd3Y7pf"+
									"hpQWVAsdenBIHjzZcwph";

    protected override bool ValidateCertificate(byte[] certificateData)
    {
		Debug.Log(certificateData);
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();
        if (pk.Equals(PUB_KEY))
            return true;

        // Bad dog
        return false;
    }
}