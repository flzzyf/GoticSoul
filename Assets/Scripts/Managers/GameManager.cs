using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
	public Unit player;

	public Effect effect;

	private void Awake() {
		//初始化各个系统
		AbilManager.Init();
		BuffManager.OnInit();
	}

	private void Update() {
		if (Input.GetKeyDown("f")) {
			AbilManager.GetAbil("Roll").Cast(player);
		}
		if (Input.GetKeyDown("1")) {
			//BuffManager.AddBuff(player, BuffManager.GetBuff("Roll"));

			EffectManager.GetEffectInstance(effect).Trigger(player);
		}
	}
}
