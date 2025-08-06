using System.Numerics;
using Raylib_cs;

namespace Button;

public class LauncherWindow
{
	public void SetSize(int width, int height)
	{
		
	}

	public void Draw()
	{
		var current = Program.TargetManager.CurrentlyPlaying;
		if (current == null)
		{
			DrawOptions();
		}
		else
		{
			DrawCurrent(current);
		}
	}

	private void DrawCurrent(Target current)
	{
		Raylib.DrawText("Currently Playing: "+current.Title, 50, 50, 24, Color.White);
		Raylib.DrawText(current.ExectuableFile.ToString(), 50, 90, 12, Color.Gray);

	}

	private void DrawOptions()
	{
		int y = 50;
		int x = 50;
		//reversed so that "up and right" decrease in number.
		for (var i = Program.TargetManager.Targets.Count - 1; i >= 0; i--)
		{
			var selected = Program.TargetManager.SelectedIndex == i;
			Color c = selected ? Color.Gold : Color.White;
			var pre = selected ? "> " : "  ";
			var target = Program.TargetManager.Targets[i];
			Raylib.DrawTextEx(Program.Font, pre + target.Title, new Vector2(x,y), 64, 20, c);
			y += 70;
		}
	}
}