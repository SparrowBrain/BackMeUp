# BackMeUp
### Automatically backup your Ubisoft savegames
![](https://github.com/SparrowBrain/BackMeUp/workflows/CI/badge.svg)
## What is it? ##
It's a Windows application that:
* runs in the background;
* periodically scans Ubisoft Connect's `savegames` direcotry for any changes;
* copies newest saves to a backup directory;
* stores every backed up save in a path with:
  * a readable game name;
  * a timestamp of when the backup was made;
* has a tray icon that shows application state;
* can autostart on Windows startup, so you don't need to remember.

It is something I've written after losing my 24 hour save. I don't expect it would happen again, but running this removes the anxiety.

## Requirements ##
This tool uses `.NET Framework 4.6`.
* If you're using Windows 10, you should be good to go.
* Otherwise download the newest version of `.NET Framework Runtime` [here](https://dotnet.microsoft.com/download/dotnet-framework).

## Installation ##
For now **BackMeUp** is published as a simple `.zip` file. Extract it wherever you fancy (ex. `C:\BackMeUp`). Feel free to add a desktop shortcut, or configure automatic startup (below).

### Start Up ###
If you would like the tool to automatically start after boot up, run the `startup-add.bat`. This will add shortcut to `BackMeUp.exe` in `%USERPROFILE%\Start Menu\Programs\StartUp`.

Don't forget to run `startup-remove.bat` before deleting/moving the tool folder to clean up.

## Configuration ##
**BackMeUp** has two configuration files:
* `config.json`
* `games.json`

### `config.json` ###
Main configuration file.
* **BackupDirectory** - place to copy save files to. If configured directory does not exist, it will be created. Take notice of double backslash (`\\`) when specifying a path. *Default is "C:\Ubisoft_savegame_Backups"*.
* **BackupPeriod** - how often backups should be created. The format is `HH:mm:ss` as in `hours:minutes:seconds`. *Default is 1 hour*.

Example:
```
{
    "BackupDirectory" : "E:\\My_Ubisoft_Backups",
    "BackupPeriod" : "00:10:00" 
}
```
This configuration will create backups in `E:\My_Ubisoft_Backups` and run every 10 minutes.

### `games.json` ###
A list that maps Ubisoft game id to a given game. The format is:

`"game_id": "game_name",`

Example:
```
{
    "Games": {
        "437": "Assassin's Creed IV Black Flag",
        "3539": "Assassin's Creed Origins",
        "46": "Far Cry 3",
        "205": "Far Cry 3 Blood Dragon",
        "1803": "Far Cry 5",
        "1843": "Tom Clancy's Rainbow Six Siege",
        "3353": "Watch Dogs Legion"
    }
}
```

The name is then used to create a backup folder for a specific game. If you want to add a game, just add another line. Notice that the last game doesn't have a comma after it.

## Restoring save files ##
### Ubisoft save file structure ###
Ubisoft saves by default are stored in `%PROGRAMFILES(X86)%\Ubisoft\Ubisoft Game Launcher\savegames`. Folder structure is:
```
savegames
        - user_id
               - game_1_id
               - game_2_id
               ...
```
For example:
```
savegames
        - 87762840-157e-48d1-b3e9-0d70750ef62e
               - 46
               - 3353
               ...
```

### Restoring your backups ###
The backups are stored in this structure:

`BackupDirectory\game\date\savegames\user_id\game_id`

For example:

`BackupDirectory\Far Cry 3\2015-01-30\savegames\87762840-157e-48d1-b3e9-0d70750ef62e\46`

To restore a backup:
1. Close the game
2. Close Ubisoft Connect
3. Copy the `savegames` folder from the backup you want to restore to `%PROGRAMFILES(X86)%\Ubisoft\Ubisoft Game Launcher`, overwritting `savegames`. Overwrite any files when asked.
4. Profit!?!!

## FAQ ##
### The game is listed as something like 123456_Unidentified ###
This means that **BackMeUp** does not recongnize the given game id and cannot use a sensible name.

If you know what game was backed up, you can enter its id in [`games.json`](https://github.com/SparrowBrain/BackMeUp#gamesjson) file. If folder is named `123456_Unidentified`, `123456` will be your game id.

Better yet, you can raise an issue/pull request, so we could include the id in a future releases.

### How to submit issues ###
If something unexpected happens with **BackMeUp** please create an issue ticket.

Attach any logs to the issue ticket that were generated when the issue happened.

Please provide steps to reproduce the issue, if possible.

## Disclaimer ##
This tool had very limited testing done to it. It is not endorsed by Ubisoft. I cannot guarantee your backed up save files will work. I cannot guarantee that backing up save files will not ruin your existing saves. In fact, [Ubisoft states that this tool will not be able to backup some of their games](https://support.ubisoft.com/en-GB/Article/000063179). While I haven't encountered any issues, I cannot guarantee you won't.

Use it at your own risk.

## Acknowledgments 
This tool is largely inspired by Walker Moore's (fauxtronic) [acrbackup.bat script](https://steamcommunity.com/app/201870/discussions/0/864976837949032506/#c864977564087259945).

`startup-add.bat` is basically [this response](https://superuser.com/questions/455364/how-to-create-a-shortcut-using-a-batch-script) by [Dennis](https://superuser.com/users/101836/dennis) in a superuser thread.

CRC16 from [Sanity Free](http://www.sanity-free.com/134/standard_crc_16_in_csharp.html)

Most of the [game ids](https://github.com/SparrowBrain/BackMeUp/blob/master/BackMeUp/games.json) are taken from [Haoose's UPLAY_GAME_ID](https://github.com/Haoose/UPLAY_GAME_ID)
