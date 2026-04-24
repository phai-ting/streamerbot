# Prize Claim

Prize Claim makes it easier for a streamer to give prizes to people who are still watching ths stream. Via a `!claim`
command, the streamer can announce that a a user is eligible to claim a prize. That user then needs to respond with 
`!claim` in chat to show they are in the stream and claim the prize.

By default the user must respond within 2 minutes, but this default can be changed or even removed via the chat command.
When the timer is in effect, there is a warning message at 30 seconds remaining and then again at 15 seconds remaining.
These values as well as the messages are stored as variables in the action so they can be easily updated.

Examples:
- Set the claim timer to 5 minutes - `!claim 5`
- Set PhaiTing as the prize winner - `!claim phaiting`
- Set PhaiTing as the prize winner and also set the timer to 3 minutes - `!claim phaiting 3`
- Remove the claim timer while not changing the winner - `!claim 0`
- Remove the prize winner and stop the prize giveaway - `!claim`

Note: After importing this action, you will need to enable the "Prize Claim" command.