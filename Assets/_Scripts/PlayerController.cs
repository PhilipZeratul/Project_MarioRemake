using UnityEngine;


public class PlayerController : PhysicsObject
{
    private float horizontalInput;
    private bool isJumpInput = false;


    protected override float CalcHorizontalVelocity()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        return horizontalInput * horizontalSpeed;
    }

    protected override float CalcVerticalVelocity()
    {
        isJumpInput = Input.GetButton("Jump");
        if (isGrounded && isJumpInput)
        {
            isGrounded = false;
            return jumpSpeed;
        }
        else
            return 0f;
    }
}
