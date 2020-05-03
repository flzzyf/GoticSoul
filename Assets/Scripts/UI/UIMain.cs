using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour{
	public Slider slider_HP;
	public Slider slider_Stamina;

	private void Start() {
		Unit player = GameManager.Player;

		player.attribute_HP.onModifyValue += () => {
			SetSlider(slider_HP, player.attribute_HP.currentPercent);
		};
		player.attribute_Stamina.onModifyValue += () => {
			SetSlider(slider_Stamina, player.attribute_Stamina.currentPercent);
		};

		SetSlider(slider_HP, player.attribute_HP.currentPercent);
		SetSlider(slider_Stamina, player.attribute_Stamina.currentPercent);
	}

	void SetUnit() {

	}

	public void SetSlider(Slider slider, float percent) {
		slider.value = percent;
	}
}
