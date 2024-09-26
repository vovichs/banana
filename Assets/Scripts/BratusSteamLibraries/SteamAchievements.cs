using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BratusSteamLibraries
{
	public class SteamAchievements : MonoBehaviour
	{
		public int quantityOfAchievements;

		private static bool m_storeStats;

		private static CSteamID m_steamID;

		public static List<Achievements> listOfAchievements = new List<Achievements>();

		private CallResult<UserStatsReceived_t> m_userStatsReceived = new CallResult<UserStatsReceived_t>();

		private CallResult<UserStatsStored_t> m_userStatsStored = new CallResult<UserStatsStored_t>();

		private CallResult<UserAchievementStored_t> m_achievementStored = new CallResult<UserAchievementStored_t>();

		private void Awake()
		{
			listOfAchievements.Clear();
			for (int i = 0; i <= quantityOfAchievements; i++)
			{
				if (i < 10)
				{
					listOfAchievements.Insert(i, new Achievements("ACHIEVEMENT_0" + i.ToString(), "", "", _Achieved: false, 0));
				}
				if (i >= 10)
				{
					listOfAchievements.Insert(i, new Achievements("ACHIEVEMENT_" + i.ToString(), "", "", _Achieved: false, 0));
				}
			}
		}

		private void OnEnable()
		{
			if (SteamManager.Initialized)
			{
				m_steamID = SteamUser.GetSteamID();
				CSteamID steamID = m_steamID;
			}
		}

		private void Start()
		{
			DownloadUserStats();
		}

		public static Sprite GetAchievementIcon(string _achievementID)
		{
			Rect rect = new Rect(0f, 0f, 184f, 184f);
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			int achievementIcon = SteamUserStats.GetAchievementIcon(_achievementID);
			if (achievementIcon == -1)
			{
				for (int i = 0; i <= 2000; i++)
				{
					UnityEngine.Debug.Log("Icon not found");
				}
			}
			if (achievementIcon > 0)
			{
				SteamUtils.GetImageSize(achievementIcon, out uint pnWidth, out uint pnHeight);
				if (pnWidth != 0 && pnHeight != 0)
				{
					byte[] array = new byte[4 * pnWidth * pnHeight];
					SteamUtils.GetImageRGBA(achievementIcon, array, (int)(4 * pnWidth * pnHeight));
					Texture2D texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false);
					texture2D.LoadRawTextureData(array);
					texture2D.Apply();
					return Sprite.Create(texture2D, rect, pivot);
				}
			}
			return null;
		}

		private void DownloadUserStats()
		{
			SteamAPICall_t hAPICall = SteamUserStats.RequestUserStats(m_steamID);
			m_userStatsReceived.Set(hAPICall, OnUserStatsReceived);
		}

		public static void UpdateSteamStat(string _achievementID, int _statQuantity)
		{
			Achievements achievements = listOfAchievements.Find((Achievements _achievement) => _achievement.achievementID == _achievementID);
			try
			{
				if (achievements.achievementID.Equals(_achievementID))
				{
					SteamUserStats.SetStat(_achievementID + "_STAT", _statQuantity);
					UnityEngine.Debug.Log("Steam Stat Successfully updated!");
				}
			}
			catch (NullReferenceException ex)
			{
				UnityEngine.Debug.Log("Update Stat failed!Error: " + ex?.ToString());
			}
			m_storeStats = true;
		}

		public static int GetSteamStat(string _achievementID)
		{
			Achievements achievements = listOfAchievements.Find((Achievements _achievement) => _achievement.achievementID == _achievementID);
			try
			{
				if (achievements.achievementID.Equals(_achievementID))
				{
					SteamUserStats.GetStat(_achievementID + "_STAT", out int pData);
					UnityEngine.Debug.Log("Stat successfuled got!");
					return pData;
				}
				return 0;
			}
			catch (NullReferenceException)
			{
				UnityEngine.Debug.Log("Not found!");
				return 0;
			}
		}

		public static void UnlockSteamAchievement(string _achievementID)
		{
			Achievements achievements = listOfAchievements.Find((Achievements _achievement) => _achievement.achievementID == _achievementID);
			try
			{
				if (achievements.achievementID == _achievementID)
				{
					SteamUserStats.SetAchievement(_achievementID);
					m_storeStats = true;
				}
			}
			catch (NullReferenceException)
			{
				UnityEngine.Debug.Log("The achievement name was not found, verify if the name follows the standard");
			}
		}

		public static void ClearSteamAchievement(string _achievementID)
		{
			Achievements achievements = listOfAchievements.Find((Achievements _achievement) => _achievement.achievementID == _achievementID);
			try
			{
				if (achievements.achievementID == _achievementID)
				{
					SteamUserStats.ClearAchievement(_achievementID);
					m_storeStats = true;
				}
			}
			catch (NullReferenceException)
			{
				UnityEngine.Debug.Log("The achievement name was not found, verify if the name follows the standard");
			}
		}

		public static void ClearAllSteamAchievements()
		{
			foreach (Achievements listOfAchievement in listOfAchievements)
			{
				SteamUserStats.ClearAchievement(listOfAchievement.achievementID);
				listOfAchievement.Achieved = false;
				SteamUserStats.SetStat(listOfAchievement.achievementID + "_STAT", 0);
			}
			m_storeStats = true;
		}

		public static void CheckAchievementAndStats(string _achievementID)
		{
			Achievements achievements = listOfAchievements.Find((Achievements _achievement) => _achievement.achievementID == _achievementID);
			try
			{
				if (achievements.achievementID.Equals(_achievementID))
				{
					SteamUserStats.GetAchievement(_achievementID, out achievements.Achieved);
					achievements.achievementName = SteamUserStats.GetAchievementDisplayAttribute(achievements.achievementID, "name");
					achievements.achievementDesc = SteamUserStats.GetAchievementDisplayAttribute(achievements.achievementID, "desc");
					SteamUserStats.GetStat(achievements.achievementID + "_STAT", out achievements.achievementStat);
					UnityEngine.Debug.Log("Achievement ID - " + achievements.achievementID + "\n Achievement Name - " + achievements.achievementName + "\n Achievement Descri - " + achievements.achievementDesc + "\n Achievement Stat - " + achievements.achievementStat.ToString() + "\n Achievement Achieved - " + achievements.Achieved.ToString() + "\n");
				}
			}
			catch (NullReferenceException)
			{
				UnityEngine.Debug.Log("The achievement name was not found, verify if the name follows the standard");
			}
		}

		private void OnUserStatsReceived(UserStatsReceived_t _callback, bool IOFailure)
		{
			string[] obj = new string[6]
			{
				"Failure - ",
				IOFailure.ToString(),
				" User - ",
				null,
				null,
				null
			};
			CSteamID steamIDUser = _callback.m_steamIDUser;
			obj[3] = steamIDUser.ToString();
			obj[4] = "GameID -";
			obj[5] = _callback.m_nGameID.ToString();
			UnityEngine.Debug.Log(string.Concat(obj));
			foreach (Achievements listOfAchievement in listOfAchievements)
			{
				if (SteamUserStats.GetAchievement(listOfAchievement.achievementID, out listOfAchievement.Achieved))
				{
					listOfAchievement.achievementName = SteamUserStats.GetAchievementDisplayAttribute(listOfAchievement.achievementID, "name");
					listOfAchievement.achievementDesc = SteamUserStats.GetAchievementDisplayAttribute(listOfAchievement.achievementID, "desc");
					SteamUserStats.GetStat(listOfAchievement.achievementID + "_STAT", out listOfAchievement.achievementStat);
					UnityEngine.Debug.Log("Achievement ID - " + listOfAchievement.achievementID + "\n Achievement Name - " + listOfAchievement.achievementName + "\n Achievement Descri - " + listOfAchievement.achievementDesc + "\n Achievement Stat - " + listOfAchievement.achievementStat.ToString() + "\n Achievement Achieved - " + listOfAchievement.Achieved.ToString() + "\n");
				}
				else
				{
					UnityEngine.Debug.Log("SteamUserStats.GetAchievement failed for Achievement " + listOfAchievement.achievementID + "\nIs it registered in the Steam Partner site?");
				}
			}
		}

		private static void OnUserStatsStored(UserStatsStored_t _callback, bool IOFailure)
		{
			if (EResult.k_EResultOK == _callback.m_eResult)
			{
				UnityEngine.Debug.Log("StoreStats was successful");
			}
			else
			{
				UnityEngine.Debug.Log("StoreStats failed");
			}
		}

		private static void OnUserAchievementStored(UserAchievementStored_t _callback, bool IOFailure)
		{
			if (_callback.m_nMaxProgress == 0)
			{
				UnityEngine.Debug.Log("Achievement " + _callback.m_rgchAchievementName + " Unlocked!");
			}
			else
			{
				UnityEngine.Debug.Log("Achievement " + _callback.m_rgchAchievementName + "Progress - " + _callback.m_nCurProgress.ToString() + " - Max - " + _callback.m_nMaxProgress.ToString());
			}
		}

		public void Update()
		{
			if (SteamManager.Initialized)
			{
				if (m_storeStats)
				{
					m_storeStats = !SteamUserStats.StoreStats();
					DownloadUserStats();
				}
				SteamAPI.RunCallbacks();
			}
		}
	}
}
