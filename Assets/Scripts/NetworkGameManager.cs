using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Collections;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance;
    public string joinCode;
    public NetworkVariable<FixedString32Bytes> joinCodeText = new NetworkVariable<FixedString32Bytes>("");
    private void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }



    private async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // [SerializeField] private Text joinCodeText;
    public async void StartHost()
    {
        if (NetworkManager.Singleton.IsListening)
    {
        Debug.LogWarning("Host is already running!");
        return;
    }
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.Value = new FixedString32Bytes(joinCode);
            //joinCodeText.text = joinCode;


            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started successfully, waiting for confirmation...");

                
                await Task.Run(() =>
                {
                    while (!NetworkManager.Singleton.IsListening) { }
                });

                Debug.Log("Host is now fully started! Changing scene...");

                
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("Failed to start Host!");
            }

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }



    [SerializeField] private InputField joinCodeInput;

    public async void StartClient()
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }
}