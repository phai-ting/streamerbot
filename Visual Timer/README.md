# Visual Timer Command

This Visual Timer Command will add a "!vtimer" command. To start a timer, use the format:

!vtimer hh:mm:ss

To start a timer for 30 seconds, you can use the command:

!vtimer 30

A timer for 5 minutes would be:

!vtimer 5:00

To cancel the timer before it has completed, use:

!vtimer

## Set up
Download all of the files in the "Overlays" directory to your local machine. 
The files must stay together in the same directory.
Next, import the "Visual Timer.sb" file into StreamerBot.

### OBS
In OBS, create a new Browser source for the Visual Timer. Turn on the "Local file" checkbox and choose one of the timer 
overlay HTML files. As a example, choose "basic-timer.html". After you choose the file, turn off the "Local file" 
checkbox and OBS will provide a URL path for the file. You will need this later.

For the basic timer, use the following settings which can also be found in the "basic-timer.html" file:
- Width: 400
- Height: 125
- Shutdown source when not visible: ON
- Refresh browser when scene becomes active: ON

### StreamerBot
In StreamerBot, select the Visual Timer action that was imported.

Open the "MODIFY - Configuration" sub-action group. Set the value of timerOverlyPath to the overlay URL from OBS 
that we got earlier. Set the value of visualTimerHideDelaySeconds to the number of seconds to wait after the timer hits 0 before it is hidden. You can leave it at the default.

Go to the first "OBS Studio Source Visibility State" and configure it to set the Visual Timer overlay to visible.

Go to the second "OBS Studio Source Visibility State" and configure it to set the Visual Timer overlay to hidden.


