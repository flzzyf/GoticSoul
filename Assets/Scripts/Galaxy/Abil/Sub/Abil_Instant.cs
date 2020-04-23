using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abil/Instant")]
public class Abil_Instant : Abil{
	public Effect effect_Cast;

	public override void Cast() {
		base.Cast();

		if(effect_Cast != null) {
			EffectManager.GetEffectInstance(effect_Cast).Trigger(caster, caster, caster.transform.position);
		}
	}
}
