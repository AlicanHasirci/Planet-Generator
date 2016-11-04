using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class Triangle {
    public List<int> indices;
    public List<Triangle> neighbours;
	public List<Polygon> polygons;
	public Vector3 centeroid;

    public Triangle(int v1, int v2, int v3) {
        this.indices = new List<int>() { v1, v2, v3 };
        this.neighbours = new List<Triangle>(3);
		this.polygons = new List<Polygon>(3);
    }

    public int this[int i] {
        get {
            return indices[i];
        }
    }

    public List<int> SharedIndices(Triangle t2) {
        return indices.FindAll(i => t2.indices.Contains(i));
    }

    public static void AddNeighbours(Triangle t1, Triangle t2) {
        if (!t1.neighbours.Contains(t2)) t1.neighbours.Add(t2);
        if (!t2.neighbours.Contains(t1)) t2.neighbours.Add(t1);
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        foreach (int i in indices) sb.Append(i).Append(' ');
        return sb.ToString();
    }

    public static void Perturb(ref Triangle t1, ref Triangle t2) {
		List<int> shared = t1.SharedIndices(t2);
		    
		int t1ExclusiveIndex = t1.indices.FindIndex(i => !shared.Contains(i));
		int t2ExclusiveIndex = t2.indices.FindIndex(i => !shared.Contains(i));
		int t1SwapIndex = t1.indices.FindIndex(i => i == shared[0]);
		int t2SwapIndex = t2.indices.FindIndex(i => i == shared[1]);
		int t1Exclusive = t1[t1ExclusiveIndex];
		int t2Exclusive = t2[t2ExclusiveIndex];
		int t1Swap = t1.indices[t1SwapIndex];
		int t2Swap = t2.indices[t2SwapIndex];

		// Find Polygons to be affected
		Polygon t1ExcPoly = t1.polygons.Find(p => p.index == t1Exclusive);
		Polygon t2ExcPoly = t2.polygons.Find(p => p.index == t2Exclusive);
		Polygon t1SidePoly = t1.polygons.Find(p => p.index == t1Swap);
		Polygon t2SidePoly = t2.polygons.Find(p => p.index == t2Swap);

		// Check For Outcome
		if(
			(t1ExcPoly != null && t1ExcPoly.Triangles.Count == 7) || 
			(t2ExcPoly != null && t2ExcPoly.Triangles.Count == 7) ||
			(t1SidePoly != null && t1SidePoly.Triangles.Count == 5) ||
			(t2SidePoly != null && t2SidePoly.Triangles.Count == 5)) {
			return;
		}
		// Apply Changes To Polygons
		if (t1ExcPoly != null) t1ExcPoly.AddTriangle(t2);
		if (t2ExcPoly != null) t2ExcPoly.AddTriangle(t1);
		if (t1SidePoly != null) t1SidePoly.RemoveTriangle(t1);
		if (t2SidePoly != null) t2SidePoly.RemoveTriangle(t2);

		//Continue Perturbing
		t1.indices[t1SwapIndex] = t2Exclusive;
		t2.indices[t2SwapIndex] = t1Exclusive;


		// Apply Neighbour Changes
        Triangle t1Temp = t1;
		Triangle t2Temp = t2;
		int t1OldNeighbourIndex = t1.neighbours.FindIndex(n => (!n.Equals(t2Temp) && n.indices.Contains(shared[0])));
		int t2OldNeighbourIndex = t2.neighbours.FindIndex(n => (!n.Equals(t1Temp) && n.indices.Contains(shared[1])));
		Triangle t1OldNeighbour = t1.neighbours[t1OldNeighbourIndex];
		Triangle t2OldNeighbour = t2.neighbours[t2OldNeighbourIndex];
		t1.neighbours[t1OldNeighbourIndex] = t2OldNeighbour;
		t2.neighbours[t2OldNeighbourIndex] = t1OldNeighbour;
        int t1IndexOnNeighbour = t1OldNeighbour.neighbours.FindIndex(n => n.Equals(t1Temp));
        int t2IndexOnNeighbour = t2OldNeighbour.neighbours.FindIndex(n => n.Equals(t2Temp));
        t1OldNeighbour.neighbours[t1IndexOnNeighbour] = t2;
        t2OldNeighbour.neighbours[t2IndexOnNeighbour] = t1;
    }


	public override bool Equals (object obj) {
		if(obj is Triangle) {
			Triangle t = (Triangle) obj;
			return this[0] == t[0] && this[1] == t[1] && this[2] == t[2];
		} else {
			return base.Equals (obj);
		}
	}
}
