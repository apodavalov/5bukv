# *5Letters* Game Bot

*5Letters* (russian: *5букв*) game and its rules can be found [here](https://5bukv.tinkoff.ru).

# Installation

1. Install dotnet SDK 8.0. Follow the [instructions](https://dotnet.microsoft.com/en-us/download).
1. [Download](https://github.com/apodavalov/5bukv/archive/refs/heads/main.zip) this repository.
1. Unpack the downloaded zip-archive.
1. Open *Command Prompt* (Windows) or *Terminal* (Unix) and proceed to the directory with unpacked 
   data. Thus, the current directory should contain `russian5.txt`. Use `cd` command to change directory.
1. Execute the following command to compile the project.

```shell
dotnet build -c Release
```

You should see *Build succeeded.*

# Quick Start

## Interactive mode (Bot)

Execute the following command to start the bot.

```shell
5LettersBin/bin/Release/net8.0/5LettersBin interactive russian5.txt норка
```

It works almost instantly. No waiting time.

## Get First Suggest

The first word that should be entered into the game is always the same. Use the following command to find it.

```shell
5LettersBin/bin/Release/net8.0/5LettersBin first russian5.txt
```

It usually takes about 4 minutes.

## Collect Statistics

This mode simulates playing the game. The application picks up each word from dictionary one by one
and starts the game. Once all the rounds completed it shows the collected statistics.

Execute the following command to start.

```shell
5LettersBin/bin/Release/net8.0/5LettersBin stats russian5.txt норка
```

It usually takes about 12 minutes.

## Get Metric for a Word

This mode computes a metric for a candidate across the dictionary.

Execute the following command to start.

```shell
5LettersBin/bin/Release/net8.0/5LettersBin metric russian5.txt норка
```

It works almost instantly. No waiting time.

## Help

Just execute the following command.

```shell
5LettersBin/bin/Release/net8.0/5LettersBin
```

Enjoy!
