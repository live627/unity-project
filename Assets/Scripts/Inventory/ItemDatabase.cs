using UnityEngine;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase {
	private List<Item> database = new List<Item>();
	private JsonData itemData;

	public ItemDatabase()
	{
		itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
		ConstructItemDatabase();	
	}

	public Item FetchItemById(int id)
	{
		for (int i = 0; i < database.Count; i++)
		{
			if (database[i].Id == id)
			{
				return database[i];
			}
		}

		return null;
	}
	
	void ConstructItemDatabase()
	{
		for (int i = 0; i < itemData.Count; i++)
		{
            Item newItem = new Item
            {
                Id = (int)itemData[i]["id"],
                Title = itemData[i]["title"].ToString(),
                Value = (int)itemData[i]["value"],
                Power = (int)itemData[i]["stats"]["power"],
                Defense = (int)itemData[i]["stats"]["defense"],
                Vitality = (int)itemData[i]["stats"]["vitality"],
                Description = itemData[i]["description"].ToString(),
                Stackable = (bool)itemData[i]["stackable"],
                Rarity = (int)itemData[i]["rarity"],
                Slug = itemData[i]["slug"].ToString()
            };
            newItem.Sprite = Resources.Load<Sprite>("Sprites/Items/" + newItem.Slug);

			database.Add(newItem);
		}
	}
}

public class Item
{
	public int Id { get; set; }
	public string Title { get; set; }
	public int Value { get; set; }
	public int Power { get; set; }
	public int Defense { get; set; }
	public int Vitality { get; set; }
	public string Description { get; set; }
	public bool Stackable { get; set; }
	public int Rarity { get; set; }
	public string Slug { get; set; }
	public Sprite Sprite { get; set; }

	public Item()
	{
		this.Id = -1;
	}
}