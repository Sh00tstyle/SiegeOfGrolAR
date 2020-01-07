using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryDecisionFitter : MonoBehaviour
{
    [SerializeField]
    private string _helpSpyText;

    [SerializeField]
    private string _sabotageSpyText;

    private TextMeshProUGUI _tmText;
    private void Awake()
    {
        _tmText = GetComponent<TextMeshProUGUI>();

        AdjustToStoryDecision();
    }

    private void OnEnable()
    {
        AdjustToStoryDecision();
    }

    public void AdjustToStoryDecision()
    {
        if (_tmText == null)
        {
            Debug.LogError("Unable to adjust text, no TextMeshProUGUI component was found on " + gameObject.name);
            return;
        }

        if (ProgressHandler.Instance.IsHelpingSpy)
            _tmText.text = _helpSpyText;
        else
            _tmText.text = _sabotageSpyText;
    }
}
