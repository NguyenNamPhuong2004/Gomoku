using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);
    public NetworkVariable<FixedString32Bytes> winner = new NetworkVariable<FixedString32Bytes>("");  
    [SerializeField] private GameObject joinCodePrefab;
    private GameObject newJoinCode;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
       if(NetworkManager.Singleton.IsHost) SpawnJoinCode();
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
        //playerDataList.Add(new PlayerData(DataServer.ins.dts.userName, DataServer.ins.dts.elo));
        //NoticeJoinRoomClientRpc(NetworkManager.LocalClientId);
        //Debug.Log($"Player {NetworkManager.LocalClientId} join room");

    }


    private void OnPlayerDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            LeaveMatch();
        }
    }

    private void OnPlayerConnected(ulong clientId)
    {
        Debug.Log("Client with id " + clientId + " joined");
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            SpawnBoard();
        }
        
    }
    private void SpawnJoinCode()
    {
        newJoinCode = Instantiate(joinCodePrefab);
        newJoinCode.GetComponent<NetworkObject>().Spawn();
    }
    public void LeaveMatch()
    {

        if (NetworkManager.Singleton.IsHost) 
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
    //private void OnPlayerDisconnected(ulong clientId)
    //{
    //    if (IsServer)
    //    {
    //        RemovePlayerData(clientId);
    //    }
    //}
    //private void LoadPlayerDataFromFirebase(ulong clientId)
    //{
    //    string userId = clientId.ToString();


    //    databaseReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
    //        if (task.IsFaulted)
    //        {
    //            Debug.LogError($"Firebase error: {task.Exception}");
    //            return;
    //        }

    //        if (task.IsCompleted)
    //        {
    //            DataSnapshot snapshot = task.Result;

    //            if (snapshot.Exists)
    //            {

    //                string playerName = snapshot.Child("userName").Value.ToString();
    //                ulong eloRating = (ulong)snapshot.Child("elo").Value;
    //                AddOrUpdatePlayerData(clientId, playerName, eloRating);
    //            }
    //        }
    //    });
    //}


    //private void AddOrUpdatePlayerData(ulong clientId, string playerName, ulong eloRating)
    //{
    //    bool playerExists = false;


    //    for (int i = 0; i < playerDataList.Count; i++)
    //    {
    //        if (playerDataList[i].id == clientId)
    //        {

    //            PlayerData updatedData = playerDataList[i];
    //            updatedData.userName = new FixedString32Bytes(playerName);
    //            updatedData.elo = eloRating;
    //            playerDataList[i] = updatedData;
    //            playerExists = true;
    //            break;
    //        }
    //    }


    //    if (!playerExists)
    //    {
    //        PlayerData newPlayerData = new PlayerData
    //        {
    //            id = clientId,
    //            userName = new FixedString32Bytes(playerName),
    //            elo = eloRating
    //        };
    //        playerDataList.Add(newPlayerData);
    //    }
    //}
    //private void RemovePlayerData(ulong clientId)
    //{
    //    for (int i = 0; i < playerDataList.Count; i++)
    //    {
    //        if (playerDataList[i].id == clientId)
    //        {
    //            playerDataList.RemoveAt(i);
    //            break;
    //        }
    //    }
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void RequestPlayerDataServerRpc(ulong clientId)
    //{
    //    LoadPlayerDataFromFirebase(clientId);
    //}

    [SerializeField] private GameObject boardPrefab;
    private GameObject newBoard;
    private void SpawnBoard()
    {
        newBoard = Instantiate(boardPrefab);
        newBoard.GetComponent<NetworkObject>().Spawn();
    }

    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private Text msgText;

    public void ShowMsg(string msg)
    {
        if (msg.Equals("won"))
        {
            msgText.text = "You Won";
            gameEndPanel.SetActive(true);
            // Show Panel with text that Opponent Won
            ShowOpponentMsg("You Lose");
        }
        else if (msg.Equals("draw"))
        {
            msgText.text = "Game Draw";
            gameEndPanel.SetActive(true);
            ShowOpponentMsg("Game Draw");
        }
    }


    private void ShowOpponentMsg(string msg)
    {
        if (IsHost)
        {
            // Then use ClientRpc to show Message at Client Side
            OpponentMsgClientRpc(msg);
        }
        else
        {
            // Use ServerRpc to show message at Server Side
            OpponentMsgServerRpc(msg);
        }
    }

    [ClientRpc]
    private void OpponentMsgClientRpc(string msg)
    {
        if (IsHost) return;
        msgText.text = msg;
        gameEndPanel.SetActive(true);

    }


    [ServerRpc(RequireOwnership = false)]
    private void OpponentMsgServerRpc(string msg)
    {
        msgText.text = msg;
        gameEndPanel.SetActive(true);
    }



    public void Restart()
    {
        // If this is client, then call SererRpc to destroy current board and create new board
        // If this is client then Client will also call ServerRpc to hide result panel on host side

        if (!IsHost)
        {
            RestartServerRpc();
            gameEndPanel.SetActive(false);
        }
        else
        {
            Destroy(newBoard);
            SpawnBoard();
            RestartClientRpc();
        }

        // Destroy the current Game Board
        // Spawn a new board
        // Hide the Result Panel
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc()
    {
        Destroy(newBoard);
        SpawnBoard();
        gameEndPanel.SetActive(false);
    }


    [ClientRpc]
    private void RestartClientRpc()
    {
        gameEndPanel.SetActive(false);
    }
}