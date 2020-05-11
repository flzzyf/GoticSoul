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
	}

	private void Start() {
		try {
			//初始化各个组件
			InitFlag();
			InitWeapon();
			InitMoving();
			InitAttribute();
		}
		finally {
			//最后初始化演算体
			actor.Init();
		}
	}

	private void Update() {
		//更新属性
		UpdateAttribute();
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
		//if (faceTargetDir) {
		//	Face(dir);
		//}
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

		//该单位是玩家，且刚恢复控制
		if(GameManager.Player == this && flag == UnitFlag.Uncontrollable && !HasFlag(UnitFlag.Uncontrollable)) {
			//触发回调
			PlayerController.OnBecomeControllable();
		}
	}

	//具有标旗
	public bool HasFlag(UnitFlag flag) {
		return flagValueDic[flag] > 0;
	}

	#endregion

	#region 单位属性

	[Header("单位属性")]
	public UnitAttribute attribute_HP;
	public UnitAttribute attribute_Stamina;

	void InitAttribute() {
		attribute_HP.Init();
		attribute_Stamina.Init();

		attribute_HP.onReach0 += () => {
			Die();
		};
	}

	void UpdateAttribute() {
		attribute_HP.Update();
		attribute_Stamina.Update();
	}

	//修改属性值
	public void ModifyAttribute(UnitAttributeType type, float modifyValue) {
		if (type == UnitAttributeType.Hp) {
			attribute_HP.ModifyValue(modifyValue);
		}
		else if (type == UnitAttributeType.Stamina) {
			attribute_Stamina.ModifyValue(modifyValue);
		}
	}

	#region 生命

	//死掉
	public void Die() {
		gameObject.SetActive(false);
	}

	#endregion

	#endregion

	#region 单位武器

	//武器
	Weapon weapon;

	//武器父级
	public Transform weaponParent;

	//初始武器
	public string weaponName;

	void InitWeapon() {
		//有初始武器就生成
		if(weaponName != "") {
			WeaponManager.EquipWeapon(this, weaponName);
		}
	}

	//使用武器
	public void UseWeapon() {
		if (weapon == null)
			return;

		weapon.UseWeapon(this);
	}

	//装备武器
	public void EquipWeapon(Weapon weapon) {
		this.weapon = weapon;

		//附着武器到武器位置
		weapon.transform.SetParent(weaponParent);
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localRotation = Quaternion.identity;
	}

	#endregion

}

public enum UnitAttributeType {
	Hp,
	Stamina,
	Mana,
}

//单位属性
[System.Serializable]
public class UnitAttribute {
	//初始上限
	public float max = 1;
	//当前上限
	[NonSerialized]
	public float currentMax;
	//当前值
	[NonSerialized]
	public float currentValue;

	public float currentPercent { get { return currentValue / currentMax; } }

	//每秒恢复率
	public float regenRate;

	public void Init() {
		currentMax = max;
		currentValue = max;
	}

	//修改值
	public void ModifyValue(float modifyValue) {
		currentValue += modifyValue;

		onModifyValue?.Invoke();

		//触发归零回调
		if(onReach0 != null && currentValue <= 0) {
			onReach0.Invoke();
		}
	}

	//当降为0的回调
	[NonSerialized]
	public Action onReach0;

	[NonSerialized]
	public Action onModifyValue;

	public void Update() {
		if(regenRate > 0) {
			ModifyValue(regenRate * Time.deltaTime);
		}
	}
}

