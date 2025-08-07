using System.Runtime.CompilerServices;
using Raylib_cs;

namespace Button
{
	class Program
	{
		public static LauncherWindow LauncherWindow = new LauncherWindow();
		public static InputManager InputManager = new InputManager();
		public static TargetManager TargetManager = new TargetManager();
		
		public static Font Font;

		public static void Main(string[] args)
		{
			//Load Resource
			if (args.Length == 0)
			{
				TargetManager.InitializeFromDirectory(Environment.CurrentDirectory);		
			}
			else
			{
				var tryRelative = Path.Join(Environment.CurrentDirectory, args[0]);
				var di = new DirectoryInfo(tryRelative);
				if (di.Exists)
				{
					TargetManager.InitializeFromDirectory(di.FullName);
				}else{
					TargetManager.InitializeFromDirectory(args[0]);
				}
			}
			
			//Prepare Program
			InitWindow();
			Raylib.SetWindowSize(Raylib.GetMonitorWidth(0), Raylib.GetMonitorHeight(0));
			Raylib.SetWindowFocused();
			Raylib.ToggleBorderlessWindowed();
			Raylib.MaximizeWindow();
			var top = Raylib.IsWindowState(ConfigFlags.TopmostWindow);
			if (top)
			{
				Raylib.ClearWindowState(ConfigFlags.TopmostWindow);
				Console.WriteLine("should not be always on top!");
			}
			 

			
			//run the loop
			while (!Raylib.WindowShouldClose())
			{
				//first do inputs and controls.
				Update();
				
				if (Raylib.IsWindowResized())
				{
					LauncherWindow.SetSize(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
				}

				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.Black);

				LauncherWindow.Draw();
				Raylib.EndDrawing();
			}
		}

		private static void InitWindow()
		{
			Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
			Raylib.InitWindow(1920, 1080, "launcher");
			Font = Raylib.LoadFont("Resources/CascadiaMono.ttf");
			LauncherWindow.SetSize(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
		}

		private static void Update()
		{
			InputManager.Tick();
		}

		private void Exit()
		{
			Raylib.CloseWindow();
		}

	}
}