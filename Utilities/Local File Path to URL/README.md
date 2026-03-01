# Local File Path to URL

This StreamerBot Action will take a local file path and convert it to a "file" URL. 

For example, "C:\temp\overlay.html" would be converted to "file:///C:/temp/overlay.html".

Set the local file path in the "localFilePath" variable. The URL version will be saved in "localFilePathUrl".

If the original file path was actually a URL, it will be saved in "localFilePathUrl".