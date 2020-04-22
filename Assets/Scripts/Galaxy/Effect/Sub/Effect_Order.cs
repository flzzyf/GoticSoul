using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Order")]
public class Effect_Order : Effect {
	//指令技能
	public Abil abil;

	public override void Trigger() {
		base.Trigger();

		abil.Cast(casterUnit, targetPoint);
	}
}
