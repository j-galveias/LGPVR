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
        "1- Introdu��o B�sica \n\n" +
            "<COLOR=#3000ff>NL- Ol� </color>\n" +
            "L- Ol�\n" +
            "<COLOR=#3000ff>NL- Eu sou *NOME*. Qual � o teu nome?</color>\n" +
            "L- *NOME*\n" +
            "<COLOR=#3000ff>NL-  Qual � a tua idade?</color>\n" +
            "L- *IDADE*\n" +
            "<COLOR=#3000ff>NL-  Adeus</color>\n" +
            "L- Prazer em conhec�-lo/a",
        "2- Loja\n\n" +
            "L- Ol�\n" +
            "<COLOR=#3000ff>NL- Ol�</COLOR>\n" +
            "L- Preciso de ajuda\n" +
            "<COLOR=#3000ff>NL- Como posso ajudar?</COLOR>\n" +
            "L- Quanto custa cadeira?\n" +
            "<COLOR=#3000ff>NL- 18 euros</COLOR>\n" +
            "L- Obrigado\n" +
            "<COLOR=#3000ff>NL-Vai querer comprar?</COLOR>\n" +
            "L- N�o",
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
