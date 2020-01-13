using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HouseButton : Button
{
    private DrunkardInteraction _interaction;
    private int _index;

    protected override void Awake()
    {
        base.Awake();

        _interaction = GetComponentInParent<DrunkardInteraction>();

        if (_interaction == null)
            Debug.LogError("Interaction in " + name + " could not be found");
    }

    public void Initialize(int pHouseIndex)
    {
        _index = pHouseIndex;
    }

    public void ActivateButton()
    {
        if (_interaction.InDialog)
            return;

        _interaction.CheckForSuccess(_index);
    }

}
