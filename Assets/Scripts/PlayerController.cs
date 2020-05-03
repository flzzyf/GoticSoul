using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

struct PlayerInput {
	Vector2 axis;
	Vector2 lastAxis;
	public Vector2 Axis {
		get { return axis; }
		set {
			axis = value;

			if(lastAxis != axis) {
				axisChanged = true;
			}

			lastAxis = axis;
		}
	}

	public bool axisChanged;

	public bool rollButtonPressed;

	public PlayerAction lastAction;

	public void Reset() {
		axis = default;
		rollButtonPressed = false;
		axisChanged = false;
	}
}

public enum PlayerAction {
	Null,
	Attack,
	HeavyAttack,
	Roll,
	Evade,
	//
	UseItem,
}

public class PlayerController : Singleton<PlayerController> {
	PlayerInput input;

	public Unit player;

	void Awake() {
		InitAction();
	}

	void Update(){
		//更新预指令生存期
		UpdatePreaction();

		input.Axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		
		input.rollButtonPressed = input.rollButtonPressed || Input.GetKeyDown(KeyCode.Space);

		if (Input.GetMouseButtonDown(0)) {
			Action(PlayerAction.Attack);
		}

		//面向鼠标点
		if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x) {
			player.Face(1);
		}
		else {
			player.Face(-1);
		}

		for (int i = 0; i < Enum.GetValues(typeof(JoystickButton)).Length; i++) {
			if (Input.GetKeyDown("joystick button " + (int)(Enum.GetValues(typeof(JoystickButton)).GetValue(i)))) {
				Debug.Log(Enum.GetValues(typeof(JoystickButton)).GetValue(i));
			}
		}
	}

	enum JoystickButton { A, B, X, Y, LB, RB, Back, Start, LS, RS}
	enum JoystickAxis {  }

	void FixedUpdate() {
		//玩家无法控制则返回
		if (player.HasFlag(UnitFlag.Uncontrollable)) {
			return;
		}

		try {
			if (input.axisChanged) {
				if (input.Axis != default) {
					player.Move((int)Mathf.Sign(input.Axis.x));
				}
				else {
					player.StopMoving();
				}
			}

			if (input.rollButtonPressed) {
				Action(PlayerAction.Roll);
			}
		}
		finally {
			input.Reset();
		}
	}

	#region 玩家指令

	//玩家操作回调字典
	Dictionary<PlayerAction, Action> actionDic;

	//初始化动作
	void InitAction() {
		actionDic = new Dictionary<PlayerAction, Action>();

		actionDic.Add(PlayerAction.Attack, () => {
			player.UseWeapon();
		});

		actionDic.Add(PlayerAction.Roll, () => {
			if(input.Axis.x != 0) {
				AbilManager.GetAbil<Abil_Target>("Roll").Cast(player, player.pos + Vector2.right * input.Axis.x);
			}
			else {
				int dir = Camera.main.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x ? 1 : -1;
				AbilManager.GetAbil<Abil_Target>("Roll").Cast(player, player.pos + Vector2.right * dir);
			}
		});
	}

	//执行玩家指令
	public void Action(PlayerAction action) {
		//如果玩家可操控
		if (!player.HasFlag(UnitFlag.Uncontrollable)) {
			//执行指令回调
			if (actionDic.ContainsKey(action)) {
				actionDic[action]?.Invoke();
			}
		}
		//如果玩家不可操控
		else {
			SetPreaction(action);
		}
	}

	#endregion

	#region 指令预输入系统

	//预指令
	public PlayerAction preaction;
	//上个预指令剩余时间
	float lastActionLifeTime;

	//默认预指令生存期
	const float preactionLifeTime = 1;

	//设置预指令
	void SetPreaction(PlayerAction action) {
		preaction = action;

		lastActionLifeTime = preactionLifeTime;
	}

	//更新预指令生存期
	void UpdatePreaction() {
		if(preaction != PlayerAction.Null) {
			lastActionLifeTime -= Time.deltaTime;

			if (lastActionLifeTime <= 0) {
				preaction = PlayerAction.Null;
			}
		}
	}

	//当玩家恢复控制
	public static void OnBecomeControllable() {
		//如果有预指令
		if(instance.preaction != PlayerAction.Null) {
			//执行预指令
			instance.Action(instance.preaction);

			//清除预指令
			instance.preaction = PlayerAction.Null;
		}
	}

	#endregion
}
