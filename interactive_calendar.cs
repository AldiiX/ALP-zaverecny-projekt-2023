using JK;

class InteractiveCalendar {

    private class Event {

        public Event(byte date, byte month, short year, string name) {
            Day = date;
            Month = month;
            Year = year;
            Name = name;
        }

        public string Name { get; set; }
        public byte Day { get; set; }
        public byte Month { get; set; }
        public short Year { get; set; }
    }

    private static class Calendar {

        public static string[] Actions { get; } = { "Kaledář aktuálního měsíce", "Zobrazit události", "Přidat událost", "Smazat událost", "[red]KONEC[/]" };
        private static List<Event> Events { get; } = new();

        public static void AddEvent() {
            Jachym.Klir();

            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Zadej [blue]jméno[/] přidávané události: ")
                .Validate(input => {

                    if (input.Length > 30) return ValidationResult.Error($"[red][invert]\nJméno musí mít maximálně 30 znaků.\n[/][/]");

                    return ValidationResult.Success();
                })
                .ValidationErrorMessage($"[red][invert]\nZadej prosím jméno události.\n[/][/]")
            );

            short year = AnsiConsole.Prompt(
                new TextPrompt<short>("Zadej [blue]rok[/] přidávané události: ")
                .Validate(input => {

                    if (input < 1950) return ValidationResult.Error($"[red][invert]\nRok nemůže být menší než 1950.\n[/][/]");
                    if (input > 2150) return ValidationResult.Error($"[red][invert]\nRok nemůže být větší než 2150.\n[/][/]");

                    return ValidationResult.Success();
                })
                .ValidationErrorMessage($"[red][invert]\nZadej prosím číslo.\n[/][/]")
            );

            byte month = AnsiConsole.Prompt(
                new TextPrompt<byte>("Zadej [blue]měsíc[/] přidávané události: ")
                .Validate(input => {

                    if (input < 1 || input > 12) return ValidationResult.Error("[red][invert]\nZadej správné číslo měsíce (1-12).\n[/][/]");

                    return ValidationResult.Success();
                })
                .ValidationErrorMessage($"[red][invert]\nZadej prosím číslo.\n[/][/]")
            );

            byte day = AnsiConsole.Prompt(
                new TextPrompt<byte>("Zadej [blue]den[/] přidávané události: ")
                .Validate(input => {

                    int numberOfDays = DateTime.DaysInMonth(year, month);

                    if (input < 1 || input > numberOfDays) return ValidationResult.Error($"[red][invert]\nZadej správný číslo dne (1-{numberOfDays}).\n[/][/]");

                    return ValidationResult.Success();
                })
                .ValidationErrorMessage($"[red][invert]\nZadej prosím číslo.\n[/][/]")
            );

            Events.Add(new Event(day,month,year,name));
        }
        
        public static void RemoveEvent() {

            Jachym.Klir();
            Console.WriteLine();
            
            var eventsList = new List<string>();
            foreach (Event e in Events) eventsList.Add($"{e.Name} ({e.Day}.{e.Month}.{e.Year})");

            if (Events.Count == 0) {

                Jachym.Klir();
                AnsiConsole.MarkupLine($"[red][invert]\nNebyla nalezena žádná událost!\n[/][/]");
                Console.ReadKey();

                return;
            }

            var selectedEventIndex = eventsList.IndexOf(AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Vyber událost ke [underline]smazání[/]:")
                    .AddChoices(eventsList)
                    .AddChoices("[red]ZPĚT[/]")
            ));

            if (selectedEventIndex < 0) return;

            Events.RemoveAt(selectedEventIndex);
        }

        public static void ShowEvents() { 

            var eventsList = new List<string>();
            foreach (Event e in Events) eventsList.Add($"{e.Name} ({e.Day}.{e.Month}.{e.Year})");



            if(Events.Count == 0) {

                Jachym.Klir();
                AnsiConsole.MarkupLine($"[red][invert]\nNebyla nalezena žádná událost!\n[/][/]");
                Console.ReadKey();
                
                return;
            }



            while(true) {

                Jachym.Klir();

                var selectedEventIndex = eventsList.IndexOf(AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("[white]\nUložené události:[/]")
                    .AddChoices(eventsList)
                    .AddChoices("[red]ZPĚT[/]")
                ));

                if (selectedEventIndex < 0) break;

                Event ev = Events[selectedEventIndex];
                ShowCalendar(ev.Month, ev.Year);
            }
        }

        public static void ShowCalendar(byte month, short year) {
            Jachym.Klir();



            var calendar = new Spectre.Console.Calendar(year, month)
            .Culture("cs-CS")
            .MinimalHeavyHeadBorder()
            .BorderColor(Color.Grey35)
            .HeaderStyle(Style.Parse("blue bold"));


            if(Events.Count > 0) {
                foreach(var e in Events) calendar.AddCalendarEvent(e.Year, e.Month, e.Day);
                calendar.HighlightStyle(Style.Parse("yellow1 bold"));
            }



            AnsiConsole.Write(calendar);
            Console.ReadKey();
        }
    }

    public static void Main() {

        bool looping = true;

        var image = new CanvasImage("src/calendar.png");
        image.NearestNeighborResampler();
        AnsiConsole.Write(image);
        Console.WriteLine();

        var rule = new Rule("[white]Vítej v aplikaci [bold]INTERAKTIVNÍ KALENDÁŘ[/][/]").LeftJustified();
        rule.Style = Style.Parse("red dim");
        AnsiConsole.Write(rule);
        Console.ReadKey();

        while (looping) {

            Jachym.Klir();

            AnsiConsole.Write(new FigletText($"MENU").LeftJustified().Color(Color.Blue));
            Console.WriteLine();

            sbyte actionIndex = (sbyte)Array.IndexOf(Calendar.Actions, AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(Calendar.Actions)
            ));


            switch (actionIndex) {
                case 0: Calendar.ShowCalendar((byte)DateTime.Now.Month, (short)DateTime.Now.Year); break;
                case 1: Calendar.ShowEvents(); break;
                case 2: Calendar.AddEvent(); break;
                case 3: Calendar.RemoveEvent(); break;
                case 4: looping = false; break;
            }
        }
    }
}