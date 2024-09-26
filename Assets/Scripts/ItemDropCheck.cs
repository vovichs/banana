using Steamworks;
using UnityEngine;

public class ItemDropCheck : MonoBehaviour
{
	private SteamInventoryResult_t TriggerResult;

	private void Start()
	{
	}

	public void Update()
	{
	}

	public void TriggerItems()
	{
		SteamInventory.TriggerItemDrop(out TriggerResult, (SteamItemDef_t)99999);
		SteamInventory.TriggerItemDrop(out TriggerResult, (SteamItemDef_t)88888);
	}
}
