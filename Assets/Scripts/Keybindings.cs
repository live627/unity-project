using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Keybindings : MonoBehaviour
{
    private Transform menuPanel;
    private Text buttonText;
    private Event keyEvent;
    private KeyCode newKey;
    private bool waitingForKey = false;
    private GameManager gameManager;
    Button[] children;
    
    void Start()
    {
        gameManager = new GameManager();

        /*
         * iterate through each child of the panel and check
         * the names of each one. Each if statement will
         * set each button's text component to display
         * the name of the key that is associated
         * with each command. Example: the ForwardKey
         * button will display "W" in the middle of it
         */
        children = gameObject.GetComponentsInChildren<Button>();
        foreach (Button child in children)
        {
            Text childtext = child.GetComponentInChildren<Text>();
            child.onClick.AddListener(() => { StartAssignment(child.name); });
            switch (child.name)
            {
                case "up":
                    newKey = gameManager.Up;
                    break;
                case "down":
                    newKey = gameManager.Down;
                    break;
                case "left":
                    newKey = gameManager.Left;
                    break;
                case "right":
                    newKey = gameManager.Right;
                    break;
                case "pause":
                    newKey = gameManager.Pause;
                    break;
                default:
                    break;
            }
            childtext.text = AddSpaces(newKey.ToString());
        }
    }

    void OnGUI()
    {
        /*
         * keyEvent dictates what key our user presses
         * by using Event.current to detect the current
         * event
         */
        keyEvent = Event.current;

        //Executes if a button gets pressed and
        //the user presses a key
        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode; //Assigns newKey to the key user presses
            waitingForKey = false;
        }
    }

    /**
     * Buttons cannot call on Coroutines via OnClick().
     * Instead, we have it call StartAssignment, which will
     * call a coroutine in this script instead, only if we
     * are not already waiting for a key to be pressed.
     */
    public void StartAssignment(string keyName)
    {
        if (!waitingForKey)
        {
            StartCoroutine(AssignKey(keyName));
        }

        // Disable all other buttons as a visal aid to the user.
        foreach (Button child in children)
        {
            if (child.name == keyName)
            {
                buttonText = child.GetComponentInChildren<Text>();
                buttonText.text = string.Format("<{0}>", buttonText.text);
            }
            else
            {
                child.interactable = false;
            }
        }
    } 

    //Used for controlling the flow of our below Coroutine
    IEnumerator WaitForKey()
    {
        while (!keyEvent.isKey)
        {
            yield return null;
        }
    }

    /**
     * AssignKey takes a keyName as a parameter. The
     * keyName is checked in a switch statement. Each
     * case assigns the command that keyName represents
     * to the new key that the user presses, which is grabbed
     * in the OnGUI() function, above.
     */
    public IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;

        yield return WaitForKey(); //Executes endlessly until user presses a key

        switch (keyName)
        {
            case "up":
                gameManager.Up = newKey; //Set forward to new keycode
                break;
            case "down":
                gameManager.Down = newKey; //set backward to new keycode
                break;
            case "left":
                gameManager.Left = newKey; //set left to new keycode
                break;
            case "right": 
                gameManager.Right = newKey; //set right to new keycode
                break;
            case "pause":
                gameManager.Pause = newKey; //set jump to new keycode
                break;
            default:
                break;
        }
        buttonText.text = AddSpaces(newKey.ToString());

        // Enable all the buttons.
        foreach (Button child in children)
        {
            child.interactable = true;
        }

        // Now save the new binding!
        gameManager.SaveSettings();

        yield return null;
    }

    // Add spaces before Capital Letters
    private string AddSpaces(string val)
    {
        return string.Concat(val.Select(x => Char.IsUpper(x) ? " " + x : x.ToString()).ToArray()).TrimStart(' ');
    }
}
