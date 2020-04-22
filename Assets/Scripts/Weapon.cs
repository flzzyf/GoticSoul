using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour{
	Action<Unit> onHitTarget;

	List<Unit> colliderList = new List<Unit>();

	//已成为过目标的目标
	List<Unit> currentTargetList;

	//在搜索目标
	bool isSearchingTarget;

	//拿着该武器的单位
	Unit holder;

	private void Awake() {
		holder = GetComponentInParent<Unit>();
	}

	//开始搜索目标
	public void SearchTargetStart(Action<Unit> onHitTargetCallback) {
		isSearchingTarget = true;
		currentTargetList = new List<Unit>();

		onHitTarget = onHitTargetCallback;

		//对接触该碰撞体的单位施加效果
		foreach (var unit in colliderList) {
			if(RelationshipManager.RelationshipBetween(holder, unit) == Relationship.Enemy) {
				currentTargetList.Add(unit);

				onHitTarget?.Invoke(unit);
			}
		}
	}

	//结束搜索目标
	public void SearchTargetEnd() {
		isSearchingTarget = false;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Unit unit = collision.GetComponent<Unit>();
		if(unit != null) {
			//Debug.Log("OnTriggerEnter2D:" + unit.name);

			colliderList.Add(unit);

			//如果在搜索目标，而且目标是敌人，触发效果
			if (isSearchingTarget && RelationshipManager.RelationshipBetween(holder, unit) == Relationship.Enemy) {
				//没在已成为目标的List中
				if (!currentTargetList.Contains(unit)) {
					currentTargetList.Add(unit);

					onHitTarget?.Invoke(collision.GetComponent<Unit>());
				}
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		Unit unit = collision.GetComponent<Unit>();
		if (unit != null) {
			//Debug.Log("OnTriggerExit2D:" + unit.name);

			colliderList.Remove(unit);
		}
	}

	public void SetParticle(bool enable) {
		if (enable)
			GetComponentInChildren<ParticleSystem>().Emit(40);
	}

	public Transform slashEffect;

	public void ShowSlashEffect(bool show) {
		foreach (var item in slashEffect.GetComponentsInChildren<MeleeWeaponTrail>()) {
			item.Emit = show;
		}
	}
}
