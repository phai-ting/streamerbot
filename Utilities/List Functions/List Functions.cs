using System;
using System.Text;
using System.Collections.Generic;

public class CPHInline
{
	/// <summary>
	/// Provide list functions to Streamer.bot native Actions.
	/// These functions follow CSV formatting rules.
	/// See: https://github.com/phai-ting/streamerbot/tree/main/Utilities/List%20Functions
	/// </summary>
	/// <param name="listFunction">This defines which list function will be executed.</param>
	/// <param name="listArg">The string representation of the list to perform functions upon.</param>
	/// <param name="stringArg">The string argument ff the chosen list function requires one.</param>
	/// <param name="intArg">The integer argument ff the chosen list function requires one.</param>
	/// <returns name="listResult">If the executed list function returns a list this will hold the result.</returns>
	/// <returns name="stringResult">If the executed list function returns a string this will hold the result.</returns>
	/// <returns name="intResult">If the executed list function returns an integer this will hold the result.</returns>
	/// <returns name="boolResult">If the executed list function returns a boolean this will hold the result.</returns>
	public bool Execute()
	{
		CPH.TryGetArg("listFunction", out string listFunction);
		CPH.TryGetArg("listArg", out string listArg);
		CPH.TryGetArg("stringArg", out string stringArg);
		CPH.TryGetArg("intArg", out int intArg);

		bool success = true;
		List<string> listResult = StringToList(listArg);
		string stringResult = null;
		int intResult = -1;
		bool boolResult = false;

		switch(listFunction.ToLower())
		{
			case "count":
				intResult = listResult.Count;
				break;
			case "add":
				listResult.Add(stringArg);
				intResult = listResult.Count;
				break;
			case "get":
				if (intArg < listResult.Count)
				{
					stringResult = listResult[intArg];
				}
				else
				{
					success = false;
				}
				break;
			case "set":
				if (intArg < listResult.Count)
				{
					listResult[intArg] = stringArg;
				}
				else
				{
					success = false;
				}
				break;
			case "removefirst":
				if (listResult.Count > 0)
				{
					stringResult = listResult[0];
					listResult.RemoveAt(0);
					intResult = listResult.Count;
				}
				else
				{
					success = false;
				}
				break;
			case "removelast":
				if (listResult.Count > 0)
				{
					var index = listResult.Count - 1;
					stringResult = listResult[index];
					listResult.RemoveAt(index);
					intResult = listResult.Count;
				}
				else
				{
					success = false;
				}
				break;
			case "removeat":
				if (intArg < listResult.Count)
				{
					stringResult = listResult[intArg];
					listResult.RemoveAt(intArg);
					intResult = listResult.Count;
				}
				else
				{
					success = false;
				}
				break;
			case "remove":
				listResult.Remove(stringArg);
				intResult = listResult.Count;
				break;
			case "contains":
				boolResult = listResult.Contains(stringArg);
				break;
			case "indexof":
				intResult = listResult.IndexOf(stringArg);
				break;
			case "sort":
				listResult.Sort();
				break;
			case "reverse":
				listResult.Reverse();
				break;
		}

		CPH.SetArgument("listResult", ListToString(listResult));
		CPH.SetArgument("stringResult", stringResult);
		CPH.SetArgument("intResult", intResult);
		CPH.SetArgument("boolResult", boolResult);
		return success;
	}

	/// <summary>
	/// Parse a string into a List of strings
	/// This function follows CSV formatting rules.
	/// </summary>
	/// <param name="listString">String representation of a comma delimited list.</param>
	/// <returns>List of strings created from the input.</returns>
	public List<string> StringToList(string listString) {
		var fields = new List<string>();
		bool inQuotes = false;
		var currentField = new StringBuilder();

		for (int i = 0; i < listString.Length; i++)
		{
			char c = listString[i];

			if (c == '\"')
			{
				// Handle escaped quotes ("")
				if (inQuotes && i + 1 < listString.Length && listString[i + 1] == '\"')
				{
					currentField.Append('\"');
					i++; // Skip next quote
				}
				else
				{
					inQuotes = !inQuotes; // Toggle quote state
				}
			}
			else if (c == ',' && !inQuotes)
			{
				fields.Add(currentField.ToString());
				currentField.Clear();
			}
			else
			{
				currentField.Append(c);
			}
		}
		fields.Add(currentField.ToString());
		return fields;
	}

	/// <summary>
	/// Converts a list of strings to a string representation.
	/// This function follows CSV formatting rules.
	/// </summary>
	/// <param name="items">A list of strings.</param>
	/// <returns>String representation of a comma delimited list created from the input.</returns>
	public string ListToString(List<string> items)
	{
		if (items == null || items.Count == 0) return string.Empty;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < items.Count; i++)
        {
            string item = items[i] ?? "";

            // If the item contains a comma or a double quote, wrap it in quotes
            if (item.Contains(",") || item.Contains("\""))
            {
                // Escape existing double quotes by doubling them (standard CSV rule)
                string escaped = item.Replace("\"", "\"\"");
                sb.Append("\"").Append(escaped).Append("\"");
            }
            else
            {
                sb.Append(item);
            }

            // Add comma separator for all items except the last one
            if (i < items.Count - 1)
            {
                sb.Append(",");
            }
        }

        return sb.ToString();
	}
}