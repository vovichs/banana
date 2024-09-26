using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LayerLabT.heStone
{
	public class PanelTheStone : MonoBehaviour
	{
		public Resolution[] availableResolutions;

		public TMP_Dropdown resolutionDropdown;

		[SerializeField]
		private GameObject[] otherPanels;

		public void Awake()
		{
			base.gameObject.SetActive(value: false);
			availableResolutions = Screen.resolutions;
			availableResolutions = (from res in availableResolutions
				group res by res.width.ToString() + " x " + res.height.ToString() into res
				where res.Count() > 0
				select res.First()).ToArray();
			resolutionDropdown.ClearOptions();
			List<string> list = new List<string>();
			for (int i = 0; i < availableResolutions.Length; i++)
			{
				list.Add(availableResolutions[i].width.ToString() + " x " + availableResolutions[i].height.ToString());
			}
			resolutionDropdown.AddOptions(list);
		}

		public void OnEnable()
		{
			for (int i = 0; i < otherPanels.Length; i++)
			{
				otherPanels[i].SetActive(value: true);
			}
		}

		public void OnDisable()
		{
			for (int i = 0; i < otherPanels.Length; i++)
			{
				otherPanels[i].SetActive(value: false);
			}
		}

		public void OpenDiscord()
		{
			Application.OpenURL("https://discord.gg/thebananas");
		}

		public void SetResolution(int resolutionIndex)
		{
			Resolution resolution = availableResolutions[resolutionIndex];
			Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		}
	}
}
