using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    public string interactionName;

    public abstract void Activate();
}
