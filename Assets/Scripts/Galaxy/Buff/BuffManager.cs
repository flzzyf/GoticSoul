﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class BuffManager : ManagerBase<Buff> {
	public static void OnInit() {
		Init();

		periodicBuffList = new List<Buff>();
		Thread update = new Thread(UpdateBuffDuration);
		update.Start();
	}

	const float updatePeriod = 0.0625f;

	static List<Buff> periodicBuffList;

	static void UpdateBuffDuration(object obj) {
		while (true) {
			Thread.Sleep((int)(updatePeriod * 1000));

			for (int i = periodicBuffList.Count - 1; i >= 0; i--) {
				Buff buff = periodicBuffList[i];
				//扣除持续时间
				buff.duration -= updatePeriod;
				Debug.Log(buff.duration);

				//超时则移除该Buff
				if (buff.duration <= 0) {
					RemoveBuff(buff.owner, buff);
					periodicBuffList.RemoveAt(i);
				}
			}
		}
	}

	//添加Buff
	public static void AddBuff(Unit unit, Buff buff) {
		//创建Buff实例
		Buff instance = GameObject.Instantiate<Buff>(buff);
		instance.owner = unit;

		//添加Buff到单位列表
		unit.buffList.Add(instance);

		//if (buff.maxStackCount > 1) {
		//	if(HasBuff(unit, buff)) {

		//	}
		//	unit.buffList.Add(buff);
		//}

		//Buff有持续时间
		if(instance.duration != -1) {
			periodicBuffList.Add(instance);
		}

		instance.OnAdd(unit);
	}

	public static void RemoveBuff(Unit unit, Buff buff) {
		Debug.Log("移除");
		for (int i = 0; i < unit.buffList.Count; i++) {
			Buff bf = periodicBuffList[i];
			if (bf.name == buff.name) {
				unit.buffList.RemoveAt(i);
				break;
			}
		}

		buff.OnRemove(unit);
	}

	//有Buff
	public static bool HasBuff(Unit unit, Buff buff) {
		foreach (var item in unit.buffList) {
			if(item.name == buff.name) {
				return true;
			}
		}

		return false;
	}

	//获取Buff
	public static Buff GetBuff(string name) {
		return GetData(name) as Buff;
	}

}
