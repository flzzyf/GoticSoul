using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton <GameManager>{
	public Unit player;
	public static Unit Player {
		get {
			return instance.player;
		}
	}

	private void Awake() {
		//初始化各个系统
		AbilManager.Init();
		BuffManager.Init();
		WeaponManager.Init();
		ItemManager.Init();
	}

	private void Update() {
		if (Input.GetKeyDown("f")) {
			AbilManager.CastAbil(AbilManager.GetAbil("Attack"), player);
		}
		if (Input.GetKeyDown("1")) {
			Debug.Log(WeaponManager.GetWeapon("Sword").Damage);
		}
	}

	//重新加载场景
	public void ReloadScene() {
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}
