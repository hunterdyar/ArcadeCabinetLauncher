# Arcade Cabinet Games Launcher

An exe to open on start.

By default is searches it's own file (exe) location for all exe and lnk (shortcuts) and makes a list of them.

It can take an optional single argument on launch (use a shortcut), which is a relative or absolute directory name.

It will watch the directory and attempt to add/remove files without needing to be restarted.

---
- Uses [LnkParser](https://github.com/louietan/LnkParser/) for reading the shortcut files
- Uses [Raylib](https://www.raylib.com/) via [raylib-cs](https://github.com/raylib-cs/raylib-cs) for input, window, graphics, etc.
