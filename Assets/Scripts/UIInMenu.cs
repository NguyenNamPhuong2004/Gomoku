using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIInMenu : MonoBehaviour
{
    public Text userName;
    public Text elo;
    public Text profileUserName, profileEmail, profileID;
    public GameObject joinRoomOption;
    public GameObject inputCode;
    public GameObject profile;

    public void OpenProfile()
    {
        profile.SetActive(true);
    }
    public void CloseProfile()
    {
        profile.SetActive(false);
    }
    public void OpenJoinRoomOption()
    {
        joinRoomOption.SetActive(true);
    } 
    public void OpenInputCode()
    {
        inputCode.SetActive(true);
    }
    public void Logout()
    {
        LoginController.ins.auth.SignOut();
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }
    private void Update()
    {
        if (LoginController.ins.user == null) return;
        userName.text = DataServer.ins.dts.userName.ToString();
        elo.text = DataServer.ins.dts.elo.ToString();
        profileUserName.text = LoginController.ins.user.DisplayName;
        profileEmail.text = LoginController.ins.user.Email;
        profileID.text = LoginController.ins.user.UserId;
    }
}
