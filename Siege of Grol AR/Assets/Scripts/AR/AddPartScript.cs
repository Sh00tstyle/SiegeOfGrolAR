using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPartScript : MonoBehaviour
{
    //public GameObject Part1;
    //public GameObject Part2;
    //public GameObject Part3;

    public GameObject Wheels;
    public GameObject Trail;
    public GameObject Axle;
    public GameObject Reinforce;
    public GameObject Chase;
    public GameObject Cascable;

    public GameObject WheelsUnTextured;
    public GameObject TrailUnTextured;
    public GameObject AxleUnTextured;
    public GameObject ReinforceUnTextured;
    public GameObject ChaseUnTextured;
    public GameObject CascableUnTextured;

    public GameObject PanelWrongPart;
    public GameObject PanelCompletion;

    [SerializeField]
    private GameObject CanonUnTextured;

    public GameObject[] PartOrder;

    int ObjectIndex = 0;

    void OnCollisionEnter(Collision col)
    {
        bool isCorrectObject = col.gameObject == PartOrder[ObjectIndex];
        if (isCorrectObject)
        {
            //Plaatsen
            if (ObjectIndex == 0)
            {
                // Part 0
                Wheels.SetActive(true);
                WheelsUnTextured.SetActive(false);
                AudioManager.Instance.Play("PartCorrect");
                Debug.Log("Part 1 detected! +1 to ObjectIndex");
            }
            else if (ObjectIndex == 1)
            {
                Trail.SetActive(true);
                TrailUnTextured.SetActive(false);
                AudioManager.Instance.Play("PartCorrect");
                Debug.Log("Part 2 detected! +1 to ObjectIndex");
            }
            else if (ObjectIndex == 2)
            {
                Axle.SetActive(true);
                AxleUnTextured.SetActive(false);
                AudioManager.Instance.Play("PartCorrect");
                Debug.Log("Part 3 detected! +1 to ObjectIndex");               
                //SceneHandler.Instance.LoadSceneWithDelay(0, 3.0f);
            }
            else if (ObjectIndex == 3)
            {
                Reinforce.SetActive(true);
                ReinforceUnTextured.SetActive(false);
                AudioManager.Instance.Play("PartCorrect");
                Debug.Log("Part 4 detected! +1 to ObjectIndex");
            }
            else if (ObjectIndex == 4)
            {
                Chase.SetActive(true);
                ChaseUnTextured.SetActive(false);
                AudioManager.Instance.Play("PartCorrect");
                Debug.Log("Part 5 detected! +1 to ObjectIndex");
            }
            else if (ObjectIndex == 5)
            {
                Cascable.SetActive(true);
                CascableUnTextured.SetActive(false);
                AudioManager.Instance.Play("PartCorrect");
                Debug.Log("Part 6 detected! +1 to ObjectIndex");
                Invoke("CompletedAssembly", 2.0f);
            }
            if (ObjectIndex < PartOrder.Length)
            {
                ObjectIndex++;
                col.transform.parent.gameObject.SetActive(false);
                Destroy(col.gameObject);
            }    
        }
        else
        {
            // Error
            AudioManager.Instance.Play("PartWrong");
            Debug.Log("This is the wrong part");
            PanelWrongPart.SetActive(true);
        }
    }

    void CompletedAssembly()
    {
        AudioManager.Instance.Play("CannonCompleted");
        PanelCompletion.SetActive(true);
    }

    public void DismissSuccessMessage()
    {
        ProgressHandler.Instance.IncreaseStoryProgress();
        SceneHandler.Instance.LoadScene(Scenes.Map);
    }
}
