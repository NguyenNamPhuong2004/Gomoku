using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LoginController : NetworkBehaviour
{
    public static LoginController ins;
    public GameObject login, register, forgetPassword, notification;
    public InputField loginEmail, loginPassword, registerEmail, registerPassword, registerUserName, registerConfirmPassword, forgetPasswordEmail;
    public Text notificationTitleText, notificationMessageText;
    public Toggle rememberMe;

    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;

    private void Awake()
    {
        ins = this;
        DontDestroyOnLoad(ins);
    }
    private void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            DataServer.ins.LoadDataFn(auth.CurrentUser.UserId);
            SceneManager.LoadScene(1);
        }
       
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });      
    }
    public void Login()
    {
         login.SetActive(true);
         register.SetActive(false);
         forgetPassword.SetActive(false);
    }
    public void Register()
    {
        login.SetActive(false);
        register.SetActive(true);
        forgetPassword.SetActive(false);
    }
    public void ForgetPassword()
    {
        login.SetActive(false);
        register.SetActive(false);
        forgetPassword.SetActive(true);
    }
    public void LoginUser()
    {
        if (loginEmail.text == null && loginPassword.text == null) 
        {
            ShowNotification("Error", "Fields Empty! Please Input Details In All Fields");
            return;
        }
        SignInUser(loginEmail.text, loginPassword.text);
    }
    public void RegisterUser()
    {
        if (registerEmail.text == null && registerPassword.text == null && registerUserName == null && registerConfirmPassword == null)
        {
            ShowNotification("Error", "Fields Empty! Please Input Details In All Fields");
            return;
        }
        CreateUser(registerEmail.text, registerPassword.text, registerUserName.text);
    }
    public void ForgetPasswordUser()
    {
        if (forgetPasswordEmail.text == null )
        {
            ShowNotification("Error", "Fields Empty! Please Input Details In All Fields");
            return;
        }
        ForgetPasswordSubmit(forgetPasswordEmail.text);
    }
    public void ShowNotification(string title, string message)
    {
        notificationTitleText.text = title;
        notificationMessageText.text = message;
        notification.SetActive(true);
    }
    public void CloseNotification()
    {
        notification.SetActive(false);
    }
    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    public override void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public void CreateUser(string email, string password, string userName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotification("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            UpdateUserProfile(userName);
            DataServer.ins.dts.userName = userName;
            DataServer.ins.dts.elo = 1000;
            DataServer.ins.SaveDataFn(1000, user.UserId);
        });
    }
    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotification("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            DataServer.ins.LoadDataFn(user.UserId);
        });
        
    }
    public void UpdateUserProfile(string userName)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = userName,
                PhotoUrl = new System.Uri("https://placehold.co/400"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
                ShowNotification("Alert", "Account Successfully Created");
            });
        }
    }

    bool isSigned = false;
    private void Update()
    {
        if(isSignIn == true)
        {
            if(isSigned == false)
            {
                isSigned = true;
            }
        }
    }
    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account Not Exist";
                break;
            case AuthError.MissingPassword:
                message = "Missing Password";
                break;
            case AuthError.WeakPassword:
                message = "Password So Weak";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Your Email Already In Use";
                break;
            case AuthError.InvalidEmail:
                message = "Your Email Invalid";
                break;
            case AuthError.MissingEmail:
                message = "Missing Email";
                break;
            default:
                message = "Invalid Error";
                break;
        }
        return message;
    }

    void ForgetPasswordSubmit(string forgetPasswordEmail)
    {
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
            }
            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotification("Error", GetErrorMessage(errorCode));
                    }
                } 
            }
            ShowNotification("Alert", "Successfully Send Email For Reset Password");
        });
    }
}
