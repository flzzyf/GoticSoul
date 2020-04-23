using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abil/Stop")]
public class Abil_Stop : Abil {
	public override void Cast() {
		base.Cast();

		caster.StopMoving();
	}
}
