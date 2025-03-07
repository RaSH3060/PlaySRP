# PlaySRP   
**Steam Remote Play Launcher for Running Third-Party Applications with Remote Play Together Support**

## About the Project  
PlaySRP allows launching **any external applications** via Steam with **Remote Play Together** support, including:  
- Games not added to Steam  
- Applications like emulators (RetroArch, Redream)  
- GOG games and other third-party apps  

Works by replacing a Steam game's (donor) executable file with PlaySRP.exe while preserving Steam Overlay integration.

## Features ✅  
- Drag-and-Drop application adding  
- Launch parameter editing  
- Custom names for games in the list  
- Automatic icon generation  
- Steam Overlay compatibility (Shift+Tab)  
- Configuration file support (`play.ini`)

## Installation 🛠️  
1. **Download a Donor Game**  
   - Select any Remote Play-supported game in Steam

2. **Replace the Executable File**  
   - Navigate to the game's folder:  
     ```Steam\steamapps\common\Game_Name\```  
   - Rename `PlaySRP.exe` to match the original game executable's name (e.g., `game.exe`).

3. **Add Applications**  
   - Use the **+ ADD** button or drag-and-drop `.exe` files into the list.

4. **Renaming Entries**  
   - Right-click an entry → select **Rename**, enter a new name, and click **Save**.

5. **Launch Parameters**  
   - Right-click an entry → select **Launch Parameters**  
   - Add parameters (e.g., `-windowed`) and click **Save**.

6. **Launch via Steam Remote Play**  
   - Select an application in the list  
   - Click **▶ PLAY** to start through Steam

## System Requirements 💻  
- Windows 7 or newer  
- .NET Framework 4.7.2+  
- Installed Steam client  

**Developer:** [RaSH](https://github.com/RaSH3060)  

## License 📄  
MIT License


---------------------------------------------------------------------------------------------------------------------


# PlaySRP  
**Steam Remote Play Launcher для запуска сторонних приложений с поддержкой удалённого игрового процесса**

## О проекте  
PlaySRP позволяет запускать **любые внешние приложения** через Steam с поддержкой **Remote Play Together**, включая:  
- Игры, не добавленные в Steam  
- Приложения вроде эмуляторов (RetroArch, Redream)  
- Игры из GOG и другие сторонние приложения

Работает через подмену исполняемого файла Steam-игры (донора) на PlaySRP.exe, сохраняя интеграцию с Steam Overlay.

## Возможности ✅  
- Добавление приложений через Drag-and-Drop  
- Редактирование параметров запуска  
- Кастомные имена для игр в списке  
- Автоматическая генерация иконок  
- Совместимость с Steam Overlay (Shift+Tab)  
- Поддержка конфигурационных файлов (`play.ini`)

## Установка 🛠️  
1. **Скачайте донорскую игру**  
   - Выберите в Steam любую игру с поддержкой Remote Play

2. **Замена исполняемого файла**  
   - Перейдите в папку игры:  
     ```Steam\steamapps\common\Название_игры\```  
   - Переименуйте `PlaySRP.exe` в имя оригинального исполняемого файла игры (например, `game.exe`).

3. **Добавление игры**  
   - Добавьте приложения через кнопку **+ ADD** или перетащите `.exe` в список.

4. **Переименование**  
   - Щёлкните ПКМ по элементу → выберите **Rename**, введите новое название и нажмите **Save**.

5. **Параметры запуска**  
   - Щёлкните ПКМ по элементу → выберите **Launch Parameters**  
   - Добавьте параметры (например: `-windowed`) и нажмите **Save**.

6. **Запуск через Steam Remote Play**  
   - Выберите приложение в списке  
   - Нажмите **▶ PLAY** для старта через Steam

## Системные требования 💻  
- Windows 7 или новее  
- .NET Framework 4.7.2+  
- Установленный Steam клиент  

**Разработчик:** [RaSH](https://github.com/RaSH3060)  

## Лицензия 📄  
MIT License