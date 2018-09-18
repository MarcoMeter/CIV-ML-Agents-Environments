using System;
using UnityEngine;

public class TagSettings : MonoBehaviour
{
    [SerializeField] internal RunnerAgent runner;
    [SerializeField] internal CatcherAgent catcher;
    [SerializeField] internal Transform spawn;

    internal static TagSettings instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            throw new Exception("Double instance of " + GetType().Name);
        }
    }
}
