using Raylib_cs;

namespace Button;

public class InputManager
{
	private bool _justToggledAxis = false;
	public void Tick()
	{
		var key = Raylib.GetKeyPressed();
		while (key != 0)
		{
			var k = (KeyboardKey)(key);
			if (k == KeyboardKey.Right || k == KeyboardKey.Up || k == KeyboardKey.D || k == KeyboardKey.W)
			{
				CycleInput(1);
			}

			if (k == KeyboardKey.Left || k == KeyboardKey.Down || k == KeyboardKey.A || k == KeyboardKey.S)
			{
				CycleInput(-1);
			}

			if (k == KeyboardKey.Enter || k == KeyboardKey.Space)
			{
				SelectInput();
			}

			key = Raylib.GetKeyPressed();
		}

		if (Raylib.IsGamepadAvailable(0))
		{
			CheckGamepadInput(0);
		}

		if (Raylib.IsGamepadAvailable(1))
		{
			CheckGamepadInput(1);
		}
	}

	private void CheckGamepadInput(int gamepad)
	{
		var right = Raylib.IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceRight) ||
		            Raylib.IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceUp);
		if (right)
		{
			CycleInput(1);
		}

		var left = Raylib.IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceLeft) ||
		           Raylib.IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceDown);
		if (left)
		{
			CycleInput(-1);
		}

		var launch = Raylib.IsGamepadButtonDown(gamepad, GamepadButton.RightTrigger2)
		             || Raylib.IsGamepadButtonDown(gamepad, GamepadButton.RightTrigger2)
			|| Raylib.IsGamepadButtonDown(gamepad, GamepadButton.RightFaceDown);
		if (launch)
		{
			SelectInput();
		}


		var axis = Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.LeftX);
		if (!_justToggledAxis)
		{
			if (axis >= 0.95f)
			{
				_justToggledAxis = true;
				CycleInput(1);
			}else if (axis <= -0.95f)
			{
				_justToggledAxis = true;
				CycleInput(-1);
			}
		}
		else
		{
			if (MathF.Abs(axis) < 0.9f)
			{
				_justToggledAxis = false;
			}
		}
	}

	public void CycleInput(int delta)
	{
		Program.TargetManager.Cycle(delta);
	}

	public void SelectInput()
	{
		Program.TargetManager.Launch();
	}
}