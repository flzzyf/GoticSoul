using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilManager : ManagerBase<Abil>{
	//从名称获取技能
	public static Abil GetAbil(string name) {
		return GetData(name) as Abil;
	}

	//释放技能
	public static void CastAbil(Abil abil, Unit caster) {

	}
}


