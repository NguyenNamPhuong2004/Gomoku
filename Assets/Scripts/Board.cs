using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject cellPrefab;
    public Sprite xSprite;
    public Sprite oXprite;
    public Transform board;
    public GridLayoutGroup gridLayout;
    public string currentTurn = "x";

    public int boardSize;

    private void Start()
    {
        gridLayout.constraintCount = boardSize;
        CreateBoard();
    }
    private void CreateBoard()
    {
        for( int i = 0; i < boardSize; i++)
        {
            for ( int j = 0; j < boardSize; j++)
            {
                Instantiate(cellPrefab, board);
            }
        }
    }
}
