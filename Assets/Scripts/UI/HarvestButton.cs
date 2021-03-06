using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestButton : MonoBehaviour
{
    public InventoryObject inventory;
    public GameObject loading;

    public void Harvest()
    {
        loading.SetActive(true);

        List<InventorySlot> slots = inventory.GetInventorySlots();

        List<ulong> ids = new List<ulong>();
        List<ulong> amounts = new List<ulong>();

        foreach (var slot in slots)
        {
            ids.Add(slot.item.id);
            amounts.Add(Convert.ToUInt64(slot.amount));
            Debug.Log($"id: {slot.item.id} amount: {slot.amount}");

        }
#if !UNITY_EDITOR

        AlgoServer.instance.HarvestASA(ids, amounts);
#else
        Debug.Log("HarvestClicked");
#endif
    }

    private void OnDestroy()
    {
        inventory.Clear();
    }
}
