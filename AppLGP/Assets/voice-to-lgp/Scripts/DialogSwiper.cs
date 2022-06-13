using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogSwiper : MonoBehaviour
{

    public TMP_Text dialog;
    public int currentDialog = 0;
    public string[] dialogs = {
        "1- Introdução Básica \n\n" +
            "<COLOR=#3000ff>NL- Olá </color>\n" +
            "L- Olá\n" +
            "<COLOR=#3000ff>NL- Como te chamas?</color>\n" +
            "L- Gil\n" +
            "<COLOR=#3000ff>NL- Eu sou a Maria</color>\n" +
            "L- Prazer em conhecê-lo/a",
        "2- Loja\n\n" +
            "L- Olá\n" +
            "<COLOR=#3000ff>NL- Olá</COLOR>\n" +
            "L- Quanto custa cadeira?\n" +
            "<COLOR=#3000ff>NL- Esta cadeira custa 50 euros</COLOR>\n" +
            "L- Obrigado\n" +
            "<COLOR=#3000ff>NL- Você precisa de mais alguma coisa?</COLOR>\n" +
            "L- Não",
        "3- Boleia\n\n" +
            "L- Bom dia\n" +
            "<COLOR=#3000ff>NL- Bom dia</color>\n" +
            "L- Como estás?\n" +
            "<COLOR=#3000ff>NL- Estou bem</color>\n" +
            "L- Preciso de ir para Lisboa\n" +
            "<COLOR=#3000ff>NL- Eu levo-te</color>\n" +
            "L- Obrigado" 
    };

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
