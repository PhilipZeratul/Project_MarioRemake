using UnityEngine;


public class ControllerManager : MonoBehaviour
{
    public float minJumpTime = 0.5f;
    // Perhaps, respond differently according to game status.
    //~TODO: Consider input response carefully.
    private float horizontalInput;
    private bool isLeftPressed;
    private bool isRightPressed;
    private bool isJumpPressed;
    private bool isJumpHolding;
    private bool isJumpReleased = true;


    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        isLeftPressed = Input.GetButton("Left");
        isRightPressed = Input.GetButton("Right");

        isJumpHolding = Input.GetButton("Jump");
        if (!isJumpPressed)
            isJumpPressed = Input.GetButtonDown("Jump");
        isJumpReleased = Input.GetButtonUp("Jump");

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

    public void JumpPressedConsumed()
    {
        isJumpPressed = false;
    }

    public bool JumpHolding()
    {
        return isJumpHolding;
    }

    public bool IsLeftPressed()
    {
        return isLeftPressed;
    }

    public bool IsRightPressed()
    {
        return isRightPressed;
    }
}
