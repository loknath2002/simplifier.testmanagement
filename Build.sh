#!/usr/bin/env bash

set -euo pipefail

PROJECT="./Simplifier.TestManagement/Simplifier.TestManagement.csproj"
DIST_DIR="./dist"
CONFIGURATION="Release"

if ! command -v dotnet >/dev/null 2>&1; then
    echo "Error: dotnet is not installed or not on PATH." >&2
    exit 1
fi

if ! command -v zip >/dev/null 2>&1; then
    echo "Error: zip is not installed or not on PATH." >&2
    exit 1
fi

VERSION="${1:-${GITHUB_REF_NAME:-$(git describe --tags --always --dirty 2>/dev/null || echo "dev")}}"
VERSION="${VERSION//\//-}"

echo "Building Simplifier Test Management ${VERSION}"
echo "Configuration: ${CONFIGURATION}"

rm -rf "${DIST_DIR}"
mkdir -p "${DIST_DIR}"

build_platform() {
    local name="$1"
    local rid="$2"
    local package_dir="${DIST_DIR}/Simplifier.TestManagement-${VERSION}"
    local archive="${DIST_DIR}/Simplifier.TestManagement-${VERSION}-${name}.zip"

    echo ""
    echo "Publishing ${name} (${rid})..."
    dotnet publish "${PROJECT}" \
        --configuration "${CONFIGURATION}" \
        --runtime "${rid}" \
        --self-contained true \
        --output "${package_dir}" \
        -p:DebugSymbols=false \
        -p:DebugType=None

    echo "Creating ${archive}"
    (
        cd "${DIST_DIR}"
        zip -r -q "$(basename "${archive}")" "$(basename "${package_dir}")"
    )

    rm -rf "${package_dir}"
}

build_platform "windows-x64" "win-x64"
build_platform "macos-arm64" "osx-arm64"
build_platform "linux-x64" "linux-x64"

echo ""
echo "Build complete:"
ls -lh "${DIST_DIR}"/*.zip