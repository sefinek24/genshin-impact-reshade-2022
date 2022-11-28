@echo off
title Cache removal tool - Genshin Impact Mod Pack 2023
chcp 65001 > nul
echo.⠀  ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀
echo.⠀ ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿
echo.⠀  ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact ReShade 2023 Mod Pack
echo.  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                     Cache removal tool
echo.⠀  ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿
echo.   ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟
echo. ⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀
echo. ⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀
echo.  ⠀⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      * Mod version: v3.0.0 [SV_21112022_300N-001]
echo.   ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    * ReShade version: v5.5.1
echo.  ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     * FPS unlocker version: v2.0.3
echo. ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟
echo.  ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟
echo ========================================================================================= & echo.

echo 1/3 - Administrative permissions are required. Please wait...

net session >nul 2>&1
if %errorLevel% == 0 (
    echo [✓] No problems found. & echo.
) else (
    echo [x] Error. Run this file as administrator.
    goto nothing_to_do
)


echo 2/3 - Checking %temp%\ReShade...
if exist "%temp%\ReShade" (
    goto question
    echo.
) else (
    echo [✓] Not found. & echo.
    goto log_file
)


:question
    set /p deleteFolder="[i] Delete cache? (y/n): "
    if "%deleteFolder%" == "y" (
        goto delete
    ) else if "%deleteFolder%" == "n" (
        goto no
    ) else (
        goto bad_answer
    )

:delete
    echo [i] Please wait...
    rmdir /s /q %temp%\ReShade
    echo [✓] Done.

    goto log_file

:log_file
    echo 3/3 - Checking %ProgramFiles%\Genshin Impact\Genshin Impact game\ReShade.log...
    if exist "%ProgramFiles%\Genshin Impact\Genshin Impact game\ReShade.log" (
        goto delete_log_file
    ) else (
        echo [✓] Not found.
        goto nothing_to_do
    )

:delete_log_file
    set /p deleteLogFile="[i] Delete log file? (y/n): "

    if "%deleteLogFile%" == "y" (
        echo [i] Please wait...
        del "%ProgramFiles%\Genshin Impact\Genshin Impact game\ReShade.log"
        echo [✓] Done.

        goto nothing_to_do
    ) else if "%deleteLogFile%" == "n" (
        goto no
    ) else (
        goto bad_answer
    )

:no
    echo [i] Bruh. Okay, no problem.
    goto nothing_to_do

:bad_answer
    echo.
    echo [x] Wrong answer. Expected: 'y' or 'n'. Click ENTER to try again.
    echo [i] y = yes
    echo [i] n = no
    set /p null=
    cls
    call Data\delete_cache.cmd

:nothing_to_do
    echo.
    echo [i] You can close this window.
    set /p null=
    exit