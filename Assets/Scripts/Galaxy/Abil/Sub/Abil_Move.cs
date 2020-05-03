using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abil/Move")]
public class Abil_Move : Abil {
	Vector2 targetPoint;

	public override void Cast() {
		base.Cast();

		caster.MoveToTarget(targetPoint.x);
	}
	public void Cast(Unit caster, Vector2 targetPoint) {
		this.caster = caster;
		this.targetPoint = targetPoint;

		Cast();
	}
}
