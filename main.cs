﻿// Stanislav Škudrna - IT1A             ALP - závěrečný projekt
global using Spectre.Console;



AnsiConsole.Markup("[yellow1]ALP - Závěrečný projekty[/]");
AnsiConsole.Markup("[grey35]\nStanislav Škudrna IT1A\n\n\n\n[/]");

string selectedProject = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
    .Title("Vyber projekt, který se spustí")
    .PageSize(15)
    .AddChoices("Rock, Paper, Scissors Game", "Launcher Login Saver")
);

switch(selectedProject) {
    case "Rock, Paper, Scissors Game": {
        Console.Clear();
        RockPaperScissors.Main();
    } break;
    case "Launcher Login Saver": {
        Console.Clear();
        LauncherLoginSaver.Main(); 
    } break;
}