using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleManager : Singleton<ExampleManager>
{
    protected override void Initialize()
    {

    }

    private void Awake()
    {
        // Use this for initialization
        Debug.Log("Hello from the ExampleManager");
    }

    public string GetExampleString()
    {
        return "Kappa123";
    }
}