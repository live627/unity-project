using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Main.UI;

public class Inventory 
{
	GameObject inventoryPanel;
	GameObject slotPanel;
	ItemDatabase database;

	private int slotAmount;
	public List<Item> items = new List<Item>();
	public List<GameObject> slots = new List<GameObject>();
    private Tooltip tooltip;

    public Inventory()
	{
             tooltip = new Tooltip(GameObject.Find("Tooltip"));
        
                database = new ItemDatabase();
        slotAmount = 30;
		inventoryPanel = GameObject.Find("InventoryPanel");
        inventoryPanel.SetActive(false);
		slotPanel = inventoryPanel.transform.Find("SlotPanel").gameObject;
		for (int i = 0; i < slotAmount; i++)
		{
			items.Add(new Item());
			slots.Add(Object.Instantiate(Resources.Load("Prefabs/Inventory/Slot", typeof(GameObject)) as GameObject, slotPanel.transform));
            Slot data = slots[i].AddComponent<Slot>();
            data.id = i;
            data.inv = this;
		}

		AddItem(0);
		AddItem(1);
		AddItem(1);
		AddItem(1);
		AddItem(1);
		AddItem(1);
		AddItem(1);
		AddItem(1);
		AddItem(1);
		AddItem(2);
	}

	public void AddItem(int id)
	{
		Item itemToAdd = database.FetchItemById(id);
		if (itemToAdd.Stackable && CheckIfItemIsInInventory(itemToAdd))
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Id == id)
				{
					ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
					data.amount++;
					data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
					break;
				}
			}
		}
		else
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Id == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Object.Instantiate(Resources.Load("Prefabs/Inventory/Item", typeof(GameObject)) as GameObject, slots[i].transform, false);
                    ItemData itemData = itemObj.AddComponent<ItemData>();
                    itemData.Init(this, tooltip);
                    itemData.item = itemToAdd;
                    itemData.slotId = i;
                    itemObj.transform.position = Vector2.zero;
                    itemObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                    itemObj.name = "Item: " + itemToAdd.Title;
                    slots[i].name = "Slot: " + itemToAdd.Title;
                    break;
                }
            }
		}
	}

    bool CheckIfItemIsInInventory(Item item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].Id == item.Id)
			{
				return true;
			}
		}

		return false;
	}

}
