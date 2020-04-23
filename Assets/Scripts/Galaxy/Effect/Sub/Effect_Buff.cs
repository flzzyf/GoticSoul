using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Buff")]
public class Effect_Buff : Effect {
	public WhichUnit whichUnit;

	Unit targetU {
		get {
			if (whichUnit == WhichUnit.Target) {
				return targetUnit;
			}
			else if (whichUnit == WhichUnit.Caster) {
				return casterUnit;
			}

			return null;
		}
	}

	//修改数量
	public int number = 1;

	public Buff buff;

	public override void Trigger() {
		base.Trigger();

		if(buff == null) {
			Debug.LogError("未设置效果Buff：" + name);
		}

		if(number > 0) {
			//Debug.Log("添加Buff:" + buff.name);
			for (int i = 0; i < number; i++) {
				BuffManager.AddBuff(targetU, buff);
			}
		}
		else if(number < 0) {
			//Debug.Log("移除Buff:" + buff.name);
			for (int i = 0; i < Mathf.Abs(number); i++) {
				BuffManager.RemoveBuff(targetU, buff);
			}
		}
	}
}
