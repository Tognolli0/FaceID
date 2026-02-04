@echo off
:: Corrige a codificação de caracteres para UTF-8 (resolve os ??)
chcp 65001 > nul

title Sistema FaceID - Biometria Facial
color 0B

echo ====================================================
echo           INICIALIZANDO MOTOR DE IA...
echo ====================================================
echo.

:: Executa o projeto diretamente pelo dotnet run
dotnet run

if %errorlevel% neq 0 (
    echo.
    echo [ERRO] Ocorreu um problema ao iniciar o programa.
    echo Verifique se o .NET SDK e o Python estao instalados.
    pause
)