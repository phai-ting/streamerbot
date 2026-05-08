# List Functions

This Action provides various functions supporting a List datatype to native Streamer.bot actions. 
It works with lists using CSV formatting rules so it can also be used with rows from CSV files.

Note: The first item in a list is at position 0.

## Functions

### Count of Items in List
**Input**

| Variable     | Value                 |
|--------------|-----------------------|
| listFunction | "count"               |
| listArg      | the list to work with |

**Output**

| Variable  | Value                                           |
|-----------|-------------------------------------------------|
| intResult | integer value of the count of items in the list |

### Add Item to List
**Input**

| Variable     | Value                         |
|--------------|-------------------------------|
| listFunction | "add"                         |
| listArg      | the list to work with         |
| stringArg    | the string to add to the list |

**Output**

| Variable   | Value                                           |
|------------|-------------------------------------------------|
| listResult | the the list with the added item                |
| intResult  | integer value of the count of items in the list |

### Get Item at a Given Position in a List
**Input**

| Variable     | Value                               |
|--------------|-------------------------------------|
| listFunction | "get"                               |
| listArg      | the list to work with               |
| intArg       | the position of the desired element |

**Output**

| Variable     | Value                                       |
|--------------|---------------------------------------------|
| stringResult | the item at the specified index of the list |

### Set Item at a Given Position in a List
**Input**

| Variable     | Value                                                                    |
|--------------|--------------------------------------------------------------------------|
| listFunction | "set"                                                                    |
| listArg      | the list to work with                                                    |
| intArg       | the position where the item should replace the existing item in the list |
| stringArg    | the item to add to the list                                              |

**Output**

| Variable   | Value                           |
|------------|---------------------------------|
| listResult | the list with the replaced item |

### Remove the First Item from a List
**Input**

| Variable     | Value         |
|--------------|---------------|
| listFunction | "removeFirst" |

**Output**

| Variable     | Value                                           |
|--------------|-------------------------------------------------|
| listResult   | the list with the item removed                  |
| stringResult | the item that was removed from the list         |
| intResult    | integer value of the count of items in the list |

### Remove the Last Item from a List
**Input**

| Variable     | Value                 |
|--------------|-----------------------|
| listFunction | "removeLast"          |
| listArg      | the list to work with |

**Output**

| Variable     | Value                                           |
|--------------|-------------------------------------------------|
| listResult   | the list with the item removed                  |
| stringResult | the item that was removed from the list         |
| intResult    | integer value of the count of items in the list |

### Remove the Item at a Given Position in a List
**Input**

| Variable     | Value                                  |
|--------------|----------------------------------------|
| listFunction | "removeAt"                             |
| listArg      | the list to work with                  |
| intArg       | the position of the item to be removed |

**Output**

| Variable     | Value                                           |
|--------------|-------------------------------------------------|
| listResult   | the list with the item removed                  |
| stringResult | the item that was removed from the list         |
| intResult    | integer value of the count of items in the list |

### Remove a Specified Item From a List
**Input**

| Variable     | Value                                                                    |
|--------------|--------------------------------------------------------------------------|
| listFunction | "remove"                                                                 |
| listArg      | the list to work with                                                    |
| stringArg    | the item to be removed from the list. Only the first instance is removed |

**Output**

| Variable   | Value                                           |
|------------|-------------------------------------------------|
| listResult | the list with the item removed                  |
| intResult  | integer value of the count of items in the list |

### Position of an Item in a List
**Input**

| Variable     | Value                              |
|--------------|------------------------------------|
| listFunction | "indexOf"                          |
| listArg      | the list to work with              |
| stringArg    | the item to be located in the list |

**Output**

| Variable  | Value                                                                                 |
|-----------|---------------------------------------------------------------------------------------|
| intResult | the position of the item in the list. If the item is not found, this value will be -1 |

### Sort the Items in a List
**Input**

| Variable     | Value                 |
|--------------|-----------------------|
| listFunction | "sort"                |
| listArg      | the list to work with |

**Output**

| Variable   | Value                                                     |
|------------|-----------------------------------------------------------|
| listResult | the sorted list sorted using the default C# sorting rules |

### Reverse the order of Items in a List
**Input**

| Variable     | Value                 |
|--------------|-----------------------|
| listFunction | "reverse"             |
| listArg      | the list to work with |

**Output**

| Variable   | Value             |
|------------|-------------------|
| listResult | the reversed list |
