using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Oculus.Interaction;
using static Oculus.Interaction.InteractableColorVisual;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
using System.Collections;
#endif

public class STT : MonoBehaviour
{
    // Hook up the two properties below with a Text and Button object in your UI.
    public TMP_Text outputText;
    public GameObject leftCanvas;
    public GameObject rightCanvas;
    //public TMP_Text errorText;
    public Button startRecoButton;
    public Button deleteButton;
    public Button sendButton;
    public Client client;
    public PhotonView photonView;
    public GameObject stopwatch;
    public MessageController messageController;
    public InteractableColorVisual colorButton;
    public string currentColor = "red";

    private object threadLocker = new object();
    private bool waitingForReco;
    private string message = string.Empty;

    private bool micPermissionGranted = false;

    bool isActive = false;

#if PLATFORM_ANDROID || PLATFORM_IOS
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    public void DeleteMessage()
    {
        if (!waitingForReco && !outputText.text.Equals(""))
        {
            message = "";
            //startRecoButton.gameObject.SetActive(true);
            sendButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
        }
    }

    public async void ButtonClick()
    {
        if (!waitingForReco && !message.Equals(""))
        {
            //client.SendMessage();
            Debug.Log("Message Sent");
            /*if (message.Contains("Quer"))
            {
                message = message.Replace("Quer", "Querer");
            }
            photonView.RPC("ReceiveTextToLgp", RpcTarget.Others, message);*/
            messageController.SendTextMessage(message);

            //outputText.gameObject.transform.parent.gameObject.SetActive(false);
            message = "";
            //startRecoButton.gameObject.SetActive(true);
            //sendButton.gameObject.SetActive(false);
            //deleteButton.gameObject.SetActive(false);
            //leftCanvas.SetActive(false);
            //rightCanvas.SetActive(false);
        }
        else
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("e267115fea8343c6ae89aef556663aa3", "westeurope");
            stopwatch.SetActive(!stopwatch.activeSelf);
            isActive = true;
            updateColor();
            // Make sure to dispose the recognizer after use!
            using (var recognizer = new SpeechRecognizer(config, "pt-PT"))
            {
                lock (threadLocker)
                {
                    waitingForReco = true;
                }

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result.
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query.
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                string newMessage = string.Empty;
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    newMessage = result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    //errorText.text = "NOMATCH: Speech could not be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    //errorText.text = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
                }

                lock (threadLocker)
                {
                    message = newMessage;
                    waitingForReco = false;
                    
                    /*Debug.Log("Message Sent");
                    photonView.RPC("ReceiveTextToLgp", RpcTarget.Others, outputText.text);
                    message = "";*/
                }
            }
        }
    }

    private void updateColor()
    {
        if(isActive){
            if (colorButton != null)
            {
                ColorState c = new ColorState();
                c.Color = Color.yellow;
                c.ColorTime = 0.1f;
                colorButton.InjectOptionalNormalColorState(c);
                c = new ColorState();
                c.Color = new Color(1, 0.92f, 0.016f, 115f / 255f);
                c.ColorTime = 0.1f;
                colorButton.InjectOptionalHoverColorState(c);
                c = new ColorState();
                c.Color = new Color(1, 0.92f, 0.016f, 36f / 255f);
                c.ColorTime = 0.05f;
                colorButton.InjectOptionalSelectColorState(c);
                colorButton.UpdateVisual();
            }
        }
        else
        {
            if (colorButton != null)
            {
                ColorState c = new ColorState();
                c.Color = currentColor.Equals("green") ? Color.green : Color.red;
                c.ColorTime = 0.1f;
                colorButton.InjectOptionalNormalColorState(c);
                c = new ColorState();
                c.Color = currentColor.Equals("green") ? new Color(0, 1, 0, 115f / 255f) : new Color(1, 0, 0, 115f / 255f);
                c.ColorTime = 0.1f;
                colorButton.InjectOptionalHoverColorState(c);
                c = new ColorState();
                c.Color = currentColor.Equals("green") ? new Color(0, 1, 0, 36f / 255f) : new Color(1, 0, 0, 36f / 255f);
                c.ColorTime = 0.05f;
                colorButton.InjectOptionalSelectColorState(c);
                colorButton.UpdateVisual();
            }
        }
    }

    void Start()
    {
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else if (startRecoButton == null)
        {
            message = "startRecoButton property is null! Assign a UI Button to it.";
            UnityEngine.Debug.LogError(message);
        }
        else
        {
            // Continue with normal initialization, Text and Button objects are present.
#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            message = "Waiting for mic permission";
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#else
            micPermissionGranted = true;
#endif
           // startRecoButton.onClick.AddListener(ButtonClick);
            sendButton.onClick.AddListener(ButtonClick);
            deleteButton.onClick.AddListener(DeleteMessage);
        }
        outputText.text = string.Empty;
    }

    void Update()
    {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "";
        }
#endif

        lock (threadLocker)
        {
            if (startRecoButton != null)
            {
                //startRecoButton.interactable = !waitingForReco && micPermissionGranted;
                if (waitingForReco)
                {
                    //startRecoButton.GetComponentInChildren<Text>().text = "A reconhecer ...";
                }
                else if (!waitingForReco && message.Equals(""))
                {
                    if (stopwatch.activeSelf)
                    {
                        stopwatch.SetActive(!stopwatch.activeSelf);
                    }
                    isActive = false;
                    updateColor();
                   // startRecoButton.GetComponentInChildren<Text>().text = "Começar";
                    //startRecoButton.gameObject.SetActive(true);
                    sendButton.gameObject.SetActive(false);
                    deleteButton.gameObject.SetActive(false);
                }
                else {
                    if (outputText != null)
                    {
                        outputText.text += message + "\n";
                    }
                    /*startRecoButton.GetComponentInChildren<Text>().text = "Enviar";
                    startRecoButton.gameObject.SetActive(false);
                    sendButton.gameObject.SetActive(true);
                    deleteButton.gameObject.SetActive(true);*/
                    ButtonClick();
                }
            }
        }
    }
}