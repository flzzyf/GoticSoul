using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_AddBuff : Effect{
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
	public int number;

	public Buff buff;

	public override void Trigger() {
		base.Trigger();

		if(number > 0) {
			for (int i = 0; i < number; i++) {
				BuffManager.AddBuff(targetU, buff);
			}
		}
		else if(number < 0) {
			for (int i = 0; i < Mathf.Abs(number); i++) {
				BuffManager.RemoveBuff(targetU, buff);
			}
		}
	}
}
