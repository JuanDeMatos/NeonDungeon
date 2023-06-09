using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerDisplayChange : MonoBehaviour
{
    [SerializeField] private Sprite keyboard;
    [SerializeField] private Sprite gamepad;
    [SerializeField] private SpriteRenderer image;

    // Start is called before the first frame update
    void Start()
    {
        InputUser.onChange += InputUser_onChange;
    }

    private void OnDestroy()
    {
        InputUser.onChange -= InputUser_onChange;
    }

    private void InputUser_onChange(InputUser arg1, InputUserChange arg2, InputDevice arg3)
    {
        if (arg1.controlScheme.ToString().Contains("Keyboard"))
        {
            image.sprite = keyboard;
        } else
        {
            image.sprite = gamepad;
        }
    }
}
