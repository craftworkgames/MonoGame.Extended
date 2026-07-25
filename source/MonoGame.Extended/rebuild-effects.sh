#!/usr/bin/env bash

set -e

MANIFEST=".config/dotnet-tools.json"
INPUT="Graphics/Effects/Resources/DefaultEffect.fx"
OUTPUT_PREFIX="Graphics/Effects/Resources/DefaultEffect"

echo "Restoring .NET tools..."
dotnet tool restore

echo "Compiling MonoGame shaders..."
dotnet mgfxc "$INPUT" "${OUTPUT_PREFIX}.ogl.mgfxo"  /Profile:OpenGL
dotnet mgfxc "$INPUT" "${OUTPUT_PREFIX}.dx11.mgfxo" /Profile:DirectX_11
dotnet mgfxc "$INPUT" "${OUTPUT_PREFIX}.dx12.mgfxo" /Profile:DirectX_12
dotnet mgfxc "$INPUT" "${OUTPUT_PREFIX}.vk.mgfxo"   /Profile:Vulkan

echo "Finding fxc.exe..."

FXC="${FXC:-}"

if [ -z "$FXC" ]; then
    FXC="$(command -v fxc.exe 2>/dev/null || true)"
fi

if [ -z "$FXC" ]; then
    for candidate in /c/Program\ Files\ \(x86\)/Windows\ Kits/10/bin/*/x64/fxc.exe; do
        if [ -x "$candidate" ]; then
            FXC="$candidate"
            break
        fi
    done
fi

if [ -z "$FXC" ]; then
    echo "Error: Could not find fxc.exe."
    echo "Set the FXC environment variable to its full path."
    exit 1
fi

echo "Compiling FNA-compatible shader..."

if command -v cygpath >/dev/null 2>&1; then
    WINDOWS_INPUT="$(cygpath -w "$INPUT")"
    WINDOWS_OUTPUT="$(cygpath -w "${OUTPUT_PREFIX}.fxb")"
else
    WINDOWS_INPUT="$INPUT"
    WINDOWS_OUTPUT="${OUTPUT_PREFIX}.fxb"
fi

MSYS2_ARG_CONV_EXCL="*" \
    "$FXC" /Tfx_2_0 /Fo"$WINDOWS_OUTPUT" "$WINDOWS_INPUT"

echo "Finding knifxc.exe..."

KNIFXC="${KNIFXC:-}"

if [ -z "$KNIFXC" ]; then
    KNIFXC="$(command -v KNIFXC.exe 2>/dev/null || true)"
fi

if [ -z "$KNIFXC" ]; then
    for candidate in /c/Program\ Files\ \(x86\)/KNI/*/Tools/KNIFXC.exe; do
        if [ -x "$candidate" ]; then
            KNIFXC="$candidate"
            break
        fi
    done
fi

if [ -z "$KNIFXC" ]; then
    echo "Error: Could not find KNIFXC.exe."
    echo "Set the KNIFXC environment variable to its full path."
    exit 1
fi

echo "Compiling KNI-compatible shader..."
"$KNIFXC" "$INPUT" "${OUTPUT_PREFIX}.knifxo"  /Backend:DirectX11 /Backend:OpenGL /Backend:GLES

echo "Done."