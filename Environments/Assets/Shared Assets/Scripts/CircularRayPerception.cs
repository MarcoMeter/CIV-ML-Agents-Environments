using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLAgents
{
    public class CircularRayPerception : RayPerceptionOld
    {
        public int horizontalRays = 10;
        public int verticalRays = 3;
        public float verticalAngle = 0.2f;

        public float distance = 0.5f;
        public float radius = 1f;

        // Use this for initialization
        public List<float> Perceive(string[] detectableObjects)
        {
            List<Vector3> offsets = new List<Vector3>();
            List<Vector3> rays = new List<Vector3>();

            Matrix4x4 m = Matrix4x4.Rotate(transform.rotation);
            for (int i = 0; i < horizontalRays; i++)
            {
                float angle = ((Mathf.PI * 2f) / horizontalRays) * i;
                Vector3 pos = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                pos = m.MultiplyPoint3x4(pos);
                Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.LookRotation(pos.normalized, transform.up));

                for (int y = 0; y < verticalRays; y++)
                {
                    float angleY = (((Mathf.PI * verticalAngle) / (verticalRays - 1f)) * y) - (Mathf.PI * verticalAngle * 0.5f);
                    Vector3 nPos = new Vector3(0, Mathf.Sin(angleY), Mathf.Cos(angleY));
                    nPos = rot.MultiplyPoint3x4(nPos);

                    offsets.Add(pos.normalized * radius);
                    rays.Add(-nPos.normalized);
                }
            }

            return Perceive(distance, offsets.ToArray(), rays.ToArray(), detectableObjects);
        }
        
        // Update is called once per frame
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Matrix4x4 m = Matrix4x4.Rotate(transform.rotation);

            for (int i = 0; i < horizontalRays; i++)
            {
                float angle = ((Mathf.PI * 2f) / horizontalRays) * i;
                Vector3 pos = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                pos = m.MultiplyPoint3x4(pos);
                Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.LookRotation(pos.normalized, transform.up));

                for (int y = 0; y < verticalRays; y++)
                {
                    float angleY = (((Mathf.PI * verticalAngle) / (verticalRays - 1f)) * y) - (Mathf.PI * verticalAngle * 0.5f);
                    Vector3 nPos = new Vector3(0, Mathf.Sin(angleY), Mathf.Cos(angleY));
                    nPos = rot.MultiplyPoint3x4(nPos);
                    Gizmos.DrawRay(transform.position + pos.normalized * radius, -nPos.normalized * distance);
                }
            }
        }
    }
}