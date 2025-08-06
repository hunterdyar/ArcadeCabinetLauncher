using System.Diagnostics;
using System.Security.Cryptography;

namespace Button;

public class TargetManager
{
	public List<Target> Targets => _targets;
	private List<Target> _targets = new List<Target>();
	public DirectoryInfo Directory;
	public int SelectedIndex => _currentSelected;
	private int _currentSelected;
	public static Action<TargetManager> OnTargetListUpdated;
	public static Action<int, Target> OnCurrentTargetChanged;
	private Process? _launchProcess;
	public Target? CurrentlyPlaying;
	public void InitializeFromDirectory(string directoryPath)
	{
		_targets.Clear();
		var d = Path.GetFullPath(directoryPath);
		Directory = new DirectoryInfo(d);
		if (!Directory.Exists)
		{
			throw new Exception($"Invalid Directory: {directoryPath}");
		}
		foreach (var executable in Directory.EnumerateFiles("*.exe", SearchOption.AllDirectories))
		{
			_targets.Add(new Target()
			{
				ExectuableFile = executable,
				Title = executable.Name
			});
		}

		if (_targets.Count == 0)
		{
			Console.WriteLine("Found no exe files in {");
		}

		_targets = _targets.OrderByDescending(x => x.Title).ToList();

		OnTargetListUpdated?.Invoke(this);
	}

	public void Cycle(int delta)
	{
		_currentSelected += delta;
		if (_currentSelected >= _targets.Count)
		{
			_currentSelected = _currentSelected % _targets.Count;
		}else if (_currentSelected < 0)
		{
			_currentSelected = (_currentSelected + _targets.Count) % _targets.Count;
		}

		OnCurrentTargetChanged?.Invoke(_currentSelected, _targets[_currentSelected]);
	}

	public void Launch()
	{
		if (_launchProcess != null && !_launchProcess.HasExited)
		{
			Console.WriteLine("Can't launch process, already running one!");
			return;
		}
		
		var target = _targets[_currentSelected];
		CurrentlyPlaying = target;
		_launchProcess = Process.Start(target.ExectuableFile.FullName);
		_launchProcess.EnableRaisingEvents = true;
		_launchProcess.Exited += OnProcessExit;
	}

	public void OnProcessExit(object? sender, EventArgs eventArgs)
	{
		_launchProcess = null;
		CurrentlyPlaying = null;
	}
}

public class Target
{
	public FileInfo ExectuableFile;
	public string Title;
}