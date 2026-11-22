# Camera Media Capacity

Estimate capture capacity across clips with different bitrates and an explicit storage reserve.

![Sample run from Camera Media Capacity](docs/demo.webp)

## Run

Install .NET 8 SDK and runtime. Run these commands from the repository root.

```sh
dotnet build --nologo
dotnet bin/Debug/net8.0/App.dll sample.json
```

Replace `sample.json` with a JSON file following the same structure. The app prints a JSON report to standard output.

Use decimal gigabytes, megabits per second and clip durations in seconds. The estimate excludes container overhead beyond the reserve.

## Verify

Run `python check.py` with Python 3.11 or newer after the build step when applicable. The check verifies the bundled scenario and confirms that an invalid input returns an error.
