# TaskManager Linux x64 release

TaskManager is a self-contained .NET 10 / Avalonia desktop application. The
packages in this directory target **Linux x64** and were built from the
repository's Release configuration without modifying application code.

## Package files

```
release/
├── linux-x64/
│   ├── AppImage/
│   │   └── TaskManager-<version>-x86_64.AppImage
│   └── deb/
│       └── taskmanager_<version>_amd64.deb
├── TaskManager-x86_64.AppImage
├── taskmanager_amd64.deb
├── SHA256SUMS
├── RELEASE_README.md
└── README.md
```

The two root-level binary names are stable links to the versioned artifacts and
are the names attached to GitHub Releases. `SHA256SUMS` covers those stable
names, so it can be verified directly against the downloaded release assets.

Both packages contain the complete self-contained publish output (Avalonia,
Skia, SQLite, fonts and .NET runtime libraries) and the 256×256 application
icon. Neither package contains a development `tasks.db`; the database is
created on first launch in the user's data directory.

## System requirements

- Linux x86-64 with glibc 2.39 or newer (built and tested on Ubuntu 24.04).
- An X11 display, or a Wayland session with XWayland available.
- The native X11/fontconfig/OpenGL/DBus libraries listed below. The `.deb`
  declares them as dependencies; the AppImage uses whatever is installed on
  the host.

On Debian/Ubuntu, install the AppImage runtime dependencies with:

```sh
sudo apt update
sudo apt install \
  libc6 libgcc-s1 libstdc++6 \
  libfontconfig1 \
  libx11-6 libx11-xcb1 libxext6 libxfixes3 libxi6 libxrandr2 libxcursor1 \
  libgl1 libdbus-1-3
```

GTK and Pango are not required for the current feature set; they are not
loaded when the application starts.

## Installation

### Debian / Ubuntu (.deb)

Install the package with `apt` so dependencies are resolved automatically:

```sh
sudo apt install ./taskmanager_1.0.0_amd64.deb
```

The package installs:

| Path | Purpose |
| --- | --- |
| `/opt/taskmanager/` | Self-contained application files |
| `/usr/bin/taskmanager` | Launcher command |
| `/usr/share/applications/taskmanager.desktop` | Desktop menu entry |
| `/usr/share/icons/hicolor/256x256/apps/taskmanager.png` | Application icon |

`dpkg -i` also works, but `apt install ./…deb` is preferred because it installs
any missing dependencies.

### AppImage

```sh
chmod +x TaskManager-1.0.0-x86_64.AppImage
mkdir -p "$HOME/.local/bin"
cp TaskManager-1.0.0-x86_64.AppImage "$HOME/.local/bin/taskmanager"
```

If the desktop environment does not integrate AppImages automatically, create
a desktop entry that points at the AppImage file. The AppImage already contains
an internal `taskmanager.desktop`, `AppRun`, icon and AppStream metadata.

The AppImage contains an internal desktop entry and icon. AppStream metadata is
not included because the project has no homepage or populated license file yet;
`appimagetool` is therefore run with `--no-appstream`.

AppImage mount mode requires FUSE 2. If mounting fails, run the extracted mode:

```sh
APPIMAGE_EXTRACT_AND_RUN=1 ./TaskManager-1.0.0-x86_64.AppImage
```

## Launching

- From the desktop menu: **TaskManager**.
- Debian/Ubuntu terminal: `taskmanager`.
- AppImage: run the AppImage file directly, for example
  `./TaskManager-1.0.0-x86_64.AppImage`.

## Updating

User data is stored outside both installation layouts, so installing a newer
package does not touch it.

### Debian / Ubuntu

Install the newer `.deb`; APT replaces `/opt/taskmanager` and keeps user data:

```sh
sudo apt install ./taskmanager_1.1.0_amd64.deb
```

### AppImage

Replace the AppImage file with the newer version. If you copied it to
`$HOME/.local/bin/taskmanager`, replace that copy:

```sh
cp TaskManager-1.1.0-x86_64.AppImage "$HOME/.local/bin/taskmanager"
chmod +x "$HOME/.local/bin/taskmanager"
```

Back up `~/.local/share/TaskManager/` before major version upgrades.

## Uninstalling

### Debian / Ubuntu

```sh
sudo apt remove taskmanager
```

To also remove APT's residual metadata, use:

```sh
sudo apt purge taskmanager
```

Neither `remove` nor `purge` deletes `~/.local/share/TaskManager/`. The package
intentionally has no maintainer scripts that touch user data.

### AppImage

Delete the AppImage file and, if created, its desktop entry:

```sh
rm -f "$HOME/.local/bin/taskmanager"
rm -f "$HOME/.local/share/applications/taskmanager.desktop"
```

## User data

The application stores all user-owned files in:

```
~/.local/share/TaskManager/
├── tasks.db
├── attachments/
├── logs/
└── theme.txt
```

`TASKMANAGER_DATA_DIR` is an override used only for isolated release testing;
normal installs do not set it. Uninstalling never deletes this directory.

## Rebuilding the packages

From the repository root:

```sh
./packaging/build-linux-packages.sh
```

The script publishes with
`dotnet publish -c Release -r linux-x64 --self-contained true`, stages the
AppDir and Debian tree, runs `appimagetool` and `fakeroot dpkg-deb --build`,
validates the desktop entry, refuses to package a `tasks.db`, applies the
selected version to package, assembly and embedded manifest metadata, writes
this README, and generates `SHA256SUMS` for the stable release asset names.

Optional environment variables:

| Variable | Purpose |
| --- | --- |
| `VERSION` | Override package and assembly version (for example from a Git tag) |
| `RELEASE_ROOT` | Write packages to a different release directory |
| `WORK_ROOT` | Keep staging files in a specific directory |
| `APPIMAGETOOL` | Use an existing `appimagetool` binary |

If `appimagetool` is not installed, the script downloads the official
`continuous` x86_64 binary over HTTPS. That GitHub release does not publish a
checksum digest.

## Verification

Verified on Ubuntu 24.04.5 x86_64 with .NET SDK 10.0.403 and runtime 10.0.14.

| Check | Result |
| --- | --- |
| `desktop-file-validate` on source and packaged desktop entries | Passed; only a hint that `Office;Utility;` contains two main categories |
| `dpkg-deb --info` and `dpkg-deb --contents` | Passed; metadata and expected paths present |
| No `tasks.db*` in either package | Passed |
| `SHA256SUMS` generation and verification | Passed; `sha256sum -c SHA256SUMS` succeeds for the stable release asset names |
| AppImage extraction/inspection | Passed; `AppRun`, root desktop file, root icon, `.DirIcon`, hicolor icon and full self-contained runtime present |
| AppImage direct FUSE launch | Passed; stayed alive for 12 seconds and created `tasks.db`, `attachments/`, `logs/` |
| AppImage `APPIMAGE_EXTRACT_AND_RUN=1` launch | Passed; stayed alive for 12 seconds and created a fresh data directory |
| AppImage update to simulated 1.0.1 | Passed; sentinel data hash unchanged and the same database reopened |
| APT dependency simulation for the `.deb` | Passed on the build host; all declared dependencies resolved |
| `.deb` install, launch, upgrade, remove and purge in isolated dpkg/Bubblewrap sandbox | Passed; installed wrapper launched, upgrade preserved sentinel data, remove/purge deleted package files only |
| User data after `.deb` remove/purge | Preserved |

The build script writes checksums for the two release binaries to
`release/SHA256SUMS`. Verify downloaded assets with:

```sh
sha256sum -c SHA256SUMS
```

### Known limitations

- The application uses Avalonia's X11 backend; on Wayland it runs through XWayland.
- AppImage mount mode requires FUSE 2. The extract-and-run fallback was tested successfully.
- AppStream metadata is omitted because `License` is empty and no project homepage is configured; `appimagetool` is invoked with `--no-appstream`.
- `lintian` is not installed on the build host, so the `.deb` was not linted with it.
- The `.deb` targets Ubuntu 24.04/amd64 and glibc 2.39. Older distributions may not resolve the declared dependency versions.
- EF Core logs non-fatal warnings about `PRAGMA foreign_keys = 0;` inside transactions; migrations still apply successfully.
- The simulated 1.0.1 packages changed package metadata only. Application source was not modified, so startup logs still report assembly version 1.0.0.
- A development `tasks.db` remains in the repository root; it is not included in either package. Release tests used a neutral working directory to avoid the application's existing legacy-database import path.
