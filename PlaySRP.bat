@echo off
setlocal

REM Укажите путь к компилятору C# (csc.exe)
set "cscPath=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe"

REM Укажите путь к вашему исходному файлу
set "sourceFile=PlaySRP.cs"

REM Укажите имя выходного файла
set "outputFile=PlaySRP.exe"

REM Укажите путь к иконке приложения
set "iconFile=256x256.ico"

REM Дополнительные параметры компиляции:
REM /target:winexe   - создает Windows-приложение (без окна консоли)
REM /optimize+       - включает оптимизацию кода
REM /nologo          - отключает вывод заголовка компилятора
REM /win32icon       - добавляет иконку в приложение
set "compilerOptions=/target:winexe /optimize+ /nologo /win32icon:"%iconFile%""

echo Компиляция с иконкой "%iconFile%"...
"%cscPath%" %compilerOptions% /out:"%outputFile%" "%sourceFile%"

if errorlevel 1 (
    echo Ошибка компиляции.
) else (
    echo Компиляция завершена. Файл "%outputFile%" создан.
)

pause
endlocal