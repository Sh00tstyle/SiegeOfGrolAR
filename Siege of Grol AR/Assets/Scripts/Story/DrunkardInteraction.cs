using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrunkardInteraction : MonoBehaviour
{
    [SerializeField]
    private HouseButton[] _houseButtons;

    [SerializeField]
    private Image[] _houseImages;

    [SerializeField]
    private Sprite[] _houseSprites;

    [SerializeField]
    private string[] _hintTemplates;

    [SerializeField]
    private GameObject _dialogCanvas;

    [SerializeField]
    private TextMeshProUGUI _dialogTextContainer;

    [SerializeField]
    private CharacterDialog.Narration[] _introNarration;

    [SerializeField]
    private CharacterDialog.Narration[] _wrongFeedbackNarration;

    [SerializeField]
    private CharacterDialog.Narration[] _rightFeedbackNarration;

    [SerializeField]
    private GameObject _hintCanvas;

    [SerializeField]
    private TextMeshProUGUI _hintTextContainer;

    private int _correctHouseIndex;
    private string[] _hints;
    private int _shownHints;

    private CharacterDialog.Narration[] _currentNarration;
    private System.Action _currentNarrationAction;
    private int _currentNarrationIndex;

    private bool _inDialog;

    private void Awake()
    {
        SetupInteraction();
    }

    public void SetDialogCanvasVisibility(bool pVisibility)
    {
        _dialogCanvas.SetActive(pVisibility);
        _hintCanvas.SetActive(!pVisibility);

        _inDialog = pVisibility;
    }

    public void CheckForSuccess(int pHouseIndex)
    {
        SetDialogCanvasVisibility(true);

        if (pHouseIndex == _correctHouseIndex)
        {
            StartDialog(_rightFeedbackNarration, () => { 
                ProgressHandler.Instance.IncreaseStoryProgress();
                AudioManager.Instance.StopPlaying("DrunkGameBG");
                AudioManager.Instance.Play("GameBG");
                SceneHandler.Instance.LoadScene(Scenes.Map);
            });
        }
        else
        {
            StartDialog(_wrongFeedbackNarration, () =>
            {
                if (_shownHints < _hints.Length)
                    _dialogTextContainer.text = _hints[_shownHints].ToUpper();
                else
                    _dialogTextContainer.text = _hintTemplates[_hintTemplates.Length - 1].ToUpper();

                ShowNextHint();
            });
        }
    }

    public void AdvanceDialog()
    {
        _currentNarrationIndex++;

        if (_currentNarrationIndex < _currentNarration.Length) // Next text
        {
            _dialogTextContainer.text = _currentNarration[_currentNarrationIndex].Text.ToUpper();
        }
        else if (_currentNarrationIndex >= _currentNarration.Length) // No more text left
        {
            if (_currentNarrationAction != null)
            {
                _currentNarrationAction();
                _currentNarrationAction = null;
            }
            else
            {
                SetDialogCanvasVisibility(false);
            }
        }
    }

    private void SetupInteraction()
    {
        // Reset amount of shown hints
        _shownHints = 0;
        _inDialog = false;

        // Setup a random order for assigning the sprites to the buttons (without duplicates)
        List<int> rndSequence = GetRandomSequence(_houseSprites.Length);

        // Assign the sprites to the images on the buttons
        for (int i = 0; i < _houseImages.Length; ++i)
        {
            _houseImages[i].sprite = _houseSprites[rndSequence[i]];
        }

        // Decide which button holds the correct house
        _correctHouseIndex = Random.Range(0, _houseButtons.Length);

        // Initialize buttons
        for (int i = 0; i < _houseButtons.Length; ++i)
        {
            _houseButtons[i].Initialize(i);
        }

        // Extract the hints from the sprite of the correct button
        string rawHints = _houseImages[_correctHouseIndex].sprite.name;
        string[] splitHints = rawHints.Split('-');

        _hints = new string[3]; // Show 3 hints at most
        rndSequence = GetRandomSequence(_hints.Length);

        for(int i = 0; i < _hints.Length; ++i)
        {
            string finalHint = _hintTemplates[rndSequence[i]];
            _hints[i] = finalHint.Replace("{0}", splitHints[rndSequence[i]]);
        }

        // Enable the dialogue box for the introduction
        ShowNextHint();
        StartDialog(_introNarration, () =>
        {
            _dialogTextContainer.text = _hints[0].ToUpper();
        });
    }

    private void StartDialog(CharacterDialog.Narration[] pNarration, System.Action pAction = null)
    {
        SetDialogCanvasVisibility(true);

        _currentNarration = pNarration;
        _currentNarrationAction = pAction;
        _currentNarrationIndex = 0;

        _dialogTextContainer.text = _currentNarration[_currentNarrationIndex].Text.ToUpper();
    }

    private List<int> GetRandomSequence(int pMaxRange)
    {
        List<int> rndSequence = new List<int>();

        while (rndSequence.Count < pMaxRange)
        {
            int rndIndex = Random.Range(0, pMaxRange);

            if (rndSequence.Contains(rndIndex))
                continue;

            rndSequence.Add(rndIndex);
        }

        return rndSequence;
    }

    private void ShowNextHint()
    {
        if (_shownHints >= _hints.Length)
            return;

        _hintTextContainer.text += (_shownHints + 1) + ". " + _hints[_shownHints].ToUpper() + "\n";
        ++_shownHints;
    }

    public bool InDialog
    {
        get
        {
            return _inDialog;
        }
    }
}
