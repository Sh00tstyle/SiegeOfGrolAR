using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    GameObject controller;
    PawnManipulator scriptPlaceObject;
    GameObject Part1Go;
    GameObject Part2Go;
    GameObject Part3Go;
    //public GameObject sphereIndicator;
    //public GameObject cubeIndicator;
    //public GameObject canonIndicator;
    public void ChangeObjectTypeToPart1()
    {
        controller = GameObject.Find("PawnGenerator");
        scriptPlaceObject = controller.GetComponent<PawnManipulator> ();
        Part1Go = GameObject.Find("Part1");
        //cubeIndicator.SetActive(true);
        //canonIndicator.SetActive(false);
        //sphereIndicator.SetActive(false);
        scriptPlaceObject.PawnPrefab = Part1Go;
    }
    public void ChangeObjectTypeToPart2()
    {
        controller = GameObject.Find("PawnGenerator");
        scriptPlaceObject = controller.GetComponent<PawnManipulator> ();
        Part2Go = GameObject.Find("Part2");
        //cubeIndicator.SetActive(false);
        //canonIndicator.SetActive(false);
        //sphereIndicator.SetActive(true);
        scriptPlaceObject.PawnPrefab = Part2Go;
    }
    public void ChangeObjectTypeToPart3()
    {
        controller = GameObject.Find("PawnGenerator");
        scriptPlaceObject = controller.GetComponent<PawnManipulator>();
        Part3Go = GameObject.Find("Part3");
        //cubeIndicator.SetActive(false);
        //canonIndicator.SetActive(true);
        //sphereIndicator.SetActive(false);
        scriptPlaceObject.PawnPrefab = Part3Go;
    }
}
