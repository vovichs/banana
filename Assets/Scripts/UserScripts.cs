using Steamworks;
using UnityEngine;

public class UserScripts : MonoBehaviour
{
	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

	private CallResult<HTTPRequestCompleted_t> m_HttpRequestCompleted;

	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			m_HttpRequestCompleted = CallResult<HTTPRequestCompleted_t>.Create(OnUserStart);
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			UnityEngine.Debug.Log("Steam Overlay has been activated");
		}
		else
		{
			UnityEngine.Debug.Log("Steam Overlay has been closed");
		}
	}

	private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
	{
		if ((pCallback.m_bSuccess != 1) | bIOFailure)
		{
			UnityEngine.Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
		}
		else
		{
			UnityEngine.Debug.Log("The number of players playing your game: " + pCallback.m_cPlayers.ToString());
		}
	}

	private void OnUserStart(HTTPRequestCompleted_t pCallback, bool bIOFailure)
	{
		if (!pCallback.m_bRequestSuccessful | bIOFailure)
		{
			UnityEngine.Debug.Log("There was an error retrieving the OnUserStart.");
			return;
		}
		UnityEngine.Debug.Log("Send data to Datanana!");
		UnityEngine.Debug.Log(pCallback.m_eStatusCode.ToString());
	}

	public void Start()
	{
		if (SteamManager.Initialized)
		{
			HTTPRequestHandle hRequest = SteamHTTP.CreateHTTPRequest(EHTTPMethod.k_EHTTPMethodPOST, "https://datanana.com/api/v1/banana/load");
			SteamHTTP.SetHTTPRequestHeaderValue(hRequest, "x-datanana-api-key", "czJ0t4pxTBNCMkkfQyqJJygiUcCIH1lSZAK54IVSbegpIEK9y0sPpgEyKXLFu5fl");
			CSteamID steamID = SteamUser.GetSteamID();
			SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, "steam_id", steamID.m_SteamID.ToString());
			SteamHTTP.SendHTTPRequest(hRequest, out SteamAPICall_t pCallHandle);
			m_HttpRequestCompleted.Set(pCallHandle);
		}
	}

	private void Update()
	{
	}
}
