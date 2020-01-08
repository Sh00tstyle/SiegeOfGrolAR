using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkardInteraction : MonoBehaviour
{
    [SerializeField]
    private HouseButton[] _houses;

    private int _correctHouseIndex;

    private void Awake()
    {
        _correctHouseIndex = Random.Range(0, _houses.Length);
        Debug.Log("Correct house index: " + _correctHouseIndex);

        for(int i = 0; i < _houses.Length; ++i)
            _houses[i].Initialize(i);
    }

    public void CheckForSuccess(int pHouseIndex)
    {
        if (pHouseIndex == _correctHouseIndex)
        {
            Debug.Log("Selected correct house");

            ProgressHandler.Instance.IncreaseStoryProgress();
            SceneHandler.Instance.LoadScene(Scenes.Map);
        }
        else
        {
            // TODO: Give feedback in the dialog box
            Debug.Log("Selected incorrect house");
        }
    }

}
