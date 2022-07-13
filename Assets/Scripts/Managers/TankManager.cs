using System;
using UnityEngine;

[Serializable]
public class TankManager // Manage various settings of a tank
{
    /*
    Works with the GameManager class to control how the tanks behave and
    whether or not players have control of their tank in the different phases of the game
    */

    public Color PlayerColor;            
    public Transform SpawnPoint;
    public Transform Camera;
    public string MoveControlText;
    public string ShootControlText;
    [HideInInspector] public int PlayerNumber;             
    [HideInInspector] public string ColoredPlayerText;
    [HideInInspector] public GameObject Instance; // Reference to the instance of the tank when it is created         
    [HideInInspector] public int Wins;                     


    private TankMovement Movement; // Reference to tank's movement script, used to disable and enable control      
    private TankShooting Shooting; // Reference to tank's shooting script, used to disable and enable control
    private GameObject CanvasGameObject; // Used to disable the world space UI during the starting and ending phases of each round
    private TutorialPopup Tutorial;

    public void Setup()
    {
        // Get references to the components
        Movement = Instance.GetComponent<TankMovement>();
        Shooting = Instance.GetComponent<TankShooting>();
        CanvasGameObject = Instance.GetComponentInChildren<Canvas>().gameObject;
        Tutorial = Instance.GetComponent<TutorialPopup>();

        // Set the player numbers to be consistent across the scripts
        Movement.PlayerNumber = PlayerNumber;
        Shooting.PlayerNumber = PlayerNumber;

        // Create a string using the correct color based on the tank's color and the player's number
        ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(PlayerColor) + ">PLAYER " + PlayerNumber + "</color>";

        // Get all of the renderers of the tank
        MeshRenderer[] renderers = Instance.GetComponentsInChildren<MeshRenderer>();
        
        // Go through all the renderers
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = PlayerColor; // Set the material color to match the current player color
        }

        Tutorial.Camera = Camera;
        Tutorial.MoveControlText = MoveControlText;
        Tutorial.ShootControlText = ShootControlText;

    }


    // Used during the phases of the game where the player shouldn't be able to control their tank
    public void DisableControl()
    {
        Movement.enabled = false;
        Shooting.enabled = false;

        CanvasGameObject.SetActive(false);
    }


    // Used during the phases of the game where the player should be able to control their tank
    public void EnableControl()
    {
        Movement.enabled = true;
        Shooting.enabled = true;

        CanvasGameObject.SetActive(true);
    }


    // Used at the start of each round to put the tank into it's default state
    public void Reset()
    {
        Instance.transform.position = SpawnPoint.position;
        Instance.transform.rotation = SpawnPoint.rotation;

        Instance.SetActive(false);
        Instance.SetActive(true);
    }
}
