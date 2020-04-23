using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { Point, Unit, UnitOrPoint }
public enum WhichUnit { Target, Caster, Origin}

public class Effect : ScriptableObject{
	[HideInInspector]
	public Unit originUnit;
	[HideInInspector]
	public Unit casterUnit;
	[HideInInspector]
	public Unit targetUnit;
	[HideInInspector]
	public Vector2 targetPoint;

	#region 触发效果

	public virtual void Trigger() {
		if(originUnit == null) {
			originUnit = casterUnit;
		}
		if (targetUnit == null) {
			targetUnit = casterUnit;
		}

		if (targetPoint == default) {
			targetPoint = casterUnit.pos;
		}
	}

	public void Trigger(Unit casterUnit) {
		this.casterUnit = casterUnit;

		Trigger();
	}
	public void Trigger(Unit casterUnit, Unit targetUnit) {
		this.casterUnit = casterUnit;
		this.targetUnit = targetUnit;

		Trigger();
	}
	public void Trigger(Unit casterUnit, Vector2 targetPoint) {
		this.casterUnit = casterUnit;
		this.targetPoint = targetPoint;

		Trigger();
	}
	public void Trigger(Unit casterUnit, Unit targetUnit, Vector2 targetPoint) {
		this.targetPoint = targetPoint;

		Trigger(casterUnit, targetUnit);
	}

	#endregion



}
