using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamecontroller : MonoBehaviour
{
    public static Gamecontroller ins;
    public Transform[,] TableChess = new Transform[10, 10];
    public List<GameObject> Chess = new List<GameObject>();
    public GameObject moveLocation;
    public List<GameObject> MoveLocation;
    public GameObject Select;
    private void Awake()
    {
        ins = this;
    }
    private void Start()
    {
        InitBoardChess();
    }
    void InitBoardChess()
    {
        // Red
        TableChess[0, 0] = Instantiate(Chess[0], new Vector2(0, 0), Quaternion.identity).transform;
        TableChess[8, 0] = Instantiate(Chess[0], new Vector2(8, 0), Quaternion.identity).transform;
        TableChess[1, 2] = Instantiate(Chess[1], new Vector2(1, 2), Quaternion.identity).transform;
        TableChess[7, 2] = Instantiate(Chess[1], new Vector2(7, 2), Quaternion.identity).transform;
        TableChess[1, 0] = Instantiate(Chess[2], new Vector2(1, 0), Quaternion.identity).transform;
        TableChess[7, 0] = Instantiate(Chess[2], new Vector2(7, 0), Quaternion.identity).transform;
        TableChess[2, 0] = Instantiate(Chess[3], new Vector2(2, 0), Quaternion.identity).transform;
        TableChess[6, 0] = Instantiate(Chess[3], new Vector2(6, 0), Quaternion.identity).transform;
        TableChess[3, 0] = Instantiate(Chess[4], new Vector2(3, 0), Quaternion.identity).transform;
        TableChess[5, 0] = Instantiate(Chess[4], new Vector2(5, 0), Quaternion.identity).transform;
        TableChess[4, 0] = Instantiate(Chess[5], new Vector2(4, 0), Quaternion.identity).transform;
        TableChess[0, 3] = Instantiate(Chess[6], new Vector2(0, 3), Quaternion.identity).transform;
        TableChess[2, 3] = Instantiate(Chess[6], new Vector2(2, 3), Quaternion.identity).transform;
        TableChess[4, 3] = Instantiate(Chess[6], new Vector2(4, 3), Quaternion.identity).transform;
        TableChess[6, 3] = Instantiate(Chess[6], new Vector2(6, 3), Quaternion.identity).transform;
        TableChess[8, 3] = Instantiate(Chess[6], new Vector2(8, 3), Quaternion.identity).transform;
        // Black
        TableChess[0, 9] = Instantiate(Chess[7], new Vector2(0, 9), Quaternion.identity).transform;
        TableChess[8, 9] = Instantiate(Chess[7], new Vector2(8, 9), Quaternion.identity).transform;
        TableChess[1, 7] = Instantiate(Chess[8], new Vector2(1, 7), Quaternion.identity).transform;
        TableChess[7, 7] = Instantiate(Chess[8], new Vector2(7, 7), Quaternion.identity).transform;
        TableChess[1, 9] = Instantiate(Chess[9], new Vector2(1, 9), Quaternion.identity).transform;
        TableChess[7, 9] = Instantiate(Chess[9], new Vector2(7, 9), Quaternion.identity).transform;
        TableChess[2, 9] = Instantiate(Chess[10], new Vector2(2, 9), Quaternion.identity).transform;
        TableChess[6, 9] = Instantiate(Chess[10], new Vector2(6, 9), Quaternion.identity).transform;
        TableChess[3, 9] = Instantiate(Chess[11], new Vector2(3, 9), Quaternion.identity).transform;
        TableChess[5, 9] = Instantiate(Chess[11], new Vector2(5, 9), Quaternion.identity).transform;
        TableChess[4, 9] = Instantiate(Chess[12], new Vector2(4, 9), Quaternion.identity).transform;
        TableChess[0, 6] = Instantiate(Chess[13], new Vector2(0, 6), Quaternion.identity).transform;
        TableChess[2, 6] = Instantiate(Chess[13], new Vector2(2, 6), Quaternion.identity).transform;
        TableChess[4, 6] = Instantiate(Chess[13], new Vector2(4, 6), Quaternion.identity).transform;
        TableChess[6, 6] = Instantiate(Chess[13], new Vector2(6, 6), Quaternion.identity).transform;
        TableChess[8, 6] = Instantiate(Chess[13], new Vector2(8, 6), Quaternion.identity).transform;
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 10; j++)
            {
                MoveLocation.Add(Instantiate(moveLocation, new Vector3Int(i, j), Quaternion.identity));
            }
        SetMoveLocation();
    }
    public void SetMoveLocation()
    {
        for (int i = 0; i < 90; i++) MoveLocation[i].SetActive(false);
    }
}
