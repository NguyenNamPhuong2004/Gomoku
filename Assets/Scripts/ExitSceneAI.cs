using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitSceneAI : MonoBehaviour
{
    private Button exitBtn;
    private void Awake()
    {
        exitBtn = GetComponent<Button>();
        exitBtn.onClick.AddListener(Exit);
    }
    private void Exit()
    {
        SceneManager.LoadScene("Menu");
    }
}
