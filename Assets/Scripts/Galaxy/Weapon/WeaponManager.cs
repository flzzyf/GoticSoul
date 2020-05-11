using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>{
	static Dictionary<string, WeaponData> weaponDic;

	public Weapon weaponPrefab;

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

	//为单位装备武器
	public static void EquipWeapon(Unit unit, string id) {
		Weapon weapon = Instantiate(instance.weaponPrefab);

		WeaponData weaponData = GetWeapon(id);
		weapon.Init(weaponData);

		//装备武器
		unit.EquipWeapon(weapon);
	}
}
