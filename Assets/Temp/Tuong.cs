using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuong : MonoBehaviour
{ 
    int h;
    int v;
    private void Update()
    {
        h = (int)gameObject.transform.position.x;
        v = (int)gameObject.transform.position.y;
    }
    private void OnMouseDown()
    {
        Gamecontroller.ins.SetMoveLocation();
        Gamecontroller.ins.Select = gameObject;
        Move();
    }
    void Move()
    {
        if (gameObject.tag == "Red")
        {
            if (v + 2 <= 4)
            {
                if (h - 2 >= 0)
                {
                    if (Gamecontroller.ins.TableChess[h - 1, v + 1] == null && Gamecontroller.ins.TableChess[h - 2, v + 2] == null || Gamecontroller.ins.TableChess[h - 2, v + 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h - 2) * 10 + (v + 2)].SetActive(true);
                    }
                }
                if (h + 2 <= 8)
                {
                    if (Gamecontroller.ins.TableChess[h + 1, v + 1] == null && Gamecontroller.ins.TableChess[h + 2, v + 2] == null || Gamecontroller.ins.TableChess[h + 2, v + 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h + 2) * 10 + (v + 2)].SetActive(true);
                    }
                }
            }
            if (v - 2 >= 0)
            {
                if (h - 2 >= 0)
                {
                    if (Gamecontroller.ins.TableChess[h - 1, v - 1] == null && Gamecontroller.ins.TableChess[h - 2, v - 2] == null || Gamecontroller.ins.TableChess[h - 2, v - 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h - 2) * 10 + (v - 2)].SetActive(true);
                    }
                }
                if (h + 2 <= 8)
                {
                    if (Gamecontroller.ins.TableChess[h + 1, v - 1] == null && Gamecontroller.ins.TableChess[h + 2, v - 2] == null || Gamecontroller.ins.TableChess[h + 2, v - 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h + 2) * 10 + (v - 2)].SetActive(true);
                    }
                }
            }
        }
        if (gameObject.tag == "Black")
        {
            if (v + 2 <= 9)
            {
                if (h - 2 >= 0)
                {
                    if (Gamecontroller.ins.TableChess[h - 1, v + 1] == null && Gamecontroller.ins.TableChess[h - 2, v + 2] == null || Gamecontroller.ins.TableChess[h - 2, v + 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h - 2) * 10 + (v + 2)].SetActive(true);
                    }
                }
                if (h + 2 <= 8)
                {
                    if (Gamecontroller.ins.TableChess[h + 1, v + 1] == null && Gamecontroller.ins.TableChess[h + 2, v + 2] == null || Gamecontroller.ins.TableChess[h + 2, v + 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h + 2) * 10 + (v + 2)].SetActive(true);
                    }
                }
            }
            if (v - 2 >= 5)
            {
                if (h - 2 >= 0)
                {
                    if (Gamecontroller.ins.TableChess[h - 1, v - 1] == null && Gamecontroller.ins.TableChess[h - 2, v - 2] == null || Gamecontroller.ins.TableChess[h - 2, v - 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h - 2) * 10 + (v - 2)].SetActive(true);
                    }
                }
                if (h + 2 <= 8)
                {
                    if (Gamecontroller.ins.TableChess[h + 1, v - 1] == null && Gamecontroller.ins.TableChess[h + 2, v - 2] == null || Gamecontroller.ins.TableChess[h + 2, v - 2].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h + 2) * 10 + (v - 2)].SetActive(true);
                    }
                }
            }
        }
    }
}
