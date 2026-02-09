using System;

public class CPHInline
{
	public bool Execute()
	{
		CPH.TryGetArg("runActionName", out string runActionName);
		if (CPH.ActionExists(runActionName))
    	{
			// Code to execute if the action exists
			CPH.LogDebug($"The action '{runActionName}' exists.");
			CPH.RunAction(runActionName); // You can then run the action
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