using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Polygon {

	private Vector3 center;
	private List<Triangle> triangles = new List<Triangle>(6);

	public List<Triangle> Triangles {get {return triangles;}}

	public Polygon (Vector3 center) {
		this.center = center;
	}

	public void AddTriangle (Triangle triangle) {
		triangles.Add(triangle);
	}

	public void RelateTriangles () {
		LinkedList<Triangle> trigs = new LinkedList<Triangle>();
		trigs.AddFirst(triangles[0]);
		LinkedListNode<Triangle> currentNode = trigs.First;
		while (trigs.Count <= triangles.Count) {
			Triangle current = currentNode.Value;
			Triangle next = current.neighbours.Find(t => {
				if (currentNode.Previous  == null) {
					return !t.Equals(currentNode.Value) && triangles.Contains(t);
				} else {
					return !t.Equals(currentNode.Previous.Value) && !t.Equals(currentNode.Value) &&  triangles.Contains(t);
				}
			});
			if (next == null) break;
			else trigs.AddLast(next);
			currentNode = trigs.Last;
		}
		triangles.Clear();
		triangles.AddRange(trigs);
	}

}
