# First Words - User Intros

PREREQUISITE: This collection of Actions is dependent on my "Run Action by Name" Action which is also in this repository. Import that Action before trying to import this one.

This collection os StreamerBot Actions will allow you to run a collection of sub-Actions after a user submits their First Words in a stream. It can be a single sub-Action like a greeting, but it can also be more information like an message introducing that person to the rest of the stream as well as anything that StreamerBot is capable of.

The logic included here allows for:
* Running user specific Actions
* Running an Action for moderators
* Running an Action for VIPs
* Running an Action for Subscribers
* Running an Action for users that don't fit the above categories

There is a main "User Intros" Action which will then call an Action file based on the user.

After importing this code into StreamerBot, there will be an "Intros" Action group. In here there will be a number of 
standalone Actions for each type of greeting or alert.

To implement the user specific Actions, create an Action named "intro_" and then the username in all lowercase letters. 
There is an example "intro_phaiting" in that group.

When the main "User Intros" Action runs, it will look for an Action named using this pattern and run it. This Action 
only needs to include the sub-Actions that you want to run and it does not need to include any other logic.

If a user specific Action is not found, the logic will then check to see what user category Action to run.