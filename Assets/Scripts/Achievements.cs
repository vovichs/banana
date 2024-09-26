using BratusSteamLibraries;
using UnityEngine;

public class Achievements : MonoBehaviour
{
	public void TriggerAchievements()
	{
		SteamAchievements.UnlockSteamAchievement("ACHIEVEMENT_00");
	}
}
