setlocal

SET TWOMGFX="mgfxc"

@for /f %%f IN ('dir /b *.fx') do (

  call %TWOMGFX% %%~nf.fx %%~nf.ogl.mgfxo /Profile:OpenGL

  call %TWOMGFX% %%~nf.fx %%~nf.dx11.mgfxo /Profile:DirectX_11

)

endlocal

pause