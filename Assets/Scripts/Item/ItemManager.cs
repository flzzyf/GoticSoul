using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager {
	static List<Item> itemList;

	public static void Init() {
		string table = Application.streamingAssetsPath + "/Tables/Item.xlsx";
		itemList = ExcelReader.ReadExcel<Item>(table, "Item");
	}

}
