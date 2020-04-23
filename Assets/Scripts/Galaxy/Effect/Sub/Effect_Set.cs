using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Set")]
public class Effect_Set : Effect {
	[HideInInspector]
	public List<Effect> effectList;

	public override void Trigger() {
		base.Trigger();

		for (int i = 0; i < effectList.Count; i++) {
			EffectManager.GetEffectInstance(effectList[i]).Trigger(casterUnit, targetUnit, targetPoint);
		}
	}
}
