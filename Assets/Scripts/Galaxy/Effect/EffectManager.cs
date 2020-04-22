using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager{
    public static Effect GetEffectInstance(Effect effect) {
		Effect e = GameObject.Instantiate(effect);

		return e;
	}
}
