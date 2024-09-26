using TMPro;
using UnityEngine;

public class UpdateMoney : MonoBehaviour
{
	public TMP_Text moneyText;

	private ulong money;

	private void Awake()
	{
		if (SteamManager.Initialized)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 30;
			AddMoney();
			ToggleFullscreen(isFullScreen: false);
		}
	}

	public void AddMoney()
	{
		moneyText.text = (money++.ToString() ?? "");
	}

	public void OpenMarketplace()
	{
		Application.OpenURL("https://store.steampowered.com/itemstore/2923300/?filter=Featured");
	}

	public void ToggleFullscreen(bool isFullScreen)
	{
		Screen.fullScreen = isFullScreen;
	}
}
