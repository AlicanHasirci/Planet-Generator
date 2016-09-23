using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class IcoSphereTest {

	private IcoSphere icoSphere;

	[SetUp]
    public void EditorTest() {
        var gameObject = new GameObject();
		icoSphere = gameObject.AddComponent<IcoSphere>();
		icoSphere.recursionLevel = 1;
		icoSphere.Create();
    }

	[Test]
	public void EightyFacesExist () {
		Assert.AreEqual(80, icoSphere.Triangles.Count);
	}
}
