using System;
using System.Collections;
using System.Collections.Generic;

public class CPHInline
{
	public bool Execute()
	{
		List<LeaderboardEntry> leaderboard;

		if (!CPH.TryGetArg("leaderboardName", out string leaderboardName))
		{
			leaderboardName = "";
		}
		CPH.TryGetArg("leaderboardAction", out string leaderboardAction);
		if (CPH.TryGetArg("userArg", out string userArg))
		{
			userArg = userArg.ToLower();
		}
		CPH.TryGetArg("scoreArg", out int scoreArg);
		CPH.TryGetArg("rankArg", out int rankArg);

		leaderboard = loadLeaderboard(leaderboardName);

		LeaderboardEntry entry;
		string userResult = "";
		int scoreResult = 0;
		int rankResult = 0;
		int positionResult = 0;
		int intResult = 0;
		bool isLeaderboardDirty = false;
		switch(leaderboardAction.ToLower())
		{
			case "count":
				intResult = leaderboard.Count;
			break;
			case "set":
				entry = updateScoreForUser(userArg, scoreArg, true, leaderboard);
				isLeaderboardDirty = true;
				userResult = entry.User;
				scoreResult = entry.Score;
			break;
			case "increment":
				entry = updateScoreForUser(userArg, scoreArg, false, leaderboard);
				isLeaderboardDirty = true;
				userResult = entry.User;
				scoreResult = entry.Score;
			break;
			case "decrement":
				entry = updateScoreForUser(userArg, 0 - scoreArg, false, leaderboard);
				isLeaderboardDirty = true;
				userResult = entry.User;
				scoreResult = entry.Score;
			break;
			case "getuser":
				entry = getEntryForUser(userArg, leaderboard);
				userResult = entry.User;
				scoreResult = entry.Score;
				rankResult = entry.Rank;
				positionResult = entry.Position;
			break;
			case "getrank":
				List<LeaderboardEntry> leaderboardSubset = new List<LeaderboardEntry>();
				foreach (LeaderboardEntry row in leaderboard)
				{
					if (row.Rank > rankArg)
					{
						break;
					}
					if (row.Rank == rankArg)
					{
						leaderboardSubset.Add(row);
					}
				}
				getTopUsers(0, leaderboardSubset);
			break;
			case "top":
				getTopUsers(rankArg, leaderboard);
			break;
			case "all":
				getTopUsers(0, leaderboard);
			break;
			case "remove":
				removeEntryForUser(userArg, leaderboard);
				intResult = leaderboard.Count;
				isLeaderboardDirty = true;
			break;
		}

		if (isLeaderboardDirty)
		{
			leaderboard.Sort(((s1, s2) => s2.Score.CompareTo(s1.Score)));
			saveLeaderboard(leaderboard, leaderboardName);
		}

		CPH.SetArgument("userResult", userResult);
		CPH.SetArgument("scoreResult", scoreResult);
		CPH.SetArgument("rankResult", rankResult);
		CPH.SetArgument("positionResult", positionResult);
		CPH.SetArgument("intResult", intResult);
		return true;
	}

	public List<LeaderboardEntry> loadLeaderboard(string leaderboardName)
	{
		List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
		string variableName = leaderboardName + "Leaderboard";
		string rawLeaderboard = CPH.GetGlobalVar<string>(variableName, true);
		if (rawLeaderboard == null)
		{
			return leaderboard;
		}
		string[] rawEntries = rawLeaderboard.Split(';');
		int position = 1;
		int rank = 0;
		int lastScore = 0;
		foreach (string rawEntry in rawEntries)
		{
			string[] entryParts = rawEntry.Split(':');
			int score;
			Int32.TryParse(entryParts[1], out score);
			if (score != lastScore)
			{
				rank++;
			}
			LeaderboardEntry entry = new LeaderboardEntry(entryParts[0], score);
			entry.Position = position;
			entry.Rank = rank;
			leaderboard.Add(entry);
			position++;
			lastScore = score;
		}
		return leaderboard;
	}

	public void saveLeaderboard(List<LeaderboardEntry> leaderboard, string leaderboardName) {
		string variableName = leaderboardName + "Leaderboard";
		List<string> rawEntries = new List<string>();
		foreach (LeaderboardEntry entry in leaderboard)
		{
			rawEntries.Add(entry.User + ":" + entry.Score);
		}
		string rawLeaderboard = string.Join(";", rawEntries);
		CPH.SetGlobalVar(variableName, rawLeaderboard, true);
	}

	public LeaderboardEntry updateScoreForUser(string user, int scoreDelta, bool forceSet, List<LeaderboardEntry> leaderboard)
	{
		LeaderboardEntry entry = removeEntryForUser(user, leaderboard);
		if (entry == null)
		{
			entry = new LeaderboardEntry();
			entry.User = user;
		}
		if (forceSet)
		{
			entry.Score = scoreDelta;
		}
		else
		{
			entry.Score = entry.Score + scoreDelta;
		}
		leaderboard.Add(entry);
		return entry;
	}

	public LeaderboardEntry getEntryForUser(string user, List<LeaderboardEntry> leaderboard)
	{
		foreach (LeaderboardEntry entry in leaderboard)
		{
			if (entry.User.Equals(user))
			{
				return entry;
			}
		}
		return null;
	}

	public LeaderboardEntry removeEntryForUser(string user, List<LeaderboardEntry> leaderboard)
	{
		LeaderboardEntry returnEntry = null;
		int pos = 0;
		int foundPos = -1;
		foreach (LeaderboardEntry entry in leaderboard)
		{
			if (entry.User.Equals(user))
			{
				foundPos = pos;
				break;
			}
			pos++;
		}
		if (foundPos > -1)
		{
			returnEntry = leaderboard[pos];
			leaderboard.RemoveAt(pos);
		}
		return returnEntry;
	}

	public void getTopUsers(int maxRank, List<LeaderboardEntry> leaderboard)
	{
		int pos = 1;
		foreach (LeaderboardEntry entry in leaderboard)
		{
			if (maxRank != 0 && entry.Rank > maxRank)
			{
				break;
			}
			CPH.SetArgument("leaderboard_count", pos);
			CPH.SetArgument("leaderboard"+pos+"_user", entry.User);
			CPH.SetArgument("leaderboard"+pos+"_score", entry.Score);
			CPH.SetArgument("leaderboard"+pos+"_rank", entry.Rank);
			CPH.SetArgument("leaderboard"+pos+"_position", entry.Position);
			pos++;
		}
	}

	public class LeaderboardEntry
	{
		public string User;
		public int Score;
		public int Rank;
		public int Position;

		public LeaderboardEntry()
		{
		}

		public LeaderboardEntry(string user, int score)
		{
			this.User = user;
			this.Score = score;
		}

		public int CompareTo(LeaderboardEntry entry)
    	{
        	if (entry == null)
			{
				return 1;
			}
			else
			{
				if (this.Score == entry.Score)
				{
					return this.User.CompareTo(entry.User);
				}
				else return this.Score.CompareTo(entry.Score);
			}
    	}
	}
}