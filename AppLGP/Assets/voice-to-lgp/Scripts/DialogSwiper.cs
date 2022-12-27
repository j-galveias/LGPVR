using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class DialogSwiper : MonoBehaviour
{

    public TMP_Text dialog;
    public int currentDialog = 0;
    [ReadOnly]
    public string[] dialogs = {
        "1- Introdução Básica \n\n" +
            "<COLOR=#3000ff>NL- Olá </color>\n" +
            "L- Olá\n" +
            "<COLOR=#3000ff>NL- Eu sou *NOME*. Qual é o teu nome?</color>\n" +
            "L- *NOME*\n" +
            "<COLOR=#3000ff>NL-  Qual é a tua idade?</color>\n" +
            "L- *IDADE*\n" +
            "<COLOR=#3000ff>NL-  Adeus</color>\n" +
            "L- Prazer em conhecê-lo/a",
        "2- Loja\n\n" +
            "L- Olá\n" +
            "<COLOR=#3000ff>NL- Olá</COLOR>\n" +
            "L- Preciso de ajuda\n" +
            "<COLOR=#3000ff>NL- Como posso ajudar?</COLOR>\n" +
            "L- Quanto custa cadeira?\n" +
            "<COLOR=#3000ff>NL- 18 euros</COLOR>\n" +
            "L- Obrigado\n" +
            "<COLOR=#3000ff>NL-Vai querer comprar?</COLOR>\n" +
            "L- Não",
        "3- Boleia\n\n" +
            "<COLOR=#3000ff>NL- Bom dia\n</color>" +
            "L- Bom dia\n" +
            "<COLOR=#3000ff>NL- Onde vais?\n</color>" +
            "L- Preciso de ir para Lisboa\n" +
            "<COLOR=#3000ff>NL- Posso te acompanhar?</color>\n" +
            "L- Sim"
    };

    private void Start()
    {
        dialog.text = dialogs[0];
    }

    public void nextDial()
    {
        currentDialog += 1;
        dialog.text = dialogs[currentDialog % dialogs.Length];
    }

    public void lastDial()
    {
        currentDialog -= 1;
        if(currentDialog < 0)
        {
            currentDialog = 2;
        }
        dialog.text = dialogs[currentDialog % dialogs.Length];
    }
}
