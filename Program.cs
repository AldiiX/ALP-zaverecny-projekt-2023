namespace FinalProject {
    public static class RockPapersScissors {

        private class Player {
            public string? Name { get; set; }
            public int Wins { get; set; }
            public Object? SelectedObject { get; set; }
        }

        private class Object {

            public Object(string id) {
                switch (id) {

                    case "1":
                    case "rock": {
                            Name = "rock";
                            NameAlias = "kámen";
                            Destroys = "scissors";
                        }
                        break;

                    case "2":
                    case "scissors": {
                            Name = "scissors";
                            NameAlias = "nůžky";
                            Destroys = "paper";
                        }
                        break;

                    case "3":
                    case "paper": {
                            Name = "paper";
                            NameAlias = "papír";
                            Destroys = "rock";
                        }
                        break;

                    default: throw new Exception();
                }
            }

            public string Name { get; }
            public string NameAlias { get; }
            public string Destroys { get; }
        }

        private class Game {
            public int Round { get; set; } = 0;

            private static void SendPanel(string result, Player player, Player botplayer) {
                result = result switch {
                    "win" => "[lime] VYHRÁL JSI! [/]",
                    "loose" => "[red] PROHRÁL JSI! [/]",
                    "draw" => "[purple] REMÍZA [/]",
                    _ => throw new Exception()
                };

                var panel = new Panel($"[gray]Tvůj výběr:[/]         {player.SelectedObject?.NameAlias}\n[gray]Nepřítelův výběr:[/]   {botplayer.SelectedObject?.NameAlias}") {
                    Header = new PanelHeader(result).SetAlignment(Justify.Center)
                };

                AnsiConsole.Write(panel);
            }

            public static void RoundResult(string result, Player player, Player botplayer, Game game) {
                Console.Clear();
                AnsiConsole.Write(new FigletText($"{game.Round}. kolo").LeftJustified().Color(Color.Yellow1));
                Console.WriteLine();

                SendPanel(result, player, botplayer);

                switch (result) {
                    case "win": player.Wins++; break;
                    case "loose": botplayer.Wins++; break;

                    case "draw": {
                            botplayer.Wins++;
                            player.Wins++;
                        }
                        break;

                    default: throw new Exception();
                }

                DisplayScore(player, botplayer);
                Console.ReadKey();
            }

            public static void DisplayScore(Player player, Player botplayer, string title = "") {
                var table = new Table();

                table.AddColumn("[gray]Hráč[/]");
                table.AddColumn(new TableColumn("[gray]Skóre[/]").Centered());

                table.AddRow($"[blue]{player.Name}[/]", player.Wins.ToString());
                table.AddRow($"[blue]{botplayer.Name}[/]", botplayer.Wins.ToString());
                if (title != null) table.Title(title);

                AnsiConsole.Write(table);
            }

            public static string GenerateBotsUsername() {
                Random r = new Random();
                int rng = 0;
                void setRng() { rng = r.Next(0, 20); }

                string[] adjectiveWords = { "amazing", "brave", "clever", "daring", "elegant", "fearless", "gracious", "handsome", "intelligent", "jolly", "kind", "loyal", "magnificent", "noble", "optimistic", "passionate", "quick-witted", "reliable", "strong", "tough" };
                string[] nounWords = { "artist", "builder", "chef", "doctor", "engineer", "farmer", "guardian", "hero", "inventor", "jester", "knight", "leader", "musician", "ninja", "optimist", "pilot", "questioner", "scientist", "teacher", "warrior" }; ;
                int number = r.Next(1, 100);
                string name = "";

                setRng();
                name += char.ToUpper(adjectiveWords[rng][0]) + adjectiveWords[rng][1..];

                setRng();
                name += char.ToUpper(nounWords[rng][0]) + nounWords[rng][1..];
                name += number.ToString();

                return name;
            }

            public static void FinalResult(Player player, Player botplayer) {
                Console.Clear();
                AnsiConsole.Write(new FigletText($"KONEC HRY").LeftJustified().Color(Color.Orange1));
                Console.WriteLine();

                string winner;
                if (player.Wins > botplayer.Wins && player.Wins != botplayer.Wins) winner = player.Name;
                else if (player.Wins < botplayer.Wins && player.Wins != botplayer.Wins) winner = botplayer.Name;
                else winner = null;

                DisplayScore(player, botplayer, winner != null ? $"\n[yellow][underline]{winner}[/] vyhrál celou hru![/]" : "[yellow][underline]Remíza[/] - oba hráči mají stejné skóre.[/]");
            }

            public static void SelectionTimeout() {
                var waitHandle = new ManualResetEvent(false);
                Func<Task> task = async () => {
                    Console.WriteLine();
                    Console.WriteLine("Kámen");
                    await Task.Delay(600);
                    Console.WriteLine("Nůžky");
                    await Task.Delay(600);
                    Console.WriteLine("Papír");
                    await Task.Delay(600);

                    waitHandle.Set();
                };
                task();
                waitHandle.WaitOne();
            }
        }



        public static void Main() {

            Random random = new();
            string[] objects = { "Kámen", "Nůžky", "Papír" };
            string playerName = AnsiConsole.Prompt(
                new TextPrompt<string>("Zadej tvůj [blue]username[/]:\n[gray35]-     [/]")
                    .Validate(input => {
                        if (input.Length > 16) {
                            return ValidationResult.Error("[red][invert]\nTvůj username musí být dlouhý maximálně 16 znaků.\n[/][/]");
                        }
                        return ValidationResult.Success();
                    })
            );


            // loop, dokud uživatel neřekne, že chce skončit hru
            while (true) {

                // vytvoří se nové instance
                var game = new Game();
                var player = new Player() { Name = playerName.Trim() };
                var botplayer = new Player() { Name = Game.GenerateBotsUsername() };



                // loop - nová kola, dokud někdo nevyhraje
                while (true) {

                    if (player.Wins >= 5 || botplayer.Wins >= 5) break; // pokud někdo dosáhne 5 výher, tak končí hra



                    game.Round++;
                    Console.Clear();
                    AnsiConsole.Write(new FigletText($"{game.Round}. kolo").LeftJustified().Color(Color.Yellow1));



                    // uživatel vybere nástroj (kámen, nůžky, papír)
                    string selectedObjectName = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("")
                        .PageSize(15)
                        .AddChoices(objects)
                    );
                    //



                    Game.SelectionTimeout(); // -> odpočítávání do vyhodnocení kola



                    // uloží se, co uživatel vybral za nástroj + vygeneruje se náhodně nástroj, který bude mít nepřítel
                    player.SelectedObject = new Object((Array.IndexOf(objects, selectedObjectName) + 1).ToString());
                    botplayer.SelectedObject = new Object(random.Next(1, 4).ToString());



                    // vyhodnocení kola

                    // pokud je remíza
                    if (player.SelectedObject.Name == botplayer.SelectedObject.Name) {
                        Game.RoundResult("draw", player, botplayer, game);
                        continue;
                    }



                    // pokud hráč vyhraje
                    if (player.SelectedObject.Destroys == botplayer.SelectedObject.Name) {
                        Game.RoundResult("win", player, botplayer, game);
                        continue;
                    }



                    // pokud hráč prohraje
                    if (player.SelectedObject.Name == botplayer.SelectedObject.Destroys) {
                        Game.RoundResult("loose", player, botplayer, game);
                        continue;
                    }
                    //
                }


                // v případě, že skončil loop (někdo vyhrál)
                Game.FinalResult(player, botplayer);
                char cont = char.ToLower(AnsiConsole.Ask<char>("[orange1]\n\nChceš začít novou hru? (y/n):  [/]"));

                if (cont == 'n') break;
            }
        }
    }
}