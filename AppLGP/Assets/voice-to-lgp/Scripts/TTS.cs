//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Photon.Pun;

public class TTS : MonoBehaviour
{
    // Hook up the three properties below with a Text, InputField and Button object in your UI.
    public AudioSource audioSource;
    public PhotonView photonView;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private string message;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    [PunRPC]
    public void ReceiveTextToSpeech(string message)
    {
        Debug.Log("Message rECEIVED");
        ButtonClick(message);
    }
    public void ButtonClick(string translation)
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = string.Empty;

        // Starts speech synthesis, and returns after a single utterance is synthesized.
        using (var result = synthesizer.SpeakTextAsync(translation).Result)
        {
            // Checks result.
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                // Use the Unity API to play audio here as a short term solution.
                // Native playback support will be added in the future release.
                var sampleCount = result.AudioData.Length / 2;
                var audioData = new float[sampleCount];
                for (var i = 0; i < sampleCount; ++i)
                {
                    audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                }

                // The output audio format is 16K 16bit mono
                var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
                audioClip.SetData(audioData, 0);
                audioSource.clip = audioClip;
                audioSource.Play();

                newMessage = "Speech synthesis succeeded!";
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                newMessage = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
            }
        }

        lock (threadLocker)
        {
            message = newMessage;
            waitingForSpeak = false;
        }
    }

    void Start()
    {
        
        // Continue with normal initialization, Text, InputField and Button objects are present.
        message = "Click button to synthesize speech";

        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        speechConfig = SpeechConfig.FromSubscription("e267115fea8343c6ae89aef556663aa3", "westeurope");
        speechConfig.SpeechSynthesisLanguage = "pt-PT";
        speechConfig.SpeechSynthesisVoiceName = "pt-PT-RaquelNeural";

        // The default format is Riff16Khz16BitMonoPcm.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to Raw16Khz16BitMonoPcm.
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        synthesizer = new SpeechSynthesizer(speechConfig, null);
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        synthesizer.Dispose();
    }
}

