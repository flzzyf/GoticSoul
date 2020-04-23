using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Buff))]
public class BuffEditor : Editor {
	Buff buff;

	public bool flagFolded = true;

	private void OnEnable() {
		buff = (Buff)target;

		buff.Init();
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		serializedObject.Update();

		flagFolded = EditorGUILayout.Foldout(flagFolded, "UnitFlags");
		if (flagFolded) {
			EditorGUI.indentLevel++;

			EditorGUI.BeginChangeCheck();
			for (int i = buff.flagList.Count - 1; i >= 0; i--) {
				buff.flagList[i].value = EditorGUILayout.Toggle(buff.flagList[i].flag.ToString(), buff.flagList[i].value);

			}
			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(buff);
			}

			EditorGUI.indentLevel--;

		}

		serializedObject.ApplyModifiedProperties();
	}
}
