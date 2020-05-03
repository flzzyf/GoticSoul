using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManagerBase<T> {
	protected static Dictionary<string, object> dataDic;

	public static void LoadData() {
		dataDic = new Dictionary<string, object>();

		//读取
		foreach (var item in Resources.LoadAll("ScriptableObjects/" + typeof(T))) {
			dataDic.Add(item.name, item);
		}
	}

	//从名称获取技能
	protected static object GetData(string name) {
		if (dataDic.ContainsKey(name))
			return dataDic[name];

		Debug.LogError("未能找到数据：" + name);
		return default;
	}
}
