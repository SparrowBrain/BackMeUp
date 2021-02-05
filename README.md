# BackMeUp - automatic save backups for Ubisoft games
## Intro ##


## Requirements ##
This tool uses `.NET Framework 4.6`.
* If you're using Windows 10, you should be good to go.
* Otherwise download the newest version of `.NET Framework` Runtime [here](https://dotnet.microsoft.com/download/dotnet-framework).

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
-- under construction

## Restoring save files ##
### Ubisoft save file structure ###
### Restoring your backups ###

## FAQ ##
### The game is listed as something like 123456_Unidentified ###
This means that **BackMeUp** does not recongnize the given game id and cannot use a sensible name.

If you know what game was backed up, you can enter it's id in `games.json` file. If folder is named `123456_Unidentified`, `123456` will be your game id.

Better yet, you can raise an issue/pull request, so we could include the id in future releases.

### How to submit issues ###



## Disclaimer ##
This tool had very limited testing done to it. It is not endorsed by Ubisoft. I cannot guarantee your backed up save files will work. I cannot guarantee that backing up save files will not ruin your existing saves. In fact, [Ubisoft states that this tool will not be able to backup some of their games](https://support.ubisoft.com/en-GB/Article/000063179). While I haven't encountered any issues, I cannot guarantee you won't.

Use it at your own risk.

## Acknowledgments 
This tool is largely inspired by Walker Moore's (fauxtronic) [acrbackup.bat script](https://steamcommunity.com/app/201870/discussions/0/864976837949032506/#c864977564087259945).

`startup-add.bat` is basically [this response](https://superuser.com/questions/455364/how-to-create-a-shortcut-using-a-batch-script) by [Dennis](https://superuser.com/users/101836/dennis) in a superuser thread.

CRC16 from [Sanity Free](http://www.sanity-free.com/134/standard_crc_16_in_csharp.html)