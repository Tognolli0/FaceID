@echo off
title Instalador de Dependencias - FaceID
echo ====================================================
echo   INSTALADOR DE AMBIENTE: BIOMETRIA FACIAL
echo ====================================================
echo.

echo [1/3] Ativando suporte a caminhos longos (Registro)...
powershell.exe -Command "Start-Process powershell -ArgumentList '-Command New-ItemProperty -Path \"HKLM:\System\CurrentControlSet\Control\FileSystem\" -Name \"LongPathsEnabled\" -Value 1 -PropertyType DWORD -Force' -Verb RunAs"

echo.
echo [2/3] Instalando bibliotecas Python (DeepFace, TensorFlow, OpenCV)...
python -m pip install --upgrade pip
python -m pip install deepface tf-keras opencv-python

echo.
echo [3/3] Verificando instalacao do .NET...
dotnet --version >nul 2>&1  
if %errorlevel% neq 0 (
    echo [AVISO] .NET SDK nao encontrado. Por favor, instale o .NET 8 SDK.
) else (
    echo [OK] .NET SDK detectado.
)

echo.
echo ====================================================
echo   PROCESSO CONCLUIDO! 
echo   Se o modelo .h5 ja estiver na pasta .deepface,
echo   voce ja pode rodar 'dotnet run'.
echo ====================================================
pause