using System.Diagnostics;
using System.Security.Cryptography;
using Raylib_cs;

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

	private FileSystemWatcher _watcher;
	public void InitializeFromDirectory(string directoryPath)
	{
		var d = Path.GetFullPath(directoryPath);
		Directory = new DirectoryInfo(d);
		if (!Directory.Exists)
		{
			throw new Exception($"Invalid Directory: {directoryPath}");
		}

		_watcher = new FileSystemWatcher(Directory.FullName);
		_watcher.EnableRaisingEvents = true;
		_watcher.IncludeSubdirectories = true;
		_watcher.NotifyFilter = NotifyFilters.Attributes
		                                               | NotifyFilters.CreationTime
		                                               | NotifyFilters.DirectoryName
		                                               | NotifyFilters.FileName
		                                               | NotifyFilters.LastAccess
		                                               | NotifyFilters.LastWrite
		                                               | NotifyFilters.Size;
		_watcher.Filter = "*.exe";
		_watcher.Created += WatcherOnCreated;
		_watcher.Deleted += WatcherOnDeleted;
		_watcher.Renamed += WatcherOnRenamed;
		_watcher.Changed += WatcherOnChange;
		_watcher.Error += (sender, args) =>
		{
			Console.WriteLine($"File Watcher error: {args.GetException().Message}");
		};
		
		//do we need to call this?
		_watcher.BeginInit();
		RecreateTargets();
	}

	private void RecreateTargets()
	{
		_targets.Clear();
		foreach (var executable in Directory.EnumerateFiles("*.exe", SearchOption.AllDirectories))
		{
			if (executable.FullName == Environment.ProcessPath)
			{
				continue;
			}
			_targets.Add(new Target()
			{
				ExectuableFile = executable,
				Title = executable.Name,
			});
		}

		if (_targets.Count == 0)
		{
			Console.WriteLine("Found no exe files in {");
		}

		_targets = _targets.OrderByDescending(x => x.ExectuableFile.LastWriteTime).ToList();
		_currentSelected = _targets.Count - 1;
		
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
		Raylib.SetWindowFocused();
	}


	private void WatcherOnRenamed(object sender, RenamedEventArgs e)
	{
		RecreateTargets();
	}

	private void WatcherOnDeleted(object sender, FileSystemEventArgs e)
	{
		RecreateTargets();
	}

	private void WatcherOnCreated(object sender, FileSystemEventArgs e)
	{
		RecreateTargets();
	}

	private void WatcherOnChange(object sender, FileSystemEventArgs e)
	{
		RecreateTargets();
	}
}

public class Target
{
	public FileInfo ExectuableFile;
	public string Title;
}