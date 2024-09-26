using BratusSteamLibraries;
using System.Collections;
using UnityEngine;

public class BratusManager : MonoBehaviour
{
	private IEnumerator test()
	{
		yield return new WaitForSeconds(2f);
		SteamLeaderboard.InstantiateLeaderboard(this);
	}

	private void Start()
	{
		StartCoroutine("test");
	}
}
