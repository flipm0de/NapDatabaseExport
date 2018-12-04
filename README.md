# NAP Database Export
This is a simple project aimed to fulfil the requirements for regulation N-18 in Bulgaria.

This regulation requires every software that manages sales which are paid in cash to supply an open source tool to extract information from the database.

The exact text states:

_3. за софтуери, които се инсталират в среда на клиента, се представят: пълно описание на обектите в базата данни (БД), свързани с управлението на продажбите, вкл. таблици и предназначението им, връзки между тях, описание на полетата в таблиците, както и изпълним файл и source-кодът, от който е генериран изпълнимият файл, за достъп и извличане на данни от БД в структуриран четим вид с възможност за избор - от всички или от част от таблиците, с които работи софтуерът;_

## What this tool does

This tool consists of a single screen which allows you to:

1. Connect to a local database or a server.
2. List all databases (if you are connecting to a server).
3. List all tables in the database.
4. Select which tables to extract information from or select all.
5. Choose a file format for extraction.
6. Save all the data from each selected table in a separate file.

## Technologies used

C#, Windows Forms and .NET 4.0 (almost any software vendor can use this as it runs on Windows XP and should run on Linux, macOS with Mono)

Connects to SQLite, MySQL, Microsoft SQL Server and Microsoft Access databases. More can be added in the future from contributors.
