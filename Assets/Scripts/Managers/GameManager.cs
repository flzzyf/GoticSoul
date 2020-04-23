using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
	public Unit player;

	public Effect effect;

	public static GameManager instnace;

	private void Awake() {
		instnace = this;

		//初始化各个系统
		AbilManager.Init();
		BuffManager.Init();
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

	public void ReloadScene() {
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	IEnumerator qwe() {
		yield return new WaitForSeconds(1);
	}
}
