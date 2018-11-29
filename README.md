# NapDatabaseExport
This is a simple project aimed to fulfil the requirements for regulation N-18 in Bulgaria.

This regulation requires every software that manages sales which are paid in cash to supply an open source tool to extract information from the database.

## What this tool does

This tool consists of a single screen which allows you to:

1. Connect to a local database or a server.
2. List all databases on that server.
3. List all tables in the database.
4. Select which tables to extract information from.
5. Choose a file format for extraction.
6. Save all the data from each selected table in a separate file.

## Technologies used

C# and .NET 4.0 (runs on Windows XP so almost any software vendor can use this)
Connects to SQLite, MySQL and Microsoft SQL Server databases. More can be added in the future from contributors.
