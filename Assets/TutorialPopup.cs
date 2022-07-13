using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialPopup : MonoBehaviour
{
    public RectTransform Canvas;
    public Transform Camera;
    public TextMeshProUGUI Text;
    public string MoveControlText;
    public string ShootControlText;
    public bool HasMoved = false;
    public bool HasTurned = false;
    public bool HasShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        Text.text = $"CONTROLS\n\n Press {MoveControlText} to move around.\n Hold {ShootControlText} to shoot.";
    }

    // Update is called once per frame
    void Update()
    {
        Canvas.LookAt(Camera);

        if (HasMoved && HasTurned && HasShoot)
        {
            Canvas.gameObject.SetActive(false);
        }
    }
}
