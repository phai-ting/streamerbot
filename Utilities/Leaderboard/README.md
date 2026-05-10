# Leaderboard

This action provides a persistent leaderboard that can be used by other Streamer.bot Actions.

Multiple leaderboards are supported by specifying different leaderboard names.

To clear a leaderboard, the data can be deleted in Streamer.bot's Global Variables interface.

## Functions

### Get Count of Users on the Leaderboard
**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "count"                                 |

**Output**

| Variable  | Value                                  |
|-----------|----------------------------------------|
| intResult | the number of users on the leaderboard |

### Set Score for User
**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "set"                                   |
| userArg           | the name of the user                    |
| scoreArg          | the user's score                        |

### Increment Score for User
**Input**

| Variable          | Value                                    |
|-------------------|------------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard.  |
| leaderboardAction | "increment"                              |
| userArg           | the name of the user                     |
| scoreArg          | the amount to increment the user's score |

**Output**

| Variable          | Value                    |
|-------------------|--------------------------|
| userResult        | the name of the user     |
| scoreResult       | the user's current score |

### Decrement Score for User
**Input**

| Variable          | Value                                    |
|-------------------|------------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard.  |
| leaderboardAction | "decrement"                              |
| userArg           | the name of the user                     |
| scoreArg          | the amount to decrement the user's score |

**Output**

| Variable          | Value                    |
|-------------------|--------------------------|
| userResult        | the name of the user     |
| scoreResult       | the user's current score |

### Get User
**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "getUser"                               |
| userArg           | the name of the user                    |

**Output**

| Variable       | Value                                          |
|----------------|------------------------------------------------|
| userResult     | the name of the user                           |
| scoreResult    | the user's current score                       |
| rankResult     | the user's current rank                        |
| positionResult | the user's current position in the leaderboard |

### Get User(s) with Specified Rank
Note: Since users can be tied for a rank, there could be more than 1 user returned.

**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "getRank"                               |
| rankArg           | the rank of users to return             |

**Output**

| Variable                                                  | Value                                      |
|-----------------------------------------------------------|--------------------------------------------|
| leaderboard_count                                         | the number of leaderboard entries returned |
| leaderboard#_user (# is the row number starting at 1)     | the name of the user                       |
| leaderboard#_score (# is the row number starting at 1)    | the score of the user                      |
| leaderboard#_rank (# is the row number starting at 1)     | the rank of the user                       |
| leaderboard#_position (# is the row number starting at 1) | the user's position in the leaderboard     |

### Get the Top # of Users by Rank
Note: Since users can be tied for a rank, there could be more users returned than the number of the max rank.

**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "top"                                   |
| rankArg           | the maximum rank of users to return     |

**Output**

| Variable                                                  | Value                                      |
|-----------------------------------------------------------|--------------------------------------------|
| leaderboard_count                                         | the number of leaderboard entries returned |
| leaderboard#_user (# is the row number starting at 1)     | the name of the user                       |
| leaderboard#_score (# is the row number starting at 1)    | the score of the user                      |
| leaderboard#_rank (# is the row number starting at 1)     | the rank of the user                       |
| leaderboard#_position (# is the row number starting at 1) | the user's position in the leaderboard     |

### Get the Top # of Users by Rank
Note: Since users can be tied for a rank, there could be more users returned than the number of the max rank.

**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "top"                                   |
| rankArg           | the maximum rank of users to return     |

**Output**

| Variable                                                  | Value                                      |
|-----------------------------------------------------------|--------------------------------------------|
| leaderboard_count                                         | the number of leaderboard entries returned |
| leaderboard#_user (# is the row number starting at 1)     | the name of the user                       |
| leaderboard#_score (# is the row number starting at 1)    | the score of the user                      |
| leaderboard#_rank (# is the row number starting at 1)     | the rank of the user                       |
| leaderboard#_position (# is the row number starting at 1) | the user's position in the leaderboard     |

### Get the Entire Leaderboard
**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "all"                                   |

**Output**

| Variable                                                  | Value                                      |
|-----------------------------------------------------------|--------------------------------------------|
| leaderboard_count                                         | the number of leaderboard entries returned |
| leaderboard#_user (# is the row number starting at 1)     | the name of the user                       |
| leaderboard#_score (# is the row number starting at 1)    | the score of the user                      |
| leaderboard#_rank (# is the row number starting at 1)     | the rank of the user                       |
| leaderboard#_position (# is the row number starting at 1) | the user's position in the leaderboard     |

### Remove User from the Leaderboard
**Input**

| Variable          | Value                                   |
|-------------------|-----------------------------------------|
| leaderboardName   | (Optional) the name of the leaderboard. |
| leaderboardAction | "remove"                                |
| userArg           | the name of the user                    |

**Output**

| Variable  | Value                             |
|-----------|-----------------------------------|
| intResult | the number of leaderboard entries |


