using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Anim")]
public class Effect_Anim : Effect {
	//释放动画
	public UnitAnim castAnim;

	//动画前摇，释放时间，后摇
	[Header("动画持续时间")]
	public float preswingDuration;
	public float swingDuration;
	public float backswingDuration;

	public Effect effect;

	public override void Trigger() {
		base.Trigger();

		//如设置了释放动画，播放
		//if (castAnim != UnitAnim.Null) {
		//	casterUnit.actor.PlayAnim(castAnim, preswingDuration, swingDuration, backswingDuration,
		//		() => {
		//			//搜索目标
		//			casterUnit.SetWeaponSearchCallback(unit => {
		//				Debug.Log("Hit:" + unit.name);

		//				HitTarget(unit);
		//			});
		//		},
		//		() => {
		//			//结束搜索目标
		//			casterUnit.StopSearchTarget();
		//		}
		//	);
		//}
	}

	void HitTarget(Unit unit) {
		unit.actor.PlayAnim(UnitAnim.Hit);

		unit.ModifyAttribute(UnitAttributeType.Hp, -30);
	}
}
