using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    GameObject controller;
    PawnManipulator scriptPlaceObject;
    public GameObject Part1;
    public GameObject Part2;
    public GameObject Part3;
    //public GameObject sphereIndicator;
    //public GameObject cubeIndicator;
    //public GameObject canonIndicator;
    public void ChangeObjectTypeToPart1()
    {
        controller = GameObject.Find("PawnGenerator");
        scriptPlaceObject = controller.GetComponent<PawnManipulator> ();
        
        //cubeIndicator.SetActive(true);
        //canonIndicator.SetActive(false);
        //sphereIndicator.SetActive(false);
        scriptPlaceObject.PawnPrefab = Part1;
    }
    public void ChangeObjectTypeToPart2()
    {
        controller = GameObject.Find("PawnGenerator");
        scriptPlaceObject = controller.GetComponent<PawnManipulator> ();
        
        //cubeIndicator.SetActive(false);
        //canonIndicator.SetActive(false);
        //sphereIndicator.SetActive(true);
        scriptPlaceObject.PawnPrefab = Part2;
    }
    public void ChangeObjectTypeToPart3()
    {
        controller = GameObject.Find("PawnGenerator");
        scriptPlaceObject = controller.GetComponent<PawnManipulator>();
        
        //cubeIndicator.SetActive(false);
        //canonIndicator.SetActive(true);
        //sphereIndicator.SetActive(false);
        scriptPlaceObject.PawnPrefab = Part3;
    }
}
