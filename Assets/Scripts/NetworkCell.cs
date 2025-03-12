using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCell : NetworkBehaviour
{
    public Button button;
    public Text buttonText;
    private NetworkGameController gameController;
    public int x, y;

    public override void OnNetworkSpawn()
    {
        button.onClick.AddListener(SetSpace);
    }
    public void SetGameControllerReference(NetworkGameController controller)
    {
        gameController = controller;
    }

    public void SetPosition(int posX, int posY)
    {
        x = posX;
        y = posY;
    }

    private void SetSpace()
    {
        if (NetworkManager.Singleton.IsHost && gameController.playerSide.Value == 0)
        {
            gameController.EndTurnServerRpc(x, y);
            buttonText.text = "X";
            button.interactable = false;
        }
        else if(!NetworkManager.Singleton.IsHost && gameController.playerSide.Value == 1)
        {
            gameController.EndTurnServerRpc(x, y);
            buttonText.text = "O";
            button.interactable = false;
        }
    }

    
}
