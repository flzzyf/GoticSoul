using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ManagerBase<T> {
	protected static Dictionary<string, object> dataDic;

	public static void Init() {
		dataDic = new Dictionary<string, object>();

		//读取技能
		var guids = AssetDatabase.FindAssets("", new[] { "Assets/Resources/ScriptableObjects/" + typeof(T) });
		foreach (var item in guids) {
			var path = AssetDatabase.GUIDToAssetPath(item);
			string name = Path.GetFileNameWithoutExtension(path);
			object data = Resources.Load(string.Format("ScriptableObjects/{0}/{1}", typeof(T), name));
			dataDic.Add(name, data);
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
