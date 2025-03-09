using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkBoardManager : NetworkBehaviour
{
    Button[,] buttons = new Button[19, 19];
    [SerializeField] private Sprite xSprite, oSprite;
    private int moveCount = 0;
    public override void OnNetworkSpawn()
    {
        var cells = GetComponentsInChildren<Button>();
        int n = 0;
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                buttons[i, j] = cells[n];
                n++;

                int r = i;
                int c = j;

                buttons[i, j].onClick.AddListener(delegate
                {
                    OnClickCell(r, c);
                });
            }
        }

    }
    private void OnClickCell(int r, int c)
    {
        var player = GameObject.FindObjectOfType<PlayerData>();
        if (player.isPlaying.Value == false) return;
        // If button clicked by host, then change button sprite as X

        if (NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 0)
        {
            buttons[r, c].GetComponent<Image>().sprite = xSprite;
            buttons[r, c].interactable = false;
            // Also change on Client side
            ChangeSpriteClientRpc(r, c);
            CheckResult(r, c);
            
            moveCount += 1;
        }

        // If button is clicked by client, then change button sprite as O

        else if (!NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 1)
        {
            buttons[r, c].GetComponent<Image>().sprite = oSprite;
            buttons[r, c].interactable = false;
            // Also change on host side
            ChangeSpriteServerRpc(r, c);
            CheckResult(r, c);
            GameManager.Instance.currentTurn.Value = 0;
            moveCount += 1;
        }

        // Make the button non interactable after clicked once


    }


    [ClientRpc]
    private void ChangeSpriteClientRpc(int r, int c)
    {
        buttons[r, c].GetComponent<Image>().sprite = xSprite;
        buttons[r, c].interactable = false;
        GameManager.Instance.currentTurn.Value = 1;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSpriteServerRpc(int r, int c)
    {
        buttons[r, c].GetComponent<Image>().sprite = oSprite;
        buttons[r, c].interactable = false;
        GameManager.Instance.currentTurn.Value = 0;

    }

    private void CheckResult(int r, int c)
    {
        if (IsWon(r, c))
        {
            GameManager.Instance.ShowMsg("won");
            GameManager.Instance.winner.Value = DataServer.ins.dts.userName;
        }
        else
        {
            if (IsGameDraw())
            {
                moveCount = 0;
                GameManager.Instance.ShowMsg("draw");
            }
        }
    }
    public bool IsWon(int x, int y)
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
            Sprite clickedButtonSprite = buttons[x, y].GetComponent<Image>().sprite;
            int count = 0;
            for (int i = 1; i < 5; i++)
            {
                int nx = x + i * dx;
                int ny = y + i * dy;
                if (nx >= 0 && nx < 19 && ny >= 0 && ny < 19 && buttons[nx, ny].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
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


    private bool IsGameDraw()
    {
        return moveCount >= 398;
    }
}
