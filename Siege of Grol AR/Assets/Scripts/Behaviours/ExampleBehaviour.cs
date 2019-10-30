using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleBehaviour : MonoBehaviour
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Example of using the Singleton of the ExampleManager (accessing it from everywhere)
            string exampleString = ExampleManager.Instance.GetExampleString();
            Debug.Log(exampleString);
        }
    }
}
