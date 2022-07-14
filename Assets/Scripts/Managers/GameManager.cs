using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int NumRoundsToWin = 3;        
    public float StartDelay = 3f; // The delay between the start of RoundStarting and RoundPlaying        
    public float EndDelay = 3f; // The delay between the start of RoundPlaying and RoundEnding phases          
    public CameraControl CameraControl; // Reference to the CameraControl script for control during different phases
    public TextMeshProUGUI MessageText; // Reference to the overlay Text to display winning text
    public GameObject TankPrefab; // Reference to the prefab the players will control        
    public TankManager[] Tanks; // Collection of managers for enabling and disabling different aspects of the tanks


    private int RoundNumber;              
    private WaitForSeconds StartWait; // Used to have a delay whilst the round starts    
    private WaitForSeconds EndWait; // Used to have a delay whilst the round ends       

    private TankManager RoundWinner;
    private TankManager GameWinner;

    public RectTransform EndScreen;

    // This line fixes a change to the physics engine
    private void Start()
    {
        // Create the delays so they only have to be made once        
        StartWait = new WaitForSeconds(StartDelay);
        EndWait = new WaitForSeconds(EndDelay);

        EndScreen.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        SpawnAllTanks();
        SetCameraTargets();

        // Once tanks have been created and camera is using them as targets -> Start the game
        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks()
    {
        // Check through all of the tanks
        for (int i = 0; i < Tanks.Length; i++)
        {
            // Create the tanks and set the player number -> Pass on to the Setup function in TankManager script
            Tanks[i].Instance =  Instantiate(TankPrefab, Tanks[i].SpawnPoint.position, Tanks[i].SpawnPoint.rotation) as GameObject;
            Tanks[i].PlayerNumber = i + 1;
            Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        // Create a collection of transforms with the same size as the number of tanks
        Transform[] targets = new Transform[Tanks.Length];

        // Check through all of the transforms
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = Tanks[i].Instance.transform; // Set it up to the appropriate tank transform
        }

        CameraControl.Targets = targets;
    }


    // Is called from the start and runs each phase of the game one after another
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (GameWinner != null)
        {
            MessageText.text = string.Empty;

            yield return null;
            GameOver();
        }
        else
        {
            StartCoroutine(GameLoop()); // If there is no winner yet -> Restart the coroutine
        }
    }


    private IEnumerator RoundStarting()
    {
        // Reset and disable the player controls when round is just starting
        ResetAllTanks();
        DisableTankControl();

        // Set the camera's position and size
        CameraControl.SetStartPositionAndSize();

        // Increment the round number and display message showing the round number
        RoundNumber++;
        MessageText.text = "ROUND " + RoundNumber;

        // Waits for specificed time length until yielding control back to the game loop
        yield return StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // Enable the player's control once round plays
        EnableTankControl();

        // Clear the text from the screen
        MessageText.text = string.Empty;

        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // Disable player control when round ends
        DisableTankControl();

        // Clear the winner from the previous round
        RoundWinner = null;

        // Check if there is a round winner in current round
        RoundWinner = GetRoundWinner();

        // If there is a winner -> Increment the win score
        if(RoundWinner != null)
        {
            RoundWinner.Wins++;
        }

        // Check if ther is a game winner in current round
        GameWinner = GetGameWinner();

        // Display message based on score and whether or not there is a game winner
        string message = EndMessage();
        MessageText.text = message;

        // Waits for specificed time length until yielding control back to the game loop
        yield return EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < Tanks.Length; i++)
        {
            if (Tanks[i].Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            if (Tanks[i].Instance.activeSelf)
                return Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            if (Tanks[i].Wins == NumRoundsToWin)
                return Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (RoundWinner != null)
            message = RoundWinner.ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < Tanks.Length; i++)
        {
            message += Tanks[i].ColoredPlayerText + ": " + Tanks[i].Wins + " WINS\n";
        }

        if (GameWinner != null)
            message = GameWinner.ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    public void GameOver()
    {
        EndScreen.gameObject.SetActive(true);
    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel); // Restarts the level if there is a game winner
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].DisableControl();
        }
    }
}