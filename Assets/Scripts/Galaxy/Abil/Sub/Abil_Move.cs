using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abil/Move")]
public class Abil_Move : Abil {
	public override void Cast() {
		base.Cast();

		caster.MoveToTarget(targetPoint.x);
	}
}
