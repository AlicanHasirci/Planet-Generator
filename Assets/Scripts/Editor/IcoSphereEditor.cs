using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(IcoSphere))]
public class IcoSphereEditor : Editor {

	private IcoSphere icoSphere;

	public override void OnInspectorGUI () {
		icoSphere = target as IcoSphere;
		DrawDefaultInspector();
		if(GUILayout.Button("Create")) {
			icoSphere.Create();
		}
	}
}
