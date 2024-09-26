using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
	public void ToggleFullscreen(bool isFullScreen)
	{
		Screen.fullScreen = isFullScreen;
	}
}
