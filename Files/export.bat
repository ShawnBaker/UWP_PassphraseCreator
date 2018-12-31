@echo off

call :export back back normal
call :export back_disabled back disabled
call :export back_hover back hover
call :export back_pressed back pressed
call :export button_small button_small normal
call :export button_small_disabled button_small disabled
call :export button_small_hover button_small hover
call :export button_small_pressed button_small pressed
call :export copy copy normal
call :export copy_disabled copy disabled
call :export copy_hover copy hover
call :export copy_pressed copy pressed
call :export info info normal
call :export info_disabled info disabled
call :export info_hover info hover
call :export info_pressed info pressed
call :export question question normal
call :export question_disabled question disabled
call :export question_hover question hover
call :export question_pressed question pressed
goto :eof

:export
if not exist "..\Assets\%~2" mkdir ..\Assets\%~2
inkscape -f images.svg -i %~1 -j -h 40 -e ..\Assets\%~2\%~3.scale-100.png
inkscape -f images.svg -i %~1 -j -h 56 -e ..\Assets\%~2\%~3.scale-140.png
inkscape -f images.svg -i %~1 -j -h 72 -e ..\Assets\%~2\%~3.scale-180.png
inkscape -f images.svg -i %~1 -j -h 96 -e ..\Assets\%~2\%~3.scale-240.png
goto :eof
