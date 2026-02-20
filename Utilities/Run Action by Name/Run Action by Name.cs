using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.TryGetArg("runActionName", out string runActionName);
		CPH.TryGetArg("runActionNameCheckOnly", out string checkOnly);
		if (CPH.ActionExists(runActionName))
    	{
			// Code to execute if the action exists
			CPH.LogDebug($"The action '{runActionName}' exists.");
			if (!"true".Equals(checkOnly, StringComparison.OrdinalIgnoreCase))
			{
				// The Action exists and this is not a Check Only. Runt it.
				CPH.RunAction(runActionName);
			}
			else
			{
				CPH.LogDebug($"Check Only was set to '{checkOnly}'.");
			}
			CPH.SetArgument("runActionResult", true);
		}
		else
		{
			// Code to execute if the action does not exist
			CPH.LogDebug($"The action '{runActionName}' does not exist.");
			CPH.SetArgument("runActionResult", false);
		}
		return true;
	}
}