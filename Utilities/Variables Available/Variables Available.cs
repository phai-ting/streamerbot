using System;
using System.Text;
using System.Collections.Generic;

public class CPHInline
{
	public bool Execute()
	{
        string variableListStr = "";
        CPH.TryGetArg("variableFilter", out string variableFilter);

        List<string> filteredVariableList = new();
        foreach (var arg in args)
        {
            if (variableFilter == null || arg.Key.StartsWith(variableFilter))
            {
                filteredVariableList.Add(arg.Key);
            }
        }
        variableListStr = string.Join(",", filteredVariableList);

        CPH.SetArgument("variablesAvailable", variableListStr);
		return true;
	}
}