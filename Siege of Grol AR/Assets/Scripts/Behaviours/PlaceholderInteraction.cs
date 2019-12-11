using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceholderInteraction : Interaction
{
    public override void Activate()
    {
        Debug.Log("Starting example interaction: " + interactionName);
        Debug.Log("Loading AR example scene (build index scene 1...");

        SceneLoader.Instance.LoadScene(1);
    }
}