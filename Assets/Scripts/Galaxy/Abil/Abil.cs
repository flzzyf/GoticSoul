using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abil : ScriptableObject{
	//技能的释放动画
	public UnitAnim castAnim;

	//技能前摇，释放时间，后摇
	[Header("动画持续时间")]
	public float preswingDuration;
	public float swingDuration;
	public float backswingDuration;

	protected Unit caster;
	protected Unit target;

	//释放
	public virtual void Cast() {
		//如果无法消耗，返回
		if (!cost.CanCost(caster)) {
			return;
		}

		//如设置了释放动画，播放
		if(castAnim != UnitAnim.Null) {
			caster.actor.PlayAnim(castAnim, preswingDuration, swingDuration, backswingDuration);
		}

		//消耗属性
		cost.Cost(caster);
	}
	public void Cast(Unit caster) {
		this.caster = caster;

		Cast();
	}
	public void Cast(Unit caster, Unit target) {
		this.caster = caster;
		this.target = target;

		Cast();
	}

	[Header("技能消耗")]
	public AbilCost cost;
}

//属性消耗
[System.Serializable]
public struct AbilCost {
	[Range(0, 100)]
	public float staminaCost;

	//产生消耗
	public void Cost(Unit caster) {
		if (staminaCost > 0) {
			caster.ModifyAttribute(UnitAttributeType.Stamina, -staminaCost);
		}
	}

	//能够消耗
	public bool CanCost(Unit caster) {
		if (caster.attribute_Stamina.currentValue >= staminaCost) {
			return true;
		}

		return false;
	}
}
