using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Xe : MonoBehaviour
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
            else if(Gamecontroller.ins.TableChess[h, i].gameObject.tag != tag)
            {
                Gamecontroller.ins.MoveLocation[h * 10 + i].SetActive(true);
                break;
            }
            else break;
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
            else if (Gamecontroller.ins.TableChess[h, i].gameObject.tag != tag)
            {
                Gamecontroller.ins.MoveLocation[h * 10 + i].SetActive(true);
                break;
            }
            else break;
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
            else if (Gamecontroller.ins.TableChess[i, v].gameObject.tag != tag)
            {
                Gamecontroller.ins.MoveLocation[i * 10 + v].SetActive(true);
                break;
            }
            else break;
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
            else if (Gamecontroller.ins.TableChess[i, v].gameObject.tag != tag)
            {
                Gamecontroller.ins.MoveLocation[i * 10 + v].SetActive(true);
                break;
            }
            else break;
        }
    }
}
