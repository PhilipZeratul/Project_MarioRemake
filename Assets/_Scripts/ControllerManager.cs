using UnityEngine;


public class ControllerManager : MonoBehaviour
{
    public float minJumpTime = 0.5f;
    // Perhaps, respond differently according to game status.
    //~TODO: Consider input response carefully.
    private float horizontalInput;
    private bool isJumpPressed;

    private float jumpButtonDownTimeStamp;
    private float jumpButtonUpTimeStamp;
    private float jumpTimeInterval;


    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        isJumpPressed = Input.GetButton("Jump");

        if (Input.GetButtonDown("Jump"))
            jumpButtonDownTimeStamp = Time.time;

        if (Input.GetButtonUp("Jump"))
        {
            jumpButtonUpTimeStamp = Time.time;
            jumpTimeInterval = jumpButtonUpTimeStamp - jumpButtonDownTimeStamp;
        }

        if (jumpTimeInterval > 0 && jumpTimeInterval < minJumpTime)
        {
            isJumpPressed = true;
            jumpTimeInterval = Time.time - jumpButtonDownTimeStamp;
        }
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
