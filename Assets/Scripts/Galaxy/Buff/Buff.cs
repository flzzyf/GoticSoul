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

	public void OnAdd(Unit unit) {
		unit.speed *= speedMultiplier;
	}

	public void OnRemove(Unit unit) {
		unit.speed /= speedMultiplier;
	}
}
