using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager{
	static Dictionary<string, WeaponData> weaponDic;

	public static void Init() {
		//读取武器
		string table = Application.streamingAssetsPath + "/Tables/Item.xlsx";
		var weaponList = ExcelReader.ReadExcel<WeaponData>(table, "Weapon");

		weaponDic = new Dictionary<string, WeaponData>();
		foreach (var item in weaponList) {
			weaponDic.Add(item.ID, item);
		}
	}

	//获取武器
	public static WeaponData GetWeapon(string id) {
		return weaponDic[id];
	}
}
