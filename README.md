# NRA Database Export

<p align="center">
    <a href="https://github.com/DavidHoldingAD/NraDatabaseExport/graphs/contributors" alt="Contributors">
        <img src="https://img.shields.io/github/contributors/DavidHoldingAD/NraDatabaseExport" />
    </a>
    <a href="https://github.com/DavidHoldingAD/NraDatabaseExport/pulse" alt="Activity">
        <img src="https://img.shields.io/github/commit-activity/m/DavidHoldingAD/NraDatabaseExport" />
    </a>
</p>

### Introduction

This project is aimed to fulfil the requirements of [Regulation N-18](https://lex.bg/laws/ldoc/2135540645) by the Minister of Finance of
Republic of Bulgaria.

The regulation requires every vendor of a software that manages sales paid in cash to provide the source code of a tool to extract related
data from the database.

The exact text of art.52в, p.2.3 states:

> 3. (доп. - ДВ, бр. 26 от 2019 г., в сила от 29.03.2019 г.) за софтуери, които се инсталират в среда на клиента, се представят: пълно
> описание на обектите в базата данни (БД), свързани с управлението на продажбите, вкл. таблици и предназначението им, връзки между тях,
> описание на полетата в таблиците, както и изпълним файл и source-кодът, от който е генериран изпълнимият файл, за достъп и извличане на
> данни от БД в структуриран четим вид с възможност за избор - от всички или от част от таблиците, с които работи софтуерът, когато
> информацията, съдържаща се в експортираните от изпълнимия файл таблици, представлява лични данни по смисъла на Регламент (ЕС) 2016/679 на
> Европейския парламент и на Съвета от 27 април 2016 (ОВ, L119 от 4 май 2016 г.) или здравна информация по смисъла на чл. 27 от Закона за
> здравето, при експорта тези данни не се извличат;

### System Requirements

In order for the application to work correctly, the following system requirements should be met:

- Microsoft Windows 7+ (64-bit)
- .NET Core 3.1+

### Supported Databases

- SQLite
- MySQL/MariaDB
- Microsoft SQL Server
- Microsoft Access
- Firebird Server
- ODBC

### Supported File Formats

- CSV - a text file, the values of which are delimited in accordance with the current regional settings
- Comma-separated CSV - a text file, the values of which are delimited with a "," (comma) symbol
- Semicolon-separated CSV - a text file, the values of which are delimited with a ";" (semicolon) symbol
- JSON - a text file following [RFC 8259](https://tools.ietf.org/html/rfc8259)

### Contribution

This project is a fork of [NapDatabaseExport](https://github.com/flipm0de/NapDatabaseExport).
