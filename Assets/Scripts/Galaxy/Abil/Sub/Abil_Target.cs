using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abil/Target")]
public class Abil_Target : Abil {
	public Effect effect_Cast;
	Vector2 targetPoint;

	public override void Cast() {
		//如果无法消耗，返回
		if (!cost.CanCost(caster)) {
			return;
		}

		base.Cast();

		if (effect_Cast != null) {
			EffectManager.GetEffectInstance(effect_Cast).Trigger(caster, targetPoint);
		}
	}
	public void Cast(Unit caster, Vector2 targetPoint) {
		this.caster = caster;
		this.targetPoint = targetPoint;

		Cast();
	}
}
