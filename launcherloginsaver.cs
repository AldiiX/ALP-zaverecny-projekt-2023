class LauncherLoginSaver {

    private static class Program {
        public static string[] Platforms { get; } = { "Steam", "Discord", "Spotify", "XBOX", "Riot", "BattleNet", "Epic Games"};
        public static string[] Options { get; } = { "1. Přidat / změnit launcher login", "2. Odstranit launcher login", "3. Vypsat launcher login podle názvu", "4. Vypsat všechny launcher loginy", "[red]5. UKONČIT[/]" };



        public static void AddLauncherLogin() {

            string selectedLauncherName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title(Options[0][3..])
                .PageSize(15)
                .AddChoices(Platforms)
                .AddChoices("[red]ZPĚT[/]")
            );


            if (selectedLauncherName == "[red]ZPĚT[/]") return;


            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Zadej tvoje [blue]jméno[/]:\n[gray35]-     [/]")
                    .Validate(input => {
                        if (input.Length > 32) {
                            return ValidationResult.Error("[red][invert]\nTvoje jméno musí být dlouhý maximálně 32 znaků.\n[/][/]");
                        }
                        return ValidationResult.Success();
                    })
            );

            Console.WriteLine();

            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("Zadej tvoje [blue]heslo[/]:\n[gray35]-     [/]")
                    .Secret('*')
                    .Validate(input => {
                        /*if (input.Length > 16) {
                            return ValidationResult.Error("[red][invert]\nTvůj username musí být dlouhý maximálně 16 znaků.\n[/][/]");
                        }*/
                        return ValidationResult.Success();
                    })
            );



            File.WriteAllText($"{selectedLauncherName.ToLower().Replace(' ', '_')}.txt", $"{name};{password}");



            Console.Clear();
            AnsiConsole.Markup($"[darkgreen]Nové přihlašovací údaje pro [green]{selectedLauncherName}[/] byly uloženy.[/]");
            Console.ReadKey();
        }

        public static void DeleteLauncherLogin() {
            string selectedLauncherName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title(Options[1][3..])
                .PageSize(15)
                .AddChoices(Platforms)
                .AddChoices("[red]ZPĚT[/]")
            );

            string selectedLauncherId = selectedLauncherName.ToLower().Replace(' ', '_');

            if (selectedLauncherName == "[red]ZPĚT[/]") return;

            if(!File.Exists($"{selectedLauncherId}.txt")) {
                Console.Clear();
                AnsiConsole.Markup($"[red][invert]Na platformě {selectedLauncherName} nejsou uložena žádná data.[/][/]");
                Console.ReadKey();
                return;
            }

            char input = AnsiConsole.Prompt(
                new TextPrompt<char>($"[blue]Opravdu chceš smazat data z platformy [underline]{selectedLauncherName}[/]? [grey62](y/n)[/]:   [/]")
                    .ValidationErrorMessage("[red][invert]\nŠpatný input, zadej y/n\n[/][/]")
                    .Validate(input => {
                        if (!char.IsLetter(input)) {
                            return ValidationResult.Error("[red][invert]\nZadej prosím y/n\n[/][/]");
                        }

                        return ValidationResult.Success();
                    })
            );

            if (input != 'y') {
                Console.Clear();
                AnsiConsole.Markup("[green]Akce zrušena.[/]");
                Console.ReadKey();
                return;
            }

            File.Delete($"{selectedLauncherId}.txt");
            Console.Clear();
            AnsiConsole.Markup($"[darkgreen]Data z platformy [green]{selectedLauncherName}[/] byla odstraněna.[/]");
            Console.ReadKey();
        }

        public static void GetLauncherLoginByName() {

            string selectedLauncherName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title(Options[2][3..])
                .PageSize(15)
                .AddChoices(Platforms)
                .AddChoices("[red]ZPĚT[/]")
            );

            string launcherCompiledName = selectedLauncherName.ToLower().Replace(' ','_');


            if(selectedLauncherName == "[red]ZPĚT[/]") return;

            if(!File.Exists(launcherCompiledName + ".txt")) {
                Console.Clear();
                AnsiConsole.Markup($"[red]{selectedLauncherName} nemá uložené žádné přihlašovací údaje.[/]");
                Console.ReadKey();
                
                return;
            }



            string[] data = File.ReadAllText(launcherCompiledName + ".txt").Split(';');
            string username = data[0];
            string password = data[1];



            AnsiConsole.Write(new FigletText(selectedLauncherName.ToUpper()).LeftJustified().Color(Color.Yellow1));
            Console.WriteLine();

            var panel = new Panel($"Jméno:   {username}\nHeslo:   {password}");

            AnsiConsole.Write(panel);
            Console.ReadKey();
        }

        public static void GetAllLaunchers() {
            var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Width(60)
            .AddColumns("[grey35]Platforma[/]", "[grey35]Jméno[/]", "[grey35]Heslo[/]");

            foreach (var p in Platforms) {
                string platformId = p.ToLower().Replace(' ','_');
                string name = "", password = "";

                if(File.Exists(platformId + ".txt")) {
                    string[] data = File.ReadAllText(platformId + ".txt").Split(';');
                    name = data[0];
                    password = data[1];
                }

                table.AddRow($"[blue]{p}[/]", name, password);
            }

            AnsiConsole.Write(table); 
            Console.ReadKey();
        }
    }



    public static void Main() {
        while (true) {
            Console.Clear();

            

            AnsiConsole.Markup("[grey62]-----[/] [blue]Launcher Login Saver[/] [grey62]-----\n\n\n[/]");
            int selectedOptionIndex = Array.IndexOf(Program.Options, AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("")
                .PageSize(15)
                .AddChoices(Program.Options)
            ));



            switch (selectedOptionIndex) {
                case 0: Program.AddLauncherLogin(); break;

                case 1: Program.DeleteLauncherLogin(); break;

                case 2: Program.GetLauncherLoginByName();  break; 
                
                case 3: Program.GetAllLaunchers();  break;

                case 4: return;
            }
        }
    }
}