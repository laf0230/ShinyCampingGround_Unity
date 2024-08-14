using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 inputvalue;
    public Vector2 rotation;
    
    public void OnMove(InputValue input) => inputvalue = input.Get<Vector2>();
    public void OnRotate(InputValue input) => rotation = input.Get<Vector2>();
}
