using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BratusSteamLibraries
{
	public class SteamLeaderboard : MonoBehaviour
	{
		[SerializeField]
		private static string m_leaderboardName;

		private static int m_leaderboardCount;

		private static SteamLeaderboard_t m_steamLeaderBoard;

		private static SteamLeaderboardEntries_t m_leaderboardEntries;

		private static List<LeaderboardCell> leaderboardPlayersList = new List<LeaderboardCell>();

		private static List<GameObject> leaderboardCellList = new List<GameObject>();

		[SerializeField]
		private static int m_leaderboardEntriesRangeInit;

		[SerializeField]
		private static int m_leaderboardEntriesRangeEnd;

		[SerializeField]
		public static Transform m_Leaderboardtarget;

		private static GameObject m_leaderboardPrefab;

		private static float m_distanceBetweenCells;

		private static bool m_leaderboardInitiated;

		private static bool m_usingentriesFinished;

		private static ELeaderboardUploadScoreMethod m_uploadScoreMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;

		private static ELeaderboardDataRequest m_requestType = ELeaderboardDataRequest.k_ELeaderboardDataRequestUsers;

		private static CallResult<LeaderboardFindResult_t> m_leaderboardFindResult = new CallResult<LeaderboardFindResult_t>();

		private static CallResult<LeaderboardScoreUploaded_t> m_uploadResult = new CallResult<LeaderboardScoreUploaded_t>();

		private static CallResult<LeaderboardScoresDownloaded_t> m_scoresDownloadedResult = new CallResult<LeaderboardScoresDownloaded_t>();

		[Tooltip("teste")]
		public string leaderboardName;

		public int leaderboardEntriesRangeinit;

		public int leaderboardEntriesRangeEnd;

		public Transform leaderboardTarget;

		public GameObject leaderboardPrefab;

		public float distanceBetweenCells;

		public ELeaderboardUploadScoreMethod uploadScoreMethod;

		public ELeaderboardDataRequest requestType;

		private void Awake()
		{
			m_leaderboardName = leaderboardName;
			m_leaderboardEntriesRangeInit = leaderboardEntriesRangeinit;
			m_leaderboardEntriesRangeEnd = leaderboardEntriesRangeEnd;
			m_leaderboardPrefab = leaderboardPrefab;
			m_distanceBetweenCells = distanceBetweenCells;
			m_uploadScoreMethod = uploadScoreMethod;
			m_requestType = requestType;
			m_usingentriesFinished = false;
		}

		private void OnEnable()
		{
			bool initialized = SteamManager.Initialized;
		}

		private void Start()
		{
			FindLeaderboard();
		}

		public static void InstantiateLeaderboard(MonoBehaviour _coroutineStart)
		{
			_coroutineStart.StartCoroutine(LeaderboardCreation());
		}

		private static IEnumerator LeaderboardCreation()
		{
			if (!m_Leaderboardtarget)
			{
				yield return null;
				yield break;
			}
			DownloadLeaderboardEntries();
			yield return new WaitForSeconds(1f);
			UseDownloadedEntries();
			InstantiateNewLeaderboardCell();
		}

		public static void FindLeaderboard()
		{
			SteamAPICall_t hAPICall = SteamUserStats.FindLeaderboard(m_leaderboardName);
			m_leaderboardFindResult.Set(hAPICall, OnLeaderBoardFindResult);
		}

		public static void DownloadLeaderboardEntries()
		{
			if (!m_leaderboardInitiated)
			{
				UnityEngine.Debug.Log("The leaderboard was not found!");
				return;
			}
			SteamAPICall_t hAPICall = SteamUserStats.DownloadLeaderboardEntries(m_steamLeaderBoard, m_requestType, m_leaderboardEntriesRangeInit, m_leaderboardEntriesRangeEnd);
			m_scoresDownloadedResult.Set(hAPICall, OnLeaderBoardScoresDownloaded);
		}

		public static void UseDownloadedEntries()
		{
			leaderboardPlayersList.Clear();
			for (int i = 0; i < m_leaderboardCount; i++)
			{
				SteamUserStats.GetDownloadedLeaderboardEntry(m_leaderboardEntries, i, out LeaderboardEntry_t pLeaderboardEntry, null, 0);
				UnityEngine.Debug.Log("Score: " + pLeaderboardEntry.m_nScore.ToString() + " User ID: " + SteamFriends.GetFriendPersonaName(pLeaderboardEntry.m_steamIDUser));
				Sprite sprite = FetchAvatar(pLeaderboardEntry.m_steamIDUser);
				if (!sprite)
				{
					sprite = FetchAvatar(pLeaderboardEntry.m_steamIDUser);
				}
				leaderboardPlayersList.Insert(i, new LeaderboardCell(sprite, SteamFriends.GetFriendPersonaName(pLeaderboardEntry.m_steamIDUser), pLeaderboardEntry.m_nScore));
			}
		}

		public static void InstantiateNewLeaderboardCell()
		{
			for (int i = 0; i < leaderboardPlayersList.Count; i++)
			{
				GameObject gameObject = (!m_leaderboardPrefab) ? null : UnityEngine.Object.Instantiate(m_leaderboardPrefab);
				gameObject.transform.parent = m_Leaderboardtarget.transform;
				Vector2 v = (i != 0) ? new Vector2(leaderboardCellList[i - 1].transform.localPosition.x, leaderboardCellList[i - 1].transform.localPosition.y - m_distanceBetweenCells) : new Vector2(0f, 0f);
				gameObject.transform.localPosition = v;
				leaderboardCellList.Insert(i, gameObject);
				Image componentInChildren = leaderboardCellList[i].transform.GetComponentInChildren<Image>();
				Text[] componentsInChildren = leaderboardCellList[i].transform.GetComponentsInChildren<Text>();
				componentInChildren.sprite = leaderboardPlayersList[i].playerImage;
				componentsInChildren[0].text = leaderboardPlayersList[i].playerName;
				componentsInChildren[1].text = leaderboardPlayersList[i].playerScore.ToString();
			}
		}

		public static bool UpdateScore(int _score)
		{
			if (!m_leaderboardInitiated)
			{
				UnityEngine.Debug.Log("!!!!!!!! The Leaderboard was not found! !!!!!!!");
				return false;
			}
			SteamAPICall_t hAPICall = SteamUserStats.UploadLeaderboardScore(m_steamLeaderBoard, m_uploadScoreMethod, _score, null, 0);
			m_uploadResult.Set(hAPICall, OnLeaderBoardUploadResult);
			return true;
		}

		public static Sprite FetchAvatar(CSteamID _steamID)
		{
			Rect rect = new Rect(0f, 0f, 184f, 184f);
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			int largeFriendAvatar = SteamFriends.GetLargeFriendAvatar(_steamID);
			if (largeFriendAvatar == -1)
			{
				for (int i = 0; i <= 2000; i++)
				{
					UnityEngine.Debug.Log("avatar not found");
				}
			}
			if (largeFriendAvatar > 0)
			{
				SteamUtils.GetImageSize(largeFriendAvatar, out uint pnWidth, out uint pnHeight);
				if (pnWidth != 0 && pnHeight != 0)
				{
					byte[] array = new byte[4 * pnWidth * pnHeight];
					SteamUtils.GetImageRGBA(largeFriendAvatar, array, (int)(4 * pnWidth * pnHeight));
					Texture2D texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false);
					texture2D.LoadRawTextureData(array);
					texture2D.Apply();
					return Sprite.Create(texture2D, rect, pivot);
				}
			}
			return null;
		}

		private static void OnLeaderBoardFindResult(LeaderboardFindResult_t _callback, bool _IOFailure)
		{
			UnityEngine.Debug.Log("STEAM LEADERBOARDS: Found - " + _callback.m_bLeaderboardFound.ToString() + " leaderboardID - " + _callback.m_hSteamLeaderboard.m_SteamLeaderboard.ToString());
			m_steamLeaderBoard = _callback.m_hSteamLeaderboard;
			m_leaderboardInitiated = true;
		}

		private static void OnLeaderBoardUploadResult(LeaderboardScoreUploaded_t _callback, bool _IOFailure)
		{
			UnityEngine.Debug.Log("STEAM LEADERBOARDS: failure - " + _IOFailure.ToString() + " Completed - " + _callback.m_bSuccess.ToString() + " NewScore: " + _callback.m_nGlobalRankNew.ToString() + " Score: " + _callback.m_nScore.ToString() + " HasChanged - " + _callback.m_bScoreChanged.ToString());
		}

		private static void OnLeaderBoardScoresDownloaded(LeaderboardScoresDownloaded_t _callback, bool _IOFailure)
		{
			m_leaderboardEntries = _callback.m_hSteamLeaderboardEntries;
			m_leaderboardCount = _callback.m_cEntryCount;
			string[] obj = new string[6]
			{
				"Leaderboard: ",
				null,
				null,
				null,
				null,
				null
			};
			SteamLeaderboard_t hSteamLeaderboard = _callback.m_hSteamLeaderboard;
			obj[1] = hSteamLeaderboard.ToString();
			obj[2] = " Entries: ";
			SteamLeaderboardEntries_t hSteamLeaderboardEntries = _callback.m_hSteamLeaderboardEntries;
			obj[3] = hSteamLeaderboardEntries.ToString();
			obj[4] = "Count: ";
			obj[5] = _callback.m_cEntryCount.ToString();
			UnityEngine.Debug.Log(string.Concat(obj));
		}

		private void Update()
		{
			SteamAPI.RunCallbacks();
		}
	}
}
