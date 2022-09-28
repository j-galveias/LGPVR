using Oculus.Interaction;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Oculus.Interaction.InteractableColorVisual;

public class GestureTypeManager : MonoBehaviour
{
    public GameObject estaticos;
    public GameObject dinamicos;
    public GameObject confirmarDinamicos;
    public GameObject ultimoDinamico;
    public GameObject ultimoEstatico;
    public Toggle toggle;
    public PhotonView photonView;
    public TMP_Text message;
    public TMP_Text buttonText;
    public InteractableColorVisual colorButton;

    public void SendLgpMessage()
    {
        photonView.RPC("ReceiveTextToSpeech", RpcTarget.Others, message.text);
        message.text = "";
    }

    public void ToggleClick()
    {
        if (toggle.isOn)
        {
            estaticos.SetActive(false);
            dinamicos.SetActive(true);
            confirmarDinamicos.SetActive(true);
            ultimoDinamico.SetActive(true);
            ultimoEstatico.SetActive(false);
        }
        else
        {
            estaticos.SetActive(true);
            dinamicos.SetActive(false);
            confirmarDinamicos.SetActive(false);
            ultimoDinamico.SetActive(false);
            ultimoEstatico.SetActive(true);
        }
    }

    public void ButtonClick()
    {
        if (!confirmarDinamicos.activeSelf)
        {
            buttonText.text = "Dinâmicos";
            buttonText.color = Color.black;
            ColorState c = new ColorState();
            c.Color = Color.cyan;
            c.ColorTime = 0.1f;
            colorButton.InjectOptionalNormalColorState(c);
            c = new ColorState();
            c.Color = new Color(0, 1, 1, 115f / 255f);
            c.ColorTime = 0.1f;
            colorButton.InjectOptionalHoverColorState(c);
            c = new ColorState();
            c.Color = new Color(0, 1, 1, 36f / 255f);
            c.ColorTime = 0.05f;
            colorButton.InjectOptionalSelectColorState(c);
        }
        else
        {
            buttonText.text = "Estáticos";
            buttonText.color = Color.white;
            ColorState c = new ColorState();
            c.Color = Color.blue;
            c.ColorTime = 0.1f;
            colorButton.InjectOptionalNormalColorState(c);
            c = new ColorState();
            c.Color = new Color(0, 0, 1, 115f / 255f);
            c.ColorTime = 0.1f;
            colorButton.InjectOptionalHoverColorState(c);
            c = new ColorState();
            c.Color = new Color(0, 0, 1, 36f / 255f);
            c.ColorTime = 0.05f;
            colorButton.InjectOptionalSelectColorState(c);
        }
        estaticos.SetActive(!estaticos.activeSelf);
        dinamicos.SetActive(!dinamicos.activeSelf);
        confirmarDinamicos.SetActive(!confirmarDinamicos.activeSelf);
        //ultimoDinamico.SetActive(!ultimoDinamico.activeSelf);
        //ultimoEstatico.SetActive(!ultimoEstatico.activeSelf);
    }
}
