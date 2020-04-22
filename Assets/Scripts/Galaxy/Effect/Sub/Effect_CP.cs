using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/CP")]
public class Effect_CP : Effect {
	public Effect effect_Init;

	//偏移
	public Offset offset;

	public override void Trigger() {
		base.Trigger();

		offset.Init(this);
		Vector2 pos = offset.targetPos;
		Debug.Log("目标位置" + pos);
		EffectManager.GetEffectInstance(effect_Init).Trigger(casterUnit, pos);
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
		Debug.Log("方向" + dir);
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