using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tot : MonoBehaviour
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
            if (v + 1 <= 9)
            {
                if (Gamecontroller.ins.TableChess[h, v + 1] == null || Gamecontroller.ins.TableChess[h, v + 1].gameObject.tag != tag)
                {
                    Gamecontroller.ins.MoveLocation[h * 10 + (v + 1)].SetActive(true);
                }
            }
            if (v >= 5)
            {
                if (h - 1 >= 0)
                {
                    if (Gamecontroller.ins.TableChess[h - 1, v] == null || Gamecontroller.ins.TableChess[h - 1, v].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h - 1) * 10 + v].SetActive(true);
                    }
                }
                if (h + 1 <= 8)
                {
                    if (Gamecontroller.ins.TableChess[h + 1, v] == null || Gamecontroller.ins.TableChess[h + 1, v].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h + 1) * 10 + v].SetActive(true);
                    }
                }
            }
        }
        if (gameObject.tag == "Black")
        {
            if (v -1 >= 0)
            {
                if (Gamecontroller.ins.TableChess[h, v - 1] == null || Gamecontroller.ins.TableChess[h, v - 1].gameObject.tag != tag)
                {
                    Gamecontroller.ins.MoveLocation[h * 10 + (v - 1)].SetActive(true);
                }
            }
            if (v <= 4)
            {
                if (h - 1 >= 0)
                {
                    if (Gamecontroller.ins.TableChess[h - 1, v] == null || Gamecontroller.ins.TableChess[h - 1, v].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h - 1) * 10 + v].SetActive(true);
                    }
                }
                if (h + 1 <= 8)
                {
                    if (Gamecontroller.ins.TableChess[h + 1, v] == null || Gamecontroller.ins.TableChess[h + 1, v].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[(h + 1) * 10 + v].SetActive(true);
                    }
                }
            }
        }
    }
}
