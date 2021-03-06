﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

[CreateAssetMenu(menuName = "Effect/CP")]
public class Effect_CP : Effect {
	[Header("初始效果")]
	//初始效果
	public Effect initEffect;
	//初始偏移
	public Offset initOffset;

	[Header("周期效果")]
	//周期次数
	public int periodCount;
	//周期时长
	public float periodicDuration;
	//周期效果
	public Effect periodicEffect;
	//周期偏移
	public Offset periodicOffset;

	[Header("结束效果")]
	//结束效果
	public Effect finalEffect;

	//默认更新周期
	const float updatePeriod = 0.0625f;

	public override void Trigger() {
		base.Trigger();

		if(initEffect != null) {
			initOffset.Init(this);
			Vector2 pos = initOffset.targetPos;
			EffectManager.GetEffectInstance(initEffect).Trigger(casterUnit, pos);
		}

		//开始周期效果
		if (periodCount > 0) {
			GameManager.instance.StartCoroutine(TriggerPeriodicEffect());
		}
		else {
			//没有周期效果就直接执行最终效果
			if(finalEffect != null) {
				EffectManager.GetEffectInstance(finalEffect).Trigger(casterUnit);
			}
		}
	}

	//执行周期效果
	IEnumerator TriggerPeriodicEffect() {
		float period = periodicDuration > 0 ? periodicDuration : updatePeriod;

		for (int i = 0; i < periodCount; i++) {
			//如果有周期效果，执行
			if(periodicEffect != null) {
				periodicOffset.Init(this);
				Vector2 pos = periodicOffset.targetPos;
				EffectManager.GetEffectInstance(periodicEffect).Trigger(casterUnit, targetUnit, pos);
			}

			yield return new WaitForSeconds(period);
		}

		//执行最终效果
		if (finalEffect != null) {
			EffectManager.GetEffectInstance(finalEffect).Trigger(casterUnit);
		}
	}
}

[System.Serializable]
public class Offset {
	public enum TargetPos { TargetPoint, TargetUnit, CasterPoint, CasterUnit, OriginPoint, OriginUnit }

	public TargetPos offsetStartPos = TargetPos.OriginUnit;
	public TargetPos offsetEndPos = TargetPos.TargetPoint;
	public float offsetX;

	Effect effect;

	public void Init(Effect effect) {
		this.effect = effect;
	}

	//偏移的目标点
	public Vector2 targetPos {
		get {
			return GetTargetPoint(offsetStartPos) + dir * offsetX;
		}
	}

	//偏移方向
	Vector2 dir {
		get {
			Vector2 startPos = GetTargetPoint(offsetStartPos);
			Vector2 endPos = GetTargetPoint(offsetEndPos);

			//如果起点和终点一样
			if (startPos == endPos) {
				//如果起点是单位，返回其朝向。否则返回上
				if (startIsUnit) {
					return Vector2.right * startUnit.facing;
				}
				else {
					return Vector2.up;
				}
			}
			//起点和终点不一样，那就返回两个点的方向
			else {
				return (endPos - startPos).normalized;
			}
		}
	}

	//偏移起始位置是单位
	bool startIsUnit {
		get {
			return offsetStartPos == TargetPos.CasterUnit || offsetStartPos == TargetPos.TargetUnit || offsetStartPos == TargetPos.OriginUnit;
		}
	}

	//偏移起始单位
	Unit startUnit {
		get {
			if (offsetStartPos == TargetPos.OriginUnit) {
				return effect.originUnit;
			}
			else if (offsetStartPos == TargetPos.TargetPoint) {
				return effect.targetUnit;
			}

			return null;
		}
	}

	//获取目标位置坐标
	Vector2 GetTargetPoint(TargetPos pos) {
		if (pos == TargetPos.OriginUnit) {
			return effect.originUnit.pos;
		}
		else if (pos == TargetPos.TargetPoint) {
			return effect.targetPoint;
		}

		return default;
	}
}