#!/bin/bash

FX_DIR="./Graphics/Effects/Resources"
MGFXC="dotnet mgfxc"
FXC="${FXC:-}"

find_fxc() {
    if [ -n "$FXC" ] && [ -x "$FXC" ]; then
        echo "$FXC"
        return
    fi

    if command -v fxc.exe >/dev/null 2>&1; then
        command -v fxc.exe
        return
    fi

    for candidate in /c/Program\ Files\ \(x86\)/Windows\ Kits/10/bin/*/x64/fxc.exe; do
        if [ -x "$candidate" ]; then
            echo "$candidate"
            return
        fi
    done
}

to_windows_path() {
    if command -v cygpath >/dev/null 2>&1; then
        cygpath -w "$1"
        return
    fi

    echo "$1"
}

if [ ! -d "$FX_DIR" ]; then
    echo "Error: Directory $FX_DIR not found."
    exit 1
fi

FXC_PATH="$(find_fxc)"

for file in "$FX_DIR"/*.fx; do
    if [ -f "$file" ]; then
        filename=$(basename "$file" .fx)
        
        $MGFXC "$FX_DIR/$filename.fx" "$FX_DIR/$filename.ogl.mgfxo" /Profile:OpenGL
        $MGFXC "$FX_DIR/$filename.fx" "$FX_DIR/$filename.dx11.mgfxo" /Profile:DirectX_11

        if [ -n "$FXC_PATH" ]; then
            windows_output="$(to_windows_path "$FX_DIR/$filename.fxb")"
            windows_input="$(to_windows_path "$FX_DIR/$filename.fx")"
            MSYS2_ARG_CONV_EXCL="*" "$FXC_PATH" /Tfx_2_0 /Fo"$windows_output" "$windows_input"
        fi
    fi
done

read -p "Press enter to continue"
