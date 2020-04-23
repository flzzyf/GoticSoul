using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//单位标旗
public enum UnitFlag {
	//不可移动
	Unmovable,
	//不可控制
	Uncontrollable,
	//无敌
	Invincible,
}

public class Unit : MonoBehaviour{
	public Actor actor;

	RigidbodyBox rb;

	//拥有的技能列表
	public List<Abil> abilList;
	//拥有的Buff列表
	public List<Buff> buffList;

	//单位所属玩家
	public int player;

	//单位位置
	public Vector2 pos {
		get {
			return transform.position;
		}
	}

	private void Awake() {
		rb = GetComponent<RigidbodyBox>();

		try {
			//初始化各个组件
			InitFlag();
			InitWeapon();
			InitMoving();
		}
		finally {
			//最后初始化演算体
			actor.Init();
		}
	}

	#region 移动

	//初始速度
	public float initSpeed = 7f;
	//当前速度
	[HideInInspector]
	public float speed;

	//正在移动
	public bool moving;

	void InitMoving() {
		speed = initSpeed;
	}

	//向目标角度移动
	public void Move(int dir, bool faceTargetDir = true) {
		//如果无法移动
		if (HasFlag(UnitFlag.Unmovable)) {
			return;
		}

		rb.movingForce = Vector2.right * dir * speed;

		moving = true;

		//面向移动方向
		if (faceTargetDir) {
			Face(dir);
		}
	}

	public void StopMoving() {
		if (moving) {
			rb.movingForce = Vector2.zero;

			moving = false;
		}
	}

	//移动到目标X坐标
	public void MoveToTarget(float x) {
		StartCoroutine(MoveToTargetCor(x));
	}
	IEnumerator MoveToTargetCor(float x) {
		float offset = x - transform.position.x;

		if (Mathf.Abs(offset) > speed * Time.fixedDeltaTime) {
			int dir = (int)Mathf.Sign(offset);
			Move(dir);

			while (Mathf.Abs(x - transform.position.x) >= speed * Time.fixedDeltaTime) {
				yield return new WaitForFixedUpdate();
			}

			StopMoving();
		}
	}

	#endregion

	#region 狼剑术

	public void WolfSword() {
		//设为不可控制
		SetFlag(UnitFlag.Uncontrollable, false);

		StartCoroutine(WolfSwordCor());

		actor.PlayAnim(UnitAnim.WolfSword, -1, -1, -1, 
			() => {
				//搜索目标
				SetWeaponSearchCallback(unit => {
					Debug.Log("Hit:" + unit.name);

					unit.actor.PlayAnim(UnitAnim.TakeDown);
				});
			}, 
			() => {
				StopSearchTarget();
			},
			() => {
				//解除不可控制
				SetFlag(UnitFlag.Uncontrollable, true);
			});
	}

	IEnumerator WolfSwordCor() {
		float rollDuration = 1.15f;
		float rollDistance = 7f;
		float rollSpeed = rollDistance / rollDuration;

		yield return new WaitForSeconds(0.13f);

		for (float i = 0; i < rollDuration; i += Time.fixedDeltaTime) {
			rb.movingForce = Vector2.right * facing * rollSpeed;

			yield return new WaitForFixedUpdate();
		}
	}

	#endregion

	#region 单位朝向

	bool facingRight = true;
	public int facing { get { return facingRight ? 1 : -1; } }

	void Flip() {
		facingRight = !facingRight;

		actor.Flip();
	}

	public void Face(int dir) {
		if(dir != facing) {
			Flip();
		}
	}

	#endregion

	#region 单位标旗

	//标旗数值表
	Dictionary<UnitFlag, int> flagValueDic;

	void InitFlag() {
		flagValueDic = new Dictionary<UnitFlag, int>();

		foreach (UnitFlag flag in Enum.GetValues(typeof(UnitFlag))) {
			flagValueDic.Add(flag, 0);
		}
	}

	//设置标旗
	public void SetFlag(UnitFlag flag, bool value) {
		//Debug.Log(string.Format("{0}标旗：{1}", value ? "添加" : "移除", flag.ToString()));
		
		int modiify = value ? 1 : -1;

		flagValueDic[flag] += modiify;
	}

	//具有标旗
	public bool HasFlag(UnitFlag flag) {
		return flagValueDic[flag] > 0;
	}

	#endregion

	#region 生命

	public int hpMax = 1;
	int hp;

	public void ModifyHp(int value) {
		hp += value;

		if(hp <= 0) {
			Die();
		}
	}

	public void Die() {
		gameObject.SetActive(false);
	}

	#endregion

	#region 攻击

	//攻击前摇
	public float attackPreswing = .2f;
	//攻击后摇
	public float attackReswing = .3f;

	int attackIndex;

	public void Attack() {
		StopMoving();

		//设为不可控制
		SetFlag(UnitFlag.Uncontrollable, false);

		UnitAnim anim = attackIndex == 0 ? UnitAnim.Slash : UnitAnim.RisingSlash;
		actor.PlayAnim(anim, 0, 0, 0,
			() => {
				//搜索目标
				SetWeaponSearchCallback(unit => {
					Debug.Log("Hit:" + unit.name);

					HitTarget(unit);
				});
			},
			() => {
				//结束搜索目标
				StopSearchTarget();

				//解除不可控制
				SetFlag(UnitFlag.Uncontrollable, true);
			},
			() => {
				
			});

		if (attackIndex == 0)
			attackIndex++;
		else
			attackIndex = 0;
	}

	void SetWeaponSearchCallback(Action<Unit> callback) {
		weapon.SearchTargetStart(callback);
	}

	void StopSearchTarget() {
		weapon.SearchTargetEnd();
	}

	void HitTarget(Unit unit) {
		unit.actor.PlayAnim(UnitAnim.Hit);
	}

	#endregion

	#region 单位武器

	//初始武器预制体
	public Weapon weaponPrefab;
	//当前使用武器
	Weapon weapon;

	//武器父级物体
	public Transform weaponParent;

	void InitWeapon() {
		//有初始武器就生成
		if(weaponPrefab != null) {
			weapon = Instantiate(weaponPrefab, weaponParent);
		}
	}

	#endregion

}

