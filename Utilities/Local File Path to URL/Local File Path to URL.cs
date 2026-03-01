using System;

// Convert a local file path to URL format
public class CPHInline
{
	public bool Execute()
	{
		// Read the variable "localFilePath" to use as the desired file path
		CPH.TryGetArg("localFilePath", out string localFilePath);
		// If the path contains "://" than assume it is already a URL and just return it
		if (!localFilePath.Contains("://"))
		{
			// Convert the local path to a File URL path and URL encode special characters
			// Windows doesn't like some URL encoded characters so undo those
			string pathUrl = "file:///" + CPH.UrlEncode(localFilePath)
				.Replace("+", "%20")
				.Replace("%3A", ":")
				.Replace("%5C", "/");
			CPH.SetArgument("localFilePathUrl", pathUrl);
		}
		else
		{
			// It looks like the file path is already in URL form so just return it
			CPH.SetArgument("localFilePathUrl", localFilePath);
		}
		return true;
	}
}