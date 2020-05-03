using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilManager : ScriptableObjectManagerBase<Abil>{
	public static void Init() {
		LoadData();
	}

	//从名称获取技能
	public static Abil GetAbil(string name) {
		return GetData(name) as Abil;
	}
	public static T GetAbil<T>(string name) {
		var a = (T)System.Convert.ChangeType(GetData(name), typeof(T));
		return a;
	}

	//释放技能
	public static void CastAbil(Abil abil, Unit caster) {
		abil.Cast(caster);
	}

	//判断单位有技能
	public static bool HasAbil(Unit unit, Abil abil) {
		return unit.abilList.Contains(abil);
	}
}


