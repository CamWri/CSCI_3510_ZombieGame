using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    Text hudText;

    string hudString;
    float timeSurvived;

    public bool infected = false;

    public bool Infected
    {
        get { return infected; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hudString = hudText.text;
    }

    // Update is called once per frame
    void Update()
    {
        if (infected)
        {
            if (hudString != "You are infected!")
            {
                hudText.text = "You are infected! Time survived: " + timeSurvived.ToString("F2") + "s";
            }
        }
        else 
        {
            timeSurvived += Time.deltaTime;
        }
    }

    public void collisionWithZombie()
    {
        infected = true;
    }
}
