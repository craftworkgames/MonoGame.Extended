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
            "$FXC_PATH" /T fx_2_0 /Fo "$FX_DIR/$filename.fxb" "$FX_DIR/$filename.fx"
        fi
    fi
done

read -p "Press enter to continue"
