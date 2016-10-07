using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[CustomEditor(typeof(IcoSphere))]
public class IcoSphereEditor : Editor {
	private const string POLYGONS_VISIBLE = "PolygonsVisible", TRIANGLES_VISIBLE = "TrianglesVisible";
	private IcoSphere icoSphere;
	private bool isPolygonsVisible, isTrianglesVisible;

	public override void OnInspectorGUI () {
		if(EditorPrefs.HasKey(POLYGONS_VISIBLE)) {isPolygonsVisible = EditorPrefs.GetBool(POLYGONS_VISIBLE);}
		if(EditorPrefs.HasKey(POLYGONS_VISIBLE)) {isTrianglesVisible = EditorPrefs.GetBool(TRIANGLES_VISIBLE);}
		icoSphere = target as IcoSphere;
		DrawDefaultInspector();
		isPolygonsVisible = GUILayout.Toggle(isPolygonsVisible, "Show Polygons");
		if (GUI.changed) EditorPrefs.SetBool(POLYGONS_VISIBLE, isPolygonsVisible);

		isTrianglesVisible = GUILayout.Toggle(isTrianglesVisible, "Show Triangles");
		if (GUI.changed) EditorPrefs.SetBool(TRIANGLES_VISIBLE, isTrianglesVisible);

		if(GUILayout.Button("Create")) {
			icoSphere.Create();
		}
		OnSceneGUI ();
	}

	void OnSceneGUI () {
		icoSphere = target as IcoSphere;
		if ( isTrianglesVisible ) {
			DrawTriangles();
		}
		if ( isPolygonsVisible ) {
			DrawPolygons();
		}

	}

	private void DrawPolygons () {
		Handles.color = Color.red;
		foreach (Polygon p in icoSphere.Polygons) {
			for (int i = 0; i < p.Triangles.Count; i++) {
				Vector3 c1 = p.Triangles[i].centeroid;
				Vector3 c2 = p.Triangles[(i+1) % p.Triangles.Count].centeroid;
				if(IsVisible(c1) && IsVisible(c2)) {
					Handles.DrawLine(c1, c2);
				}
			}
		}
	}

	private void DrawTriangles () {
		Handles.color = Color.green;
		foreach (Triangle t in icoSphere.Triangles) {
			Vector3 v1 = icoSphere.Vertices[t.indices[0]];
			Vector3 v2 = icoSphere.Vertices[t.indices[1]];
			Vector3 v3 = icoSphere.Vertices[t.indices[2]];
			if(IsVisible(v1) && IsVisible(v2) && IsVisible(v3)) {
				Handles.DrawLine(v1, v2);
				Handles.DrawLine(v2, v3);
				Handles.DrawLine(v3, v1);
			}
		}
	}

	private bool IsVisible (Vector3 normal) {
		if (Camera.current != null) {
			Vector3 cam = Camera.current.transform.position;
			return Vector3.Dot(cam.normalized, normal.normalized) > .2f;
		}
		return false;
	}
}
