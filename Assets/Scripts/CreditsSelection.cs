using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSelection : MonoBehaviour
{
    public TextMeshProUGUI bioBox;

    /*
    public enum Developer
    {
        Damian,
        Justin,
        Andrew,
        Dante
    }

    public Developer developer = Developer.Damian;
    */

    Dictionary<string, string> developerCredits = new Dictionary<string, string>
    {
        { "Damian", "Damian: Developer" },
        { "Justin", "Justin: Developer" },
        { "Andrew", "Andrew: Developer" },
        { "Dante", "Dante: Developer" }
    };

    void Start()
    {
        bioBox.text = developerCredits["Damian"];
    }

    public void OnSelected(string dev)
    {
        switch (dev)
        {
            case "Damian":
                bioBox.text = developerCredits["Damian"];
                break;
            case "Justin":
                bioBox.text = developerCredits["Justin"];
                break;
            case "Andrew":
                bioBox.text = developerCredits["Andrew"];
                break;
            case "Dante":
                bioBox.text = developerCredits["Dante"];
                break;
        }
    }
}
