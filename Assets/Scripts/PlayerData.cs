using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : NetworkBehaviour
{
    public Text playerNameText;
    public Text playerScoreText;

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(
        new FixedString32Bytes(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<int> playerScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<bool> isPlaying = new NetworkVariable<bool>(
        true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public override void OnNetworkSpawn()
    {
        Invoke(nameof(UpdateScore), 2);
        
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            playerNameText.text = newValue.ToString();
        };

        playerScore.OnValueChanged += (oldValue, newValue) =>
        {
            playerScoreText.text = "Score: " + newValue;
            DataServer.ins.SaveDataFn(newValue, LoginController.ins.user.UserId);
        };

        if (IsOwner) 
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log(DataServer.ins.dts.userName);
                Debug.Log(DataServer.ins.dts.elo);

                playerName.Value = DataServer.ins.dts.userName;
                playerScore.Value = DataServer.ins.dts.elo;

                Debug.Log("Host Set Data: " + playerName.Value + " - " + playerScore.Value);
            }
            else
            {
                string myName = DataServer.ins.dts.userName;
                int myElo = DataServer.ins.dts.elo;
                Debug.Log(myName);
                Debug.Log(myElo);
                UpdatePlayerInfo(myName, myElo);
            }
        }
        if (!IsHost) IsConnectedServerRpc();
        //if (NetworkManager.Singleton.ConnectedClients.Count > 2)
        //{
        //    gameObject.SetActive(false);
        //    isPlaying.Value = false;
        //}


        playerNameText.text = playerName.Value.ToString();
        playerScoreText.text = "Score: " + playerScore.Value;

        Debug.Log("UI Updated: " + playerNameText.text + " - " + playerScoreText.text);

        Invoke(nameof(ArrangeUI), 1);
    }
    [ServerRpc(RequireOwnership = false)]
    private void IsConnectedServerRpc()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > 2)
        {
            IsConnectedClientRpc();
        }
    }
    [ClientRpc]    
    private void IsConnectedClientRpc()
    {      
        gameObject.SetActive(false);
        isPlaying.Value = false;
    }

    private void UpdateScore()
    {
        GameManager.Instance.winner.OnValueChanged += (oldValue, newValue) =>
        {
            if (IsOwner) 
            {
                if (DataServer.ins.dts.userName == newValue)
                {
                    playerScore.Value += 10;
                }
                else
                {
                    playerScore.Value -= 10;
                }
            }
        };
    }
    private void UpdatePlayerInfo(FixedString32Bytes name, int elo)
    {
        Debug.Log(name);
        Debug.Log(elo);
        playerName.Value = name;
        playerScore.Value = elo;
    }

    private void ArrangeUI()
    {
        if (IsOwner)
        {
            Vector3 temp = GameObject.Find("SpawnObjPosition").transform.Find("OwnerPlayer").position;
            transform.position = new Vector3(temp.x, temp.y, 0);
        }
        else
        {
            Vector3 temp = GameObject.Find("SpawnObjPosition").transform.Find("OtherPlayer").position;
            transform.position = new Vector3(temp.x, temp.y, 0);
        }
    }
}
