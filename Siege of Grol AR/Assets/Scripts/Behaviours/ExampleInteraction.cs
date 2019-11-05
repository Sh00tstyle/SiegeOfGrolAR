using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleInteraction : Interaction
{
    private Coroutine _interactionRoutine;

    public override void Activate()
    {
        Debug.Log("Example Interaction of object " + gameObject.name);

        _interactionRoutine = StartCoroutine(InteractionRoutine());
    }

    private IEnumerator InteractionRoutine()
    {
        Debug.Log("Starting example interaction coroutine");

        yield return new WaitForSeconds(3.0f);

        GameManager.Instance.NextLocation();
    }
}
