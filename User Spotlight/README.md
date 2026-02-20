# User Spotlight

Highlight select active users in chat on a periodic interval. 

The streamer can create a Spotlight action for select users. This Spotlight is only triggered if the user is active in chat.

This action can do something as simple as posting a message
to chat to highlight the user, or it can even do something more sophisticated like triggering actions in OBS. Basically,
the Spotlight can do anything that Streamer.bot is capable of doing.

After a streamer defined period, Streamer.bot will run a Spotlight for the next person who posts something in chat and
has a Spotlight Action created.

## Configuration
In the "Spotlight" Action, the streamer can configure:
- spotlightIntervalMinutes - The number of minutes to wait until the next Spotlight
- spotlightDelaySeconds - The number of seconds to wait between a Spotlight being triggered and when the Spotlight Action runs

To choose a user to have a Spotlight, create an Action in the "Spotlight" group in the form:
`spotlight_%userName%`. A Spotlight Action for "PhaiTing" would be named `spotlight_phaiting`. This Action will be run
when it comes to Spotlight the user. This can include any functionality that Streamer.bot has available. As mentioned, 
it could be a chat message. It could also trigger a video, GDI Text source, or scene to play in OBS. Be creative.

If there are some common sub-Actions that you want to run for every Spotlight, you can put that in the "Spotlight Intro" 
Action. You could use this to hold a general Spotlight template that includes variables to make it apply to all users. 
If all of desired the functionality can be captured in the common Spotlight Intro, there still needs to be a 
"spotlight_%userName%" Action created even if it is empty. It is the existence of this personalized Action that allows 
the Spotlight knows which users to run a Spotlight for.

## Spotlight Command
In addition to the automated Spotlight Action, there is also a `!spotlight` command that can be used to immediately spotlight a user.

Example:
`!spotlight phaiting`

Triggering a Spotlight manually will also restart the automated Spotlight timer.