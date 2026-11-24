#!/usr/bin/env bash
set -euo pipefail

# Build reproducible Linux x64 packages for TaskManager.
# No application source files are modified; everything is staged outside the repo.

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(CDPATH= cd -- "$SCRIPT_DIR/.." && pwd)"
PROJECT="$REPO_ROOT/TaskManager.UI/TaskManager.UI.csproj"
DESKTOP_SOURCE="$SCRIPT_DIR/taskmanager.desktop"
ICON_SOURCE="$REPO_ROOT/TaskManager.UI/Assets/taskmanager.png"
MANIFEST_SOURCE="$REPO_ROOT/TaskManager.UI/app.manifest"
RELEASE_README_SOURCE="$SCRIPT_DIR/RELEASE_README.md"

die() {
    printf 'error: %s\n' "$*" >&2
    exit 1
}

require() {
    command -v "$1" >/dev/null 2>&1 || die "required tool not found: $1"
}

for tool in dotnet dpkg-deb fakeroot desktop-file-validate curl file sha256sum; do
    require "$tool"
done

[[ -f "$PROJECT" ]] || die "project not found: $PROJECT"
[[ -f "$DESKTOP_SOURCE" ]] || die "desktop entry not found: $DESKTOP_SOURCE"
[[ -f "$ICON_SOURCE" ]] || die "icon not found: $ICON_SOURCE"
[[ -f "$MANIFEST_SOURCE" ]] || die "application manifest not found: $MANIFEST_SOURCE"
[[ -f "$RELEASE_README_SOURCE" ]] || die "release README not found: $RELEASE_README_SOURCE"

VERSION="${VERSION:-$(sed -n 's/^[[:space:]]*<Version>\([^<]*\)<\/Version>[[:space:]]*$/\1/p' "$PROJECT" | head -n 1)}"
[[ -n "$VERSION" ]] || die "could not read <Version> from $PROJECT"
[[ "$VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+([-+][0-9A-Za-z.-]+)?$ ]] ||
    die "VERSION must be a semantic version such as 1.0.0 (got: $VERSION)"
ASSEMBLY_VERSION="${VERSION%%[-+]*}.0"

DOTNET_RID="${DOTNET_RID:-linux-x64}"
ARCH="${ARCH:-x86_64}"
DEB_ARCH="${DEB_ARCH:-amd64}"
PACKAGE_NAME="taskmanager"
APP_DIR_NAME="TaskManager.AppDir"
APP_IMAGE_NAME="TaskManager-${VERSION}-${ARCH}.AppImage"
DEB_NAME="${PACKAGE_NAME}_${VERSION}_${DEB_ARCH}.deb"

RELEASE_ROOT="${RELEASE_ROOT:-$REPO_ROOT/release}"
APP_IMAGE_OUTPUT_DIR="$RELEASE_ROOT/linux-x64/AppImage"
DEB_OUTPUT_DIR="$RELEASE_ROOT/linux-x64/deb"
APP_IMAGE_OUTPUT="$APP_IMAGE_OUTPUT_DIR/$APP_IMAGE_NAME"
DEB_OUTPUT="$DEB_OUTPUT_DIR/$DEB_NAME"

if [[ -n "${WORK_ROOT:-}" ]]; then
    WORK_ROOT="$(mkdir -p "$WORK_ROOT" && cd "$WORK_ROOT" && pwd)"
    KEEP_WORK=1
else
    WORK_ROOT="$(mktemp -d "${TMPDIR:-/tmp}/taskmanager-packaging.XXXXXX")"
    KEEP_WORK=0
fi

cleanup() {
    if [[ "$KEEP_WORK" -eq 0 ]]; then
        rm -rf "$WORK_ROOT"
    fi
}
trap cleanup EXIT

PUBLISH_DIR="$WORK_ROOT/publish"
APPDIR="$WORK_ROOT/$APP_DIR_NAME"
DEB_ROOT="$WORK_ROOT/deb-root"
GENERATED_MANIFEST="$WORK_ROOT/app.manifest"
APPIMAGETOOL_URL="https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage"

# Keep the source manifest unchanged, but embed the release version in its
# temporary copy so Windows manifest metadata matches the package version.
sed "s|<assemblyIdentity version=\"[^\"]*\"|<assemblyIdentity version=\"${ASSEMBLY_VERSION}\"|" \
    "$MANIFEST_SOURCE" > "$GENERATED_MANIFEST"

printf '==> Publishing self-contained %s build (version %s)\n' "$DOTNET_RID" "$VERSION"
rm -rf "$PUBLISH_DIR"
dotnet publish "$PROJECT" \
    -c Release \
    -r "$DOTNET_RID" \
    --self-contained true \
    -o "$PUBLISH_DIR" \
    -p:Version="$VERSION" \
    -p:AssemblyVersion="$ASSEMBLY_VERSION" \
    -p:FileVersion="$ASSEMBLY_VERSION" \
    -p:InformationalVersion="$VERSION" \
    -p:ApplicationManifest="$GENERATED_MANIFEST"

if find "$PUBLISH_DIR" -type f \( -iname 'tasks.db' -o -iname 'tasks.db-*' -o -iname 'tasks.db.*' \) -print -quit | grep -q .; then
    die "a development database was found in the publish output"
fi

[[ -x "$PUBLISH_DIR/TaskManager.UI" ]] || die "published executable is missing: $PUBLISH_DIR/TaskManager.UI"

printf '==> Validating desktop entry\n'
desktop-file-validate "$DESKTOP_SOURCE"

mkdir -p "$APP_IMAGE_OUTPUT_DIR" "$DEB_OUTPUT_DIR"
rm -f "$APP_IMAGE_OUTPUT" "$DEB_OUTPUT"

# ---------------------------------------------------------------------------
# Common staged files
# ---------------------------------------------------------------------------
ICON_REL="usr/share/icons/hicolor/256x256/apps/taskmanager.png"

write_appimage_desktop() {
    local destination="$1"
    sed 's|^Exec=.*|Exec=AppRun|' "$DESKTOP_SOURCE" > "$destination"
    chmod 0644 "$destination"
}

write_deb_desktop() {
    local destination="$1"
    cp "$DESKTOP_SOURCE" "$destination"
    chmod 0644 "$destination"
}

normalize_publish_payload() {
    local destination="$1"
    find "$destination" -type f -exec chmod 0644 {} +
    [[ ! -f "$destination/TaskManager.UI" ]] || chmod 0755 "$destination/TaskManager.UI"
    [[ ! -f "$destination/createdump" ]] || chmod 0755 "$destination/createdump"
}

normalize_directories() {
    find "$1" -type d -exec chmod 0755 {} +
}

# ---------------------------------------------------------------------------
# AppImage
# ---------------------------------------------------------------------------
printf '==> Building AppImage staging tree\n'
rm -rf "$APPDIR"
mkdir -p "$APPDIR/usr/lib/taskmanager" "$APPDIR/usr/bin" "$(dirname "$APPDIR/$ICON_REL")"

cp -a "$PUBLISH_DIR/." "$APPDIR/usr/lib/taskmanager/"
normalize_publish_payload "$APPDIR/usr/lib/taskmanager"
cp "$ICON_SOURCE" "$APPDIR/$ICON_REL"
cp "$ICON_SOURCE" "$APPDIR/taskmanager.png"
chmod 0644 "$APPDIR/$ICON_REL" "$APPDIR/taskmanager.png"
write_appimage_desktop "$APPDIR/taskmanager.desktop"
ln -s "$ICON_REL" "$APPDIR/.DirIcon"

cat > "$APPDIR/AppRun" <<'EOF'
#!/bin/sh
set -e
HERE="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
exec "$HERE/usr/lib/taskmanager/TaskManager.UI" "$@"
EOF
chmod 0755 "$APPDIR/AppRun"

cat > "$APPDIR/usr/bin/taskmanager" <<'EOF'
#!/bin/sh
set -e
HERE="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
exec "$HERE/../lib/taskmanager/TaskManager.UI" "$@"
EOF
chmod 0755 "$APPDIR/usr/bin/taskmanager"

desktop-file-validate "$APPDIR/taskmanager.desktop"
normalize_directories "$APPDIR"

APPIMAGETOOL_BIN="${APPIMAGETOOL:-}"
if [[ -z "$APPIMAGETOOL_BIN" ]]; then
    if command -v appimagetool >/dev/null 2>&1; then
        APPIMAGETOOL_BIN="$(command -v appimagetool)"
    else
        APPIMAGETOOL_BIN="$WORK_ROOT/appimagetool-x86_64.AppImage"
        printf '==> Downloading appimagetool\n'
        curl -fsSL --retry 3 --connect-timeout 20 -o "$APPIMAGETOOL_BIN" "$APPIMAGETOOL_URL"
        chmod 0755 "$APPIMAGETOOL_BIN"
    fi
fi
[[ -x "$APPIMAGETOOL_BIN" ]] || die "appimagetool is not executable: $APPIMAGETOOL_BIN"
file "$APPIMAGETOOL_BIN" | grep -q ELF || die "appimagetool does not look like an AppImage/ELF binary"

printf '==> Building %s\n' "$APP_IMAGE_NAME"
ARCH="$ARCH" APPIMAGE_EXTRACT_AND_RUN=1 "$APPIMAGETOOL_BIN" --no-appstream "$APPDIR" "$APP_IMAGE_OUTPUT"

# ---------------------------------------------------------------------------
# Debian package
# ---------------------------------------------------------------------------
printf '==> Building Debian package staging tree\n'
rm -rf "$DEB_ROOT"
mkdir -p \
    "$DEB_ROOT/DEBIAN" \
    "$DEB_ROOT/opt/taskmanager" \
    "$DEB_ROOT/usr/bin" \
    "$DEB_ROOT/usr/share/applications" \
    "$(dirname "$DEB_ROOT/$ICON_REL")"

cp -a "$PUBLISH_DIR/." "$DEB_ROOT/opt/taskmanager/"
normalize_publish_payload "$DEB_ROOT/opt/taskmanager"
cp "$ICON_SOURCE" "$DEB_ROOT/$ICON_REL"
chmod 0644 "$DEB_ROOT/$ICON_REL"
write_deb_desktop "$DEB_ROOT/usr/share/applications/taskmanager.desktop"

cat > "$DEB_ROOT/usr/bin/taskmanager" <<'EOF'
#!/bin/sh
set -e
exec /opt/taskmanager/TaskManager.UI "$@"
EOF
chmod 0755 "$DEB_ROOT/usr/bin/taskmanager"

# Dependencies are listed explicitly. dpkg-shlibdeps cannot resolve private
# self-contained .NET libraries reliably, so its generated substitutions were
# replaced with the libraries actually observed at runtime.
cat > "$DEB_ROOT/DEBIAN/control" <<EOF
Package: ${PACKAGE_NAME}
Version: ${VERSION}
Section: office
Priority: optional
Architecture: ${DEB_ARCH}
Maintainer: TaskManager <noreply@localhost>
Depends: libc6, libgcc-s1, libstdc++6, libfontconfig1, libx11-6, libx11-xcb1, libxext6, libxfixes3, libxi6, libxrandr2, libxcursor1, libgl1, libdbus-1-3
Description: Local-first workspace for goals, projects, tasks and notes
 TaskManager is a local-first desktop workspace for goals, projects,
 tasks and notes. User data is stored under ~/.local/share/TaskManager
 and is never removed by package removal or purge.
EOF
chmod 0644 "$DEB_ROOT/DEBIAN/control"

# Intentionally no maintainer scripts: installation and removal must never
# modify ~/.local/share/TaskManager.

normalize_directories "$DEB_ROOT"
fakeroot dpkg-deb --build "$DEB_ROOT" "$DEB_OUTPUT"

printf '==> Copying release documentation\n'
cp "$RELEASE_README_SOURCE" "$RELEASE_ROOT/README.md"
cp "$RELEASE_README_SOURCE" "$RELEASE_ROOT/RELEASE_README.md"

printf '==> Generating checksums\n'
ln -f "$APP_IMAGE_OUTPUT" "$RELEASE_ROOT/TaskManager-x86_64.AppImage"
ln -f "$DEB_OUTPUT" "$RELEASE_ROOT/taskmanager_amd64.deb"
(
    cd "$RELEASE_ROOT"
    sha256sum TaskManager-x86_64.AppImage taskmanager_amd64.deb > SHA256SUMS
)

printf '\nBuild complete:\n'
printf '  %s (%s)\n' "$APP_IMAGE_OUTPUT" "$(du -h "$APP_IMAGE_OUTPUT" | cut -f1)"
printf '  %s (%s)\n' "$DEB_OUTPUT" "$(du -h "$DEB_OUTPUT" | cut -f1)"
printf '  %s\n' "$RELEASE_ROOT/TaskManager-x86_64.AppImage"
printf '  %s\n' "$RELEASE_ROOT/taskmanager_amd64.deb"
printf '  %s\n' "$RELEASE_ROOT/SHA256SUMS"
printf '  %s\n' "$RELEASE_ROOT/RELEASE_README.md"

if [[ "$KEEP_WORK" -eq 1 ]]; then
    printf '  staging: %s\n' "$WORK_ROOT"
fi
