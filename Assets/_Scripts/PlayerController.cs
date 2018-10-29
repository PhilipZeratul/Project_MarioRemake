using UnityEngine;


public class PlayerController : PhysicsObject
{
    protected override float CalcHorizontalVelocity()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        return horizontalInput * horizontalSpeed;
    }

    protected override float CalcVerticalVelocity()
    {
        bool isJumpInput = Input.GetButtonDown("Jump");


        Debug.LogFormat("isGrounded = {0}, isJumpInput = {1}", isGrounded, isJumpInput);
        if (isGrounded && isJumpInput)
        {
            Debug.Log("Jump!");
            isGrounded = false;
            return jumpSpeed;
        }
        else
            return 0f;
    }
}
