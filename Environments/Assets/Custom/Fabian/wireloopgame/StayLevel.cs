using System;
using UnityEngine;

public class StayLevel : MonoBehaviour
{

    public Axis axis;

    private float initialDegree;


    private void Awake()
    {
        switch (axis)
        {
            case Axis.x:
                initialDegree = transform.eulerAngles.x;
                break;

            case Axis.y:
                initialDegree = transform.eulerAngles.y;
                break;

            case Axis.z:
                initialDegree = transform.eulerAngles.z;
                break;

            default:
                throw new Exception();
        }
    }

    [ExecuteInEditMode]

    private void FixedUpdate()
    {
        switch (axis)
        {
            case Axis.x:
                transform.eulerAngles = new Vector3(
                    initialDegree,
                    transform.eulerAngles.y,
                    transform.eulerAngles.z);
                break;

            case Axis.y:
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    initialDegree,
                    transform.eulerAngles.z);
                break;

            case Axis.z:
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    initialDegree);
                break;

            default:
                throw new Exception();
        }
    }

    public enum Axis
    {
        x,
        y,
        z
    }
}