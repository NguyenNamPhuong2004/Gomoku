using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameController : NetworkBehaviour
{
    private GameObject button;
    public GameObject buttonPrefab;
    public GameObject gridPanel;
    private int[,] board;
    private int boardSize = 19;
    public NetworkVariable<int> playerSide = new NetworkVariable<int>(0);
    private NetworkVariable<int> moveCount = new NetworkVariable<int>(0);

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            Debug.Log("Client with id " + clientId + " joined");
            if (NetworkManager.Singleton.IsHost &&
            NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                CreateBoard();
            }
        };
    }
    /*public override void OnNetworkSpawn()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                button.GetComponent<NetworkCell>().SetGameControllerReference(this);
                button.GetComponent<NetworkCell>().SetPosition(i, j);
            }
        }
    }*/

    void CreateBoard()
    {
        board = new int[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                button = Instantiate(buttonPrefab);
                button.GetComponent<NetworkObject>().Spawn();
                button.transform.SetParent(gridPanel.transform,false);
                button.GetComponent<NetworkCell>().SetGameControllerReference(this);
                button.GetComponent<NetworkCell>().SetPosition(i, j);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(int x, int y)
    {
        board[x, y] = playerSide.Value;
        moveCount.Value++;

        if (CheckWin(x, y))
        {
            GameOver();
        }
        else if (moveCount.Value >= boardSize * boardSize)
        {
            GameOver();
        }
        else
        {
            ChangeSides();
        }
    }

    void ChangeSides()
    {
        playerSide.Value = (playerSide.Value == 0) ? 1 : 0;
    }

    bool CheckWin(int x, int y)
    {
        return CheckDirection(x, y, 1, 0) || CheckDirection(x, y, 0, 1) ||
               CheckDirection(x, y, 1, 1) || CheckDirection(x, y, 1, -1);
    }

    bool CheckDirection(int x, int y, int dx, int dy)
    {
        int count = 1;
        count += CountInDirection(x, y, dx, dy);
        count += CountInDirection(x, y, -dx, -dy);
        return count >= 5;
    }

    int CountInDirection(int x, int y, int dx, int dy)
    {
        int count = 0;
        for (int i = 1; i < 5; i++)
        {
            int nx = x + i * dx;
            int ny = y + i * dy;
            if (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize && board[nx, ny] == playerSide.Value)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    void GameOver()
    {
        Debug.Log("Game Over! " + playerSide.Value + " wins!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartGameServerRpc()
    {
        playerSide.Value = 0;
        moveCount.Value = 0;
        board = new int[boardSize, boardSize];
        foreach (Transform child in gridPanel.transform)
        {
            child.GetComponent<Button>().interactable = true;
            child.GetComponentInChildren<Text>().text = "";
        }
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
