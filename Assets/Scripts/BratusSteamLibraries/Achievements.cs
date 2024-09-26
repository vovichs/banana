namespace BratusSteamLibraries
{
	public class Achievements
	{
		public string achievementID;

		public string achievementName;

		public string achievementDesc;

		public int achievementStat;

		public bool Achieved;

		public Achievements(string _achievementID, string _AchievementName, string _AchievementDesc, bool _Achieved, int _achievementStat)
		{
			achievementID = _achievementID;
			achievementName = _AchievementName;
			achievementDesc = _AchievementDesc;
			achievementStat = _achievementStat;
			Achieved = _Achieved;
		}
	}
}
