using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPartScript : MonoBehaviour
{
    public GameObject Part1;
    public GameObject Part2;
    public GameObject Part3;

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

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.name == "Part1")
        {
            BarrelTextured.SetActive(true);
            BarrelUnTextured.SetActive(false);
            Destroy(col.gameObject);

            Debug.Log("Part 1 detected!");
        }

        if (col.gameObject.name == "Part2")
        {
            CartTextured.SetActive(true);
            CartUnTextured.SetActive(false);
            Destroy(col.gameObject);

            Debug.Log("Part 2 detected!");
        }

        if (col.gameObject.name == "Part3")
        {
            Wheel1Textured.SetActive(true);
            Wheel1UnTextured.SetActive(false);
            Wheel2Textured.SetActive(true);
            Wheel2UnTextured.SetActive(false);
            Wheel3Textured.SetActive(true);
            Wheel3UnTextured.SetActive(false);
            Wheel4Textured.SetActive(true);
            Wheel4UnTextured.SetActive(false);
            Destroy(col.gameObject);

            Debug.Log("Part 3 detected!");
        }
    }
}
