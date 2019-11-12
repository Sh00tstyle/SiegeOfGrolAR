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

        MeshRenderer renderer = GetComponentInParent<MeshRenderer>();

        Color startColor = renderer.material.GetColor("_BaseColor");
        Color endColor = Color.blue;
        Color lerpColor = startColor;

        float lerpSpeed = 0.5f;
        float incrementor = 0.0f;

        yield return null;

        while (incrementor < 1.0f)
        {
            incrementor += Time.deltaTime * lerpSpeed;
            incrementor = Mathf.Clamp01(incrementor);

            lerpColor = Color.Lerp(startColor, endColor, incrementor);

            renderer.material.SetColor("_BaseColor", lerpColor);

            yield return null;
        }

        GameManager.Instance.NextLocation();
    }
}
