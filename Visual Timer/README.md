# Visual Timer Command

This Visual Timer Command will add a "!vtimer" command. To start a timer, use the format:

!vtimer hh:mm:ss

To start a timer for 30 seconds, you can use the command:

!vtimer 30

A timer for 5 minutes would be:

!vtimer 5:00

To cancel the timer before it has completed, use:

!vtimer

### OBS
In OBS, create a new Browser source for the Visual Timer. Be sure the "Local file" checkbox is off. I am hosting the 
sample Visual Timer overlays at:
https://phai-ting.github.io/visual-timer/

You can use these hosted overlays, or you can download the overlays in this repository to use as a base for your own.
Visual Timer can accept local file paths or URLs. In OBS, the "Local" file" checkbox has to be off even if you are going
to use a local file.

For the basic timer, use the following settings which can also be found in the "basic-timer.html" file:
- URL: https://phai-ting.github.io/visual-timer/basic-timer.html
- Width: 400
- Height: 125
- Shutdown source when not visible: ON
- Refresh browser when scene becomes active: ON

For the timer bar, use the following settings which can also be found in the "timer-bar.html" file:
- URL: https://phai-ting.github.io/visual-timer/timer-bar.html
- Width: 400
- Height: 40
- Shutdown source when not visible: ON
- Refresh browser when scene becomes active: ON

### StreamerBot
In StreamerBot, select the Visual Timer action that was imported.

Open the **Visual Timer Configuration** to set it up. Set the value of `visualTimerOverlay` to the URL or the local file 
path of the overlay. When using a local path from your computer, do not include quotation marks around it.
Set the value of `visualTimerHideDelaySeconds` to the number of seconds to wait after the timer hits 0 before it is hidden. You can leave it at the default.

Go to the **"Visual Timer Start"** action and configure it for the scene and source of your Visual Timer Broswer source.

Go to the **"Visual Timer Hide"** action and configure it for the scene and source of your Visual Timer Broswer source.

NOTE: This collection of Actions is dependent on my "Local File Path to URL" Action.

A tutorial video is on YouTube here: https://www.youtube.com/watch?v=B7YVV1uqK2w