using UnityEngine;

namespace BratusSteamLibraries
{
	public class LeaderboardCell
	{
		public Sprite playerImage;

		public string playerName;

		public int playerScore;

		public LeaderboardCell(Sprite _playerImage, string _playerName, int _playerScore)
		{
			playerImage = _playerImage;
			playerName = _playerName;
			playerScore = _playerScore;
		}
	}
}
