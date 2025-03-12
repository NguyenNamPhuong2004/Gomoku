using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinCode : NetworkBehaviour
{
    public Text playerNameText;
    public override void OnNetworkSpawn()
    {
        playerNameText.text = "Room code : " + NetworkGameManager.Instance.joinCodeText.Value;
        Vector3 temp = GameObject.Find("SpawnObjPosition").transform.Find("Joincode").position;
        transform.position = new Vector3(temp.x, temp.y, 0);
    }
}
