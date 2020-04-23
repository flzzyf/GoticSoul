using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PlayerInput {
	public Vector2 axis;
	public Vector2 lastAxis;

	public bool rollButtonPressed;

	public void Reset() {
		lastAxis = axis;
		axis = default;
		rollButtonPressed = false;
	}
}

public enum PlayerAction {
	Attack,
	HeavyAttack,
	Roll,
	//
	UseItem,
}

public class PlayerController : MonoBehaviour{
	PlayerInput input;

	public Unit unit;

	void Update(){
		if (!unit.HasFlag(UnitFlag.Controllable)){
			return;
		}

		input.axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		input.rollButtonPressed = input.rollButtonPressed || Input.GetKeyDown(KeyCode.Space);

		if (Input.GetMouseButtonDown(0)) {
			unit.Attack();
		}
    }

	void FixedUpdate() {
		try {
			if(input.axis != input.lastAxis) {
				if (input.axis != default) {
					unit.Move((int)Mathf.Sign(input.axis.x));
				}
				else {
					unit.StopMoving();
				}
			}

			if (input.rollButtonPressed) {
				//unit.Roll();
				//unit.WolfSword();

				AbilManager.CastAbil(AbilManager.GetAbil("Roll"), unit);
			}
		}
		finally {
			input.Reset();
		}
	}
}
