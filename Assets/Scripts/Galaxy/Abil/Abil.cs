using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Abil")]
public class Abil : ScriptableObject{
	//技能的释放动画
	public UnitAnim castAnim;

	//技能前摇，释放时间，后摇
	public float preswingDuration;
	public float swingDuration;
	public float backswingDuration;

	protected Unit caster;
	protected Unit target;
	protected Vector2 targetPoint;
	//释放
	public virtual void Cast() {
		//如设置了释放动画，播放
		if(castAnim != UnitAnim.Null) {
			caster.actor.PlayAnim(castAnim, preswingDuration, swingDuration, backswingDuration);
		}
	}
	public void Cast(Unit caster) {
		this.caster = caster;

		Cast();
	}
	public void Cast(Unit caster, Vector2 targetPoint) {
		this.caster = caster;
		this.targetPoint = targetPoint;

		Cast();
	}
	public void Cast(Unit caster, Unit target) {
		this.caster = caster;
		this.target = target;

		Cast();
	}
}
