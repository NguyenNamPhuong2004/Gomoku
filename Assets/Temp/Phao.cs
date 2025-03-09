using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phao : MonoBehaviour
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
        Up();
        Down();
        Left();
        Right();
    }
    void Up()
    {
        for (int i = v + 1; i <= 9; i++)
        {
            if (Gamecontroller.ins.TableChess[h, i] == null)
            {
                Gamecontroller.ins.MoveLocation[h * 10 + i].SetActive(true);
            }
            else
            {
                for (int j = i+1; j <=9; j++ )
                {
                    if(Gamecontroller.ins.TableChess[h , j] != null && Gamecontroller.ins.TableChess[h, j].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[h * 10 + j].SetActive(true);
                        break;
                    }
                }
                break;
            }
        }
    }
    void Down()
    {
        for (int i = v - 1; i >= 0; i--)
        {
            if (Gamecontroller.ins.TableChess[h, i] == null)
            {
                Gamecontroller.ins.MoveLocation[h * 10 + i].SetActive(true);
            }
            else
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Gamecontroller.ins.TableChess[h, j] != null && Gamecontroller.ins.TableChess[h, j].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[h * 10 + j].SetActive(true);
                        break;
                    }
                }
                break;
            }
        }
    }
    void Left()
    {
        for (int i = h - 1; i >= 0; i--)
        {
            if (Gamecontroller.ins.TableChess[i, v] == null)
            {
                Gamecontroller.ins.MoveLocation[i * 10 + v].SetActive(true);
            }
            else
            {
                for (int j = i - 1; j >=0; j--)
                {
                    if (Gamecontroller.ins.TableChess[j, v] != null && Gamecontroller.ins.TableChess[j, v].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[j * 10 + v].SetActive(true);
                        break;
                    }
                }
                break;
            }
        }
    }
    void Right()
    {
        for (int i = h + 1; i <= 8; i++)
        {
            if (Gamecontroller.ins.TableChess[i, v] == null)
            {
                Gamecontroller.ins.MoveLocation[i * 10 + v].SetActive(true);
            }
            else
            {
                for (int j = i + 1; j <=8; j++)
                {
                    if (Gamecontroller.ins.TableChess[j, v] != null && Gamecontroller.ins.TableChess[j, v].gameObject.tag != tag)
                    {
                        Gamecontroller.ins.MoveLocation[j * 10 + v].SetActive(true);
                        break;
                    }
                }
                break;
            }
        }
    }
}
