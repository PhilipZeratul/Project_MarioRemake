using UnityEngine;


public class ControllerManager : MonoBehaviour
{
    // Perhaps, respond differently according to game status.
    //~TODO: Consider input response carefully.
    private float horizontalInput;
    private bool isJumpPressed;


    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        isJumpPressed = Input.GetButton("Jump");
    }

    public float HorizontalMove()
    {
        //Debug.LogFormat("Input HorizontalMove = {0}", horizontalInput);
        return horizontalInput;
    }

    public bool JumpPressed()
    {
        //Debug.LogFormat("Input JumpPressed = {0}", isJumpPressed);
        return isJumpPressed;
    }
}
