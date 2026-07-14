# Configuration Helper



## Install
Click "Utility - Configuration Helper.sb" in the repo then click the "Download" button to download it to your computer.

![](assets/github-download.png)

In Streamer.bot on your computer click the "Import" menu to open the import dialog.

![](assets/streamerbot-import.png)

On your computer, drag the "Utility - Configuration Helper.sb" file and drop it into the window. Click the "Import" button.

![](assets/import-dialog.png)

(There is a general video tutorial on importing into Streamer.bot here: https://youtu.be/gHqw3gwpbco)

## How to Use
In the Streamer.bot action that you are creating, add a set of “set argument” sub-actions to set up the various 
messages and values that you want to let users customize. Those argument names need to start with “config_”. 

After that section, add a sub-action to run the Configuration Helper action.

![](assets/config-arguments.png)

No additional code is needed.

## How It Works
The first time the action runs, it creates a `config` directory within the Streamer.bot directory if it doesn't already exist.
It then creates a configuration file and writes all of the current variables that start with "config_". The name of the 
configuration file will be the name of your Action with a ".config" extension. If your Action was named "My Awesome Alerts",
the file would be named "My Awesome Alerts.config".

![](assets/config-directory.png)

The content of the file will be a header followed by a line for each variable and value. The "_"'s in the variable names will be replaced with "."'s.

![](assets/config-file.png)

If the Configuration Helper action is run a second time in your Action, the tool will replace the values in the file
with all of the "config_" variables available at the time the helper runs. This can be used to add configurations to
the file after it was originally created.