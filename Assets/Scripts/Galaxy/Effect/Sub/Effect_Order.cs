using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Order")]
public class Effect_Order : Effect {
	//指令技能
	public Abil abil;

	public override void Trigger() {
		base.Trigger();


		if(abil.GetType() == typeof(Abil_Instant)) {
			abil.Cast(casterUnit);
		}
		else if (abil.GetType() == typeof(Abil_Stop)) {
			abil.Cast(casterUnit);
		}
		else if (abil.GetType() == typeof(Abil_Target)) {
			(abil as Abil_Target).Cast(casterUnit, targetPoint);
		}
		else if(abil.GetType() == typeof(Abil_Move)) {
			(abil as Abil_Move).Cast(casterUnit, targetPoint);
		}
	}
}
