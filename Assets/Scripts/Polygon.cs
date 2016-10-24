using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Polygon {

	public int index;
	private List<Triangle> triangles = new List<Triangle>(6);

	public List<Triangle> Triangles {get {return triangles;}}

	public Polygon (int index) {
		this.index = index;
	}

	public void AddTriangle (Triangle triangle) {
		triangles.Add(triangle);
		triangle.polygons.Add(this);
	}

	public void RemoveTriangle (Triangle triangle) {
		triangles.Remove(triangle);
		triangle.polygons.Remove(this);
	}

	public void RelateTriangles () {
		LinkedList<Triangle> trigs = new LinkedList<Triangle>();
		int nextIndex = 0;
		trigs.AddFirst(triangles[nextIndex]);
		while (trigs.Count <= triangles.Count) {
			nextIndex = FindNextIndex(nextIndex);
			trigs.AddLast(triangles[nextIndex]);
		}
		triangles.Clear();
		triangles.AddRange(trigs);
	}

	public int FindNextIndex (int index) {
		Triangle trig = triangles[index];
		int ci = trig.indices.FindIndex(i => i == this.index);
		int ni = (ci - 1).Mod(3);
		int sharedIndex = trig.indices[ni];
		int nextIndex = triangles.FindIndex(t => t.indices.Contains(sharedIndex) && !t.Equals(trig));
		return nextIndex;
	}
} 