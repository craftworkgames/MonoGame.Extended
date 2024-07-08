#!/bin/bash

FX_DIR="./Graphics/Effects/Resources"
MGFXC="dotnet mgfxc"

if [ ! -d "$FX_DIR" ]; then
    echo "Error: Directory $FX_DIR not found."
    exit 1
fi

for file in "$FX_DIR"/*.fx; do
    if [ -f "$file" ]; then
        filename=$(basename "$file" .fx)
        
        $MGFXC "$FX_DIR/$filename.fx" "$FX_DIR/$filename.ogl.mgfxo" /Profile:OpenGL
        $MGFXC "$FX_DIR/$filename.fx" "$FX_DIR/$filename.dx11.mgfxo" /Profile:DirectX_11
    fi
done

read -p "Press enter to continue"
