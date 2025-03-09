using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (gameObject.activeSelf)
        {
            Gamecontroller.ins.TableChess[(int)Gamecontroller.ins.Select.transform.position.x, (int)Gamecontroller.ins.Select.transform.position.y] = null;
            Gamecontroller.ins.Select.transform.position = transform.position;
            if (Gamecontroller.ins.TableChess[(int)Gamecontroller.ins.Select.transform.position.x, (int)Gamecontroller.ins.Select.transform.position.y] != null)
            {
                Gamecontroller.ins.TableChess[(int)Gamecontroller.ins.Select.transform.position.x, (int)Gamecontroller.ins.Select.transform.position.y].gameObject.SetActive(false);
                Gamecontroller.ins.TableChess[(int)Gamecontroller.ins.Select.transform.position.x, (int)Gamecontroller.ins.Select.transform.position.y] = null;
            }
            Gamecontroller.ins.TableChess[(int)Gamecontroller.ins.Select.transform.position.x, (int)Gamecontroller.ins.Select.transform.position.y] = Gamecontroller.ins.Select.transform;
            Gamecontroller.ins.SetMoveLocation();

        }            
    }
}
