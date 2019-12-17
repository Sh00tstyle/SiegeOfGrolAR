using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCarousel : MonoBehaviour
{
    [SerializeField] CharacterPanel[] _panels;
    [SerializeField] Sprite _unlockedSprite, _lockedSprite;

    [System.Serializable]
    struct CharacterPanel
    {
        public Image progressImage;
        public UIEffect blurEffect;
        public TextMeshProUGUI name;
        public TextMeshProUGUI text;
    }

    private void Awake()
    {
        int locationIndex = ProgressHandler.Instance.StoryProgress;

        // Also needs some optimisation
        for (int i = 0; i < _panels.Length; ++i)
        {
            CharacterPanel panel = _panels[i];

            // Set progress sprites, if it's equal to index don't show anything.
            panel.progressImage.gameObject.SetActive(i != locationIndex);
            panel.progressImage.sprite = i < 1 ? _unlockedSprite : _lockedSprite;

            // Set blur only to those who are active
            panel.blurEffect.enabled = i > locationIndex;

            // Set text to "?" when it is outside of the index
            if (i > locationIndex)
            {
                panel.name.gameObject.SetActive(false);
                panel.text.gameObject.SetActive(false);
            }
               
        }
    }





}
