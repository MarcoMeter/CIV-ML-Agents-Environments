using System;
using System.Collections.Generic;
using UnityEngine;

namespace MLAgents
{

    /// <summary>
    /// Ray perception component. Attach this to agents to enable "local perception"
    /// via the use of ray casts directed outward from the agent. 
    /// </summary>
    public class RayPerceptionOld : MonoBehaviour
    {
        List<float> perceptionBuffer = new List<float>();
        Vector3 endPosition;
        RaycastHit hit;

        /// <summary>
        /// Creates perception vector to be used as part of an observation of an agent.
        /// </summary>
        /// <returns>The partial vector observation corresponding to the set of rays</returns>
        /// <param name="rayDistance">Radius of rays</param>
        /// <param name="rayAngles">Anlges of rays (starting from (1,0) on unit circle).</param>
        /// <param name="detectableObjects">List of tags which correspond to object types agent can see</param>
        /// <param name="startOffset">Starting heigh offset of ray from center of agent.</param>
        /// <param name="endOffset">Ending height offset of ray from center of agent.</param>
        public List<float> Perceive(
            float rayDistance,
            float sphereCastRadius,
            float[] rayAngles,
            string[] detectableObjects,
            float startOffset,
            float endOffset)
        {
            perceptionBuffer.Clear();

            // For each ray sublist stores categorial information on detected object
            // along with object distance.
            foreach (float angle in rayAngles)
            {
                endPosition = transform.TransformDirection(PolarToCartesian(rayDistance, angle));
                endPosition.y = endOffset;

                if (Application.isEditor)
                {
                    Debug.DrawRay(transform.position + new Vector3(0f, startOffset, 0f), endPosition, Color.black, 0.01f, true);
                }

                float[] subList = new float[detectableObjects.Length + 2];

                if (Physics.SphereCast(
                    transform.position + new Vector3(0f, startOffset, 0f),
                    sphereCastRadius,
                    endPosition,
                    out hit,
                    rayDistance))
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            break;
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }

                perceptionBuffer.AddRange(subList);
            }

            return perceptionBuffer;
        }

        public List<float> Perceive(
            float rayLength,
            float rayThickness,
            Ray[] rays,
            string[] detectableObjects,
            LayerMask physicsLayer,
            float startOffset,
            float endOffset)
        {
            // "One hot fashion"

            perceptionBuffer.Clear();

            foreach (Ray ray in rays)
            {
                float[] subList = new float[detectableObjects.Length + 2];

                if (Physics.SphereCast(ray.origin, rayThickness, ray.direction, out hit, rayLength, physicsLayer))
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayLength;
                            break;
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }

                if (Application.isEditor)
                {
                    Debug.DrawLine(hit.point, hit.point + (Vector3.up / 4f));
                    Debug.DrawLine(ray.origin, ray.origin + (ray.direction * rayLength), Color.black, 0.01f, true);
                }

                perceptionBuffer.AddRange(subList);
            }

            return perceptionBuffer;
        }

        /// <summary>
        /// Creates perception vector to be used as part of an observation of an agent.
        /// </summary>
        /// <returns>The partial vector observation corresponding to the set of rays</returns>
        /// <param name="rayDistance">Radius of rays</param>
        /// <param name="rayAngles">Anlges of rays (starting from (1,0) on unit circle).</param>
        /// <param name="detectableObjects">List of tags which correspond to object types agent can see</param>
        /// <param name="startOffset">Starting heigh offset of ray from center of agent.</param>
        /// <param name="endOffset">Ending height offset of ray from center of agent.</param>
        public List<float> Perceive(float rayDistance,
            float[] rayAngles, string[] detectableObjects,
            float startOffset, float endOffset)
        {
            perceptionBuffer.Clear();
            // For each ray sublist stores categorial information on detected object
            // along with object distance.
            foreach (float angle in rayAngles)
            {
                endPosition = transform.TransformDirection(
                    PolarToCartesian(rayDistance, angle));
                endPosition.y = endOffset;
                if (Application.isEditor)
                {
                    //Debug.DrawRay(transform.position + new Vector3(0f, startOffset, 0f), endPosition, Color.black, 0.01f, true);
                }

                float[] subList = new float[detectableObjects.Length + 2];
                if (Physics.SphereCast(transform.position +
                                       new Vector3(0f, startOffset, 0f), 0.5f,
                    endPosition, out hit, rayDistance))
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            break;
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }

                perceptionBuffer.AddRange(subList);
            }

            return perceptionBuffer;
        }

        public List<float> Perceive(float rayDistance, Vector3[] offsets, Vector3[] rays, string[] detectableObjects)
        {
            perceptionBuffer.Clear();
            int length = offsets.Length;

            for(int n = 0; n < length; n++)
            {
                if (Application.isEditor)
                {
                    //Debug.DrawRay(transform.position + offsets[n], rays[n] * rayDistance, Color.black, 0.01f, true);
                }

                float[] subList = new float[detectableObjects.Length + 2];
                if(Physics.SphereCast(transform.position + offsets[n], 0.5f, rays[n], out hit, rayDistance))
                {
                    for(int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            break;
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }

                perceptionBuffer.AddRange(subList);
            }

            return perceptionBuffer;
        }

        /// <summary>
        /// Converts polar coordinate to cartesian coordinate.
        /// </summary>
        public static Vector3 PolarToCartesian(float radius, float angle)
        {
            float x = radius * Mathf.Cos(DegreeToRadian(angle));
            float z = radius * Mathf.Sin(DegreeToRadian(angle));
            return new Vector3(x, 0f, z);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        public static float DegreeToRadian(float degree)
        {
            return degree * Mathf.PI / 180f;
        }
    }
}
