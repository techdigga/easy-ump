#!/usr/bin/env bash

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

log() {
  echo "[validate-package] $1"
}

fail() {
  echo "[validate-package] ERROR: $1" >&2
  exit 1
}

require_file() {
  local path="$1"
  [[ -f "$path" ]] || fail "Missing required file: $path"
}

require_file "package.json"
require_file "README.md"
require_file "CHANGELOG.md"
require_file "README.md.meta"
require_file "CHANGELOG.md.meta"
require_file "package.json.meta"
log "Required package files: OK"

version="$(sed -n 's/^[[:space:]]*"version":[[:space:]]*"\([^"]*\)".*/\1/p' package.json | head -n 1)"
[[ -n "$version" ]] || fail "Unable to read version from package.json"
[[ "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]] || fail "package.json version is not semver: $version"
log "package.json version: $version"

top_changelog_version="$(grep -E '^## [0-9]+\.[0-9]+\.[0-9]+$' CHANGELOG.md | head -n 1 | cut -d' ' -f2)"
[[ -n "$top_changelog_version" ]] || fail "Unable to read top version from CHANGELOG.md"
[[ "$top_changelog_version" == "$version" ]] || fail "Top CHANGELOG.md version ($top_changelog_version) does not match package.json version ($version)"
log "CHANGELOG.md top version matches package.json: OK"

grep -q '"name":[[:space:]]*"com.techdigga.easy-ump"' package.json || fail "package.json name is not com.techdigga.easy-ump"
grep -q 'com.techdigga.easy-ump' README.md || fail "README.md does not mention the package name"
log "Package identity checks: OK"

missing_meta=0
meta_checked_count=0
while IFS= read -r file; do
  case "$file" in
    *.meta)
      continue
      ;;
  esac

  meta_checked_count=$((meta_checked_count + 1))
  if [[ ! -f "${file}.meta" ]]; then
    echo "[validate-package] ERROR: Missing Unity .meta file for $file" >&2
    missing_meta=1
  fi
done < <(
  {
    find Editor Runtime Tests -type f \( -name '*.cs' -o -name '*.asmdef' -o -name '*.png' \)
    find Editor -maxdepth 1 -type f -name '*.xml'
  } | sort -u
)

[[ "$missing_meta" -eq 0 ]] || exit 1
log "Unity .meta checks: OK ($meta_checked_count files checked)"

log "Package validation passed for version $version."
