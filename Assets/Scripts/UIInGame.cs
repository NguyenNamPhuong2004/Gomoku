using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : NetworkBehaviour
{
    public Text myUserName;
    public Text myElo;
    public Text otherUserName;
    public Text otherElo;

    private void Update()
    {
        myUserName.text = LoginController.ins.user.DisplayName;
        myElo.text = DataServer.ins.dts.elo.ToString();
    }
}
