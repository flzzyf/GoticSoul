using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff")]
public class Buff : ScriptableObject {
	public string name;

	//移动速度倍率
	public float speedMultiplier = 1;

	//可叠加次数
	public int maxStackCount = 1;

	//持续时间
	public float duration = -1;

	//持有该Buff的单位
	[HideInInspector]
	public Unit owner;

	//标旗修改
	[HideInInspector]
	public List<UnitFlagValue> flagList;

	public void OnAdd(Unit unit) {
		unit.speed *= speedMultiplier;

		//标旗添加
		foreach (var item in flagList) {
			if (item.value) {
				unit.SetFlag(item.flag, true);
			}
		}
	}

	public void OnRemove(Unit unit) {
		unit.speed /= speedMultiplier;

		//标旗移除
		foreach (var item in flagList) {
			if (item.value) {
				unit.SetFlag(item.flag, false);
			}
		}
	}

	public void Init() {
		//初始化标旗字典
		if (flagList == null) {
			flagList = new List<UnitFlagValue>(); 
		}

		//如果枚举里有新键，加入
		foreach (UnitFlag item in System.Enum.GetValues(typeof(UnitFlag))) {
			if (!flagList.Contains(item)) {
				flagList.Add(new UnitFlagValue{ flag = item });
			}
		}
	}
}

[System.Serializable]
public class UnitFlagValue {
	public UnitFlag flag;
	public bool value;
}

public static class MehtondExtension {
	//包含Flag
	public static bool Contains(this List<UnitFlagValue> list, UnitFlag flag) {
		foreach (var item in list) {
			if(item.flag == flag) {
				return true;
			}
		}

		return false;
	}
}