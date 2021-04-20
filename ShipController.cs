using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;

public class ShipController : ShipMotor
{
    public float noTurn = 0.05f;

    public void Update()
    {
        //  Turning

        Vector2 delta = (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0.0f)) / Screen.height;
        float vertical = 0.0f;
        float horizontal = 0.0f;
        if (delta.y > noTurn)
            vertical = -(delta.y - noTurn);
        if (delta.y < -noTurn)
            vertical = -(delta.y + noTurn);
        if (delta.x > noTurn)
            horizontal = (delta.x + noTurn);
        if (delta.x < -noTurn)
            horizontal = (delta.x - noTurn);

        if (vertical != 0.0f)
        {
            EngagePitch(vertical);
        }
        else
        {
            DisengagePitch();
        }

        if (horizontal != 0.0f)
        {
            EngageYaw(horizontal);
        }
        else
        {
            DisengageYaw();
        }

        if (Input.GetKeyDown(KeyCode.E) == true)
		{
            EngageRoll(-1.0f);
        }
        else if (Input.GetKeyUp(KeyCode.E) == true)
		{
            DisengageRoll();
        }

        if (Input.GetKeyDown(KeyCode.Q) == true)
		{
            EngageRoll(1.0f);
        }
        else if (Input.GetKeyUp(KeyCode.Q) == true)
		{
            DisengageRoll();
        }


        //  Movement

        if (Input.GetKeyDown(KeyCode.W) == true)
		{
            EngageThrust(Direction.NORTH);
        }
        else if (Input.GetKeyUp(KeyCode.W) == true)
		{
            DisengageThrust(Direction.NORTH);
        }

        if (Input.GetKeyDown(KeyCode.A) == true)
		{
            EngageThrust(Direction.WEST);
        }
        else if (Input.GetKeyUp(KeyCode.A) == true)
		{
            DisengageThrust(Direction.WEST);
        }

        if (Input.GetKeyDown(KeyCode.S) == true)
		{
            EngageThrust(Direction.SOUTH);
        }
        else if (Input.GetKeyUp(KeyCode.S) == true)
		{
            DisengageThrust(Direction.SOUTH);
        }

        if (Input.GetKeyDown(KeyCode.D) == true)
		{
            EngageThrust(Direction.EAST);
        }
        else if (Input.GetKeyUp(KeyCode.D) == true)
		{
            DisengageThrust(Direction.EAST);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) == true)
		{
            EngageThrust(Direction.BELOW);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) == true)
		{
            DisengageThrust(Direction.BELOW);
        }

        if (Input.GetKeyDown(KeyCode.Space) == true)
		{
            EngageThrust(Direction.ABOVE);
        }
        else if (Input.GetKeyUp(KeyCode.Space) == true)
		{
            DisengageThrust(Direction.ABOVE);
        }
    }
}
