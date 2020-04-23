using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;

[CustomEditor(typeof(Effect_Set))]
public class Effect_SetEditor : Editor {
	ReorderableList effectList;

	private void OnEnable() {
		effectList = new ReorderableList(serializedObject.FindProperty("effectList"), true, true, true);
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		serializedObject.Update();

		effectList.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}
}
