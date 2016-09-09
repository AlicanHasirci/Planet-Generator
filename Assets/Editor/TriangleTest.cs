using UnityEngine;
using UnityEditor;
using NUnit.Framework;

/// <summary>
/// 				7
/// 			   /\
/// 			  /  \
/// 			 / n3 \
///    5_______2/______\4
/// 	\	   /\	   /\
/// 	 \ n1 /  \ t2 /  \
/// 	  \  / t1 \  / n4 \
///		   \/______\/______\
/// 	   1\      /3		8
/// 	     \ n2 /
/// 		  \	 /  
/// 		   \/
/// 			6
/// </summary>
public class TriangleTest {

	Triangle t1,t2,n1,n2,n3,n4;
	[SetUp]
	public void SetUp () {
		t1 = new Triangle(1,2,3);
		t2 = new Triangle(2,4,3);

		n1 = new Triangle(5,2,1);
		n2 = new Triangle(1,3,6);
		n3 = new Triangle(2,7,4);
		n4 = new Triangle(3,4,8);

		Triangle.AddNeighbours(t1,t2);
		Triangle.AddNeighbours(t1,n1);
		Triangle.AddNeighbours(t1,n2);
		Triangle.AddNeighbours(t2,n3);
		Triangle.AddNeighbours(t2,n4);
	}

	[Test]
	public void TrianglesHaveCorrectNumberOfNeighbours () {
		Assert.AreEqual(3, t1.neighbours.Count);
		Assert.AreEqual(3, t2.neighbours.Count);
		Assert.AreEqual(1, n1.neighbours.Count);
		Assert.AreEqual(1, n2.neighbours.Count);
		Assert.AreEqual(1, n3.neighbours.Count);
		Assert.AreEqual(1, n4.neighbours.Count);
	}

	[Test]
	public void NeighboursHaveTwoSharedIndices () {
		AssertNeighbourIndiceCount(t1);
		AssertNeighbourIndiceCount(t2);
		AssertNeighbourIndiceCount(n1);
		AssertNeighbourIndiceCount(n2);
		AssertNeighbourIndiceCount(n3);
		AssertNeighbourIndiceCount(n4);
	}

	[Test]
	public void ReconstructSwapsEdgesCorrectly () {
		Triangle.Reconstruct(ref t1, ref t2);
		AssertTriangleIndices(t1, 1,2,4);
		AssertTriangleIndices(t2, 1,3,4);
	}

	private void AssertNeighbourIndiceCount(Triangle t) {
		foreach (Triangle n in t.neighbours) {
			Assert.AreEqual(2, t.SharedIndices(n).Count);
		}
	}

	private void AssertTriangleIndices(Triangle t, int i1, int i2, int i3) {
		Assert.IsTrue(t.indices.Contains(i1));
		Assert.IsTrue(t.indices.Contains(i2));
		Assert.IsTrue(t.indices.Contains(i3));
		Assert.AreEqual(3, t.indices.Count);
	}
}
