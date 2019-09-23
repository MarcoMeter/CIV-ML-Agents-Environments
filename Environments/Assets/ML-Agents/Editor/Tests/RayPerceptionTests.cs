using UnityEngine;
using NUnit.Framework;


namespace MLAgents.Tests
{
    public class RayPerceptionTests : MonoBehaviour
    {
        [Test]
        public void TestPerception3D()
        {
            var angles = new[] {0f, 90f, 180f};
            var tags = new[] {"test", "test_1"};
            
            var go = new GameObject("MyGameObject");
            RayPerception rayPer3D = go.AddComponent<RayPerception>();
            var result = rayPer3D.Perceive(1f, angles ,
                tags, 0f, 0f);
            Debug.Log(result.Count);
            Assert.IsTrue(result.Count == angles.Length * (tags.Length + 2));
        }

        [Test]
        public void TestPerception2D()
        {
            /*var angles = new[] {0f, 90f, 180f};
            var tags = new[] {"test", "test_1"};
            
            var go = new GameObject("MyGameObject");
            RayPerception rayPer2D = go.AddComponent<RayPerception>();
            var result = rayPer2D.Perceive(1f, angles,
                tags);
            Debug.Log(result.Count);
            Assert.IsTrue(result.Count == angles.Length * (tags.Length + 2));*/
        }
    }
}
