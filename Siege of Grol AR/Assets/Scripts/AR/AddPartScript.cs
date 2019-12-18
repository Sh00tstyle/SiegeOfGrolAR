using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPartScript : MonoBehaviour
{
    //public GameObject Part1;
    //public GameObject Part2;
    //public GameObject Part3;

    public GameObject BarrelTextured;
    public GameObject CartTextured;
    public GameObject Wheel1Textured;
    public GameObject Wheel2Textured;
    public GameObject Wheel3Textured;
    public GameObject Wheel4Textured;

    public GameObject BarrelUnTextured;
    public GameObject CartUnTextured;
    public GameObject Wheel1UnTextured;
    public GameObject Wheel2UnTextured;
    public GameObject Wheel3UnTextured;
    public GameObject Wheel4UnTextured;

    public GameObject PanelWrongPart;
    public GameObject PanelCompletion;
    public GameObject CrossHair;
    public GameObject ManipulationButton;

    [SerializeField]
    private GameObject CanonUnTextured;

    public GameObject[] PartOrder;

    int ObjectIndex = 0;

    

    //private GameObject manipulator;

    //GameObject CrosshairManipulator;
    //CrosshairManipulator scriptCrossMani;

    void OnCollisionEnter(Collision col)
    {

        //CrosshairManipulator = GameObject.Find("CrosshairManipulator");
        //scriptCrossMani = CrosshairManipulator.GetComponent<CrosshairManipulator>();

        if (ObjectIndex == 0 && col.gameObject == PartOrder[ObjectIndex])
        {


            BarrelTextured.SetActive(true);
            BarrelUnTextured.SetActive(false);
            col.transform.parent.gameObject.SetActive(false);
            Destroy(col.gameObject);
            //col.gameObject.transform.parent.SetParent(CanonUnTextured.transform);
            // scriptCrossMani._manipulationLineRenderer.enabled = (false);
            Debug.Log("Part 1 detected! +1 to ObjectIndex");

            ObjectIndex++;
        }
        else if (ObjectIndex == 0 && col.gameObject != PartOrder[ObjectIndex])
        {
            Debug.Log("This is the wrong part");
            PanelWrongPart.SetActive(true);
        }



        if (ObjectIndex == 1 && col.gameObject == PartOrder[ObjectIndex])
        {
            CartTextured.SetActive(true);
            CartUnTextured.SetActive(false);
            col.transform.parent.gameObject.SetActive(false);
            Destroy(col.gameObject);
            //col.gameObject.transform.parent.SetParent(CanonUnTextured.transform);

            Debug.Log("Part 2 detected! +1 to ObjectIndex");

            ObjectIndex++;
        }
        else if (ObjectIndex == 1 && !col.gameObject != PartOrder[ObjectIndex])
        {
            Debug.Log("This is the wrong part (2)");
            PanelWrongPart.SetActive(true);
        }

        if (ObjectIndex == 2 && col.gameObject == PartOrder[ObjectIndex])
        {
            Wheel1Textured.SetActive(true);
            Wheel1UnTextured.SetActive(false);
            Wheel2Textured.SetActive(true);
            Wheel2UnTextured.SetActive(false);
            Wheel3Textured.SetActive(true);
            Wheel3UnTextured.SetActive(false);
            Wheel4Textured.SetActive(true);
            Wheel4UnTextured.SetActive(false);
            col.transform.parent.gameObject.SetActive(false);
            Destroy(col.gameObject);
            //col.gameObject.transform.parent.SetParent(CanonUnTextured.transform);
            

            Debug.Log("Part 3 detected! +1 to ObjectIndex");

            ObjectIndex++;
        }
        else if (ObjectIndex == 2 && !col.gameObject != PartOrder[ObjectIndex])
        {
            Debug.Log("This is the wrong part (3)");
            PanelWrongPart.SetActive(true);
        }

        if (ObjectIndex == 3)
        {
            Invoke("CompletedAssembly", 2.0f);
        }
    }

    void CompletedAssembly ()
    {
        PanelCompletion.SetActive(true);
        CrossHair.SetActive(false);
        ManipulationButton.SetActive(false);
    }
}
