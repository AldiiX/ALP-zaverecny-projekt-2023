using JK;

class BankCalculator {

    private static class Utils {

        public static double RateInPercents { get; private set; }

        public class Period {
            public static string[] PeriodStrings { get; } = new string[] { "5 let", "10 let", "15 let" };
            public int InYears { get; set; }
            public int InMonths { get; set; }
        }

        /// <returns>Vklad</returns>
        public static double GetDeposit() {
            Jáchym.Klír();
            double deposit = AnsiConsole.Prompt(
                new TextPrompt<double>("Zadej [blue]počáteční vklad[/]: ")
                .Validate(input => {
                    if (input >= 15000000 || input < 1) return ValidationResult.Error("\n[red][invert]Zadej číslo od 1 do 15M.[/][/]\n");
                    return ValidationResult.Success();
                })
                .ValidationErrorMessage("\n[red][invert]Špatný input. Zadej číslo.[/][/]\n")
            );
            return Math.Round(deposit, 2);
        }

        /// <returns>Úroková sazba</returns>
        public static double GetRate() {
            double rate = AnsiConsole.Prompt(
                new TextPrompt<double>("Zadej [blue]úrokovou sazbu[/]:")
                    .Validate(input => {
                        if (input > 10)
                        {
                            return ValidationResult.Error("[red][invert]\nÚroková sazba musí být maximálně 10%.\n[/][/]");
                        }
                        if (input < 1)
                        {
                            return ValidationResult.Error("[red][invert]\nÚroková sazba musí být minimálně 1%.\n[/][/]");
                        }
                        return ValidationResult.Success();
                    })
                    .ValidationErrorMessage("\n[red][invert]Špatný input. Zadej číslo od 1 do 10.[/][/]\n")
            );

            double r = Math.Round(rate, 2);
            RateInPercents = r;
            return r;
        }


        public static class GetPeriod {
            /// <returns>Úrokové období</returns>
            public static Period Repayments() {

                var period = new Period();

                var index = Array.IndexOf(Period.PeriodStrings, AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Vyber úrokové období")
                    .AddChoices(Period.PeriodStrings)
                ));

                period.InYears = index switch {
                    0 => 5,
                    1 => 10,
                    2 => 15,
                    _ => 1
                };

                period.InMonths = period.InYears * 12;

                return period;
            }

            /// <returns>Úrokové období</returns>
            public static Period Savings() {

                var period = new Period();

                period.InYears = AnsiConsole.Prompt(
                    new TextPrompt<int>("Zadejte [blue]délku spoření[/] v [underline]letech[/]: ")
                    .Validate(input => {
                        if (input < 1 || input > 20) return ValidationResult.Error("[red][invert]\nZadej počet let (od 1 do 20)\n[/][/]");

                        return ValidationResult.Success();
                    })
                    .ValidationErrorMessage("\n[red][invert]Špatný input. Zadej celé číslo.[/][/]\n")
                );

                period.InMonths = period.InYears * 12;

                return period;
            }
            
        }
    }



    private static void RunRepayments() {

        double deposit = Utils.GetDeposit();



        Console.WriteLine();
        var period = Utils.GetPeriod.Repayments();



        double monthlyRate = (Utils.GetRate() / 100) / 12;
        double monthlyRepayment = Math.Round((deposit * monthlyRate) / (1 - Math.Pow(1 + monthlyRate, -period.InMonths)), 2);
        double totalPayment = Math.Round(monthlyRepayment * period.InMonths, 2);
        double totalRepayment = Math.Round(totalPayment - deposit, 2);



        var table = new Table();
        table.AddColumns("[grey35]Název[/]", "[grey35]Hodnota[/]");
        table.AddRow("[blue]Jistina[/]", $"{deposit} Kč");
        table.AddRow("[blue]Úrokové období[/]", $"{period.InYears} let");
        table.AddRow("[blue]Úroková sazba[/]", $"{Utils.RateInPercents} %");
        table.AddRow("[yellow1]Měsíční úrok[/]".ToUpper(), $"[yellow]{monthlyRepayment} Kč[/]");
        table.AddRow("[yellow1]Celkový úrok[/]".ToUpper(), $"[yellow]{totalRepayment} Kč[/]");
        table.AddRow("[yellow1]Celková platba[/]".ToUpper(), $"[yellow]{totalPayment} Kč[/]");

        Jáchym.Klír();
        Console.WriteLine("\n");
        AnsiConsole.Write(table);
        Console.ReadKey();
    }

    private static void RunSavings() {
        double deposit = Utils.GetDeposit();

        double monthlyDeposit = AnsiConsole.Prompt(
            new TextPrompt<double>("Zadej [blue]měsíční vklad[/]:")
                .Validate(input => {
                    if (input < 0) {
                        return ValidationResult.Error("[red][invert]\nVklad nesmí být záporné číslo!\n[/][/]");
                    }
                    return ValidationResult.Success();
                })
                .ValidationErrorMessage("\n[red][invert]Špatný input. Zadej číslo.[/][/]\n")
        );

        double rate = Utils.GetRate() / 100;
        var period = Utils.GetPeriod.Savings();

        double finalAmount = deposit;
        for (int i = 0; i < period.InYears; i++) { // Tento vzorec postupně přičítá roční vklady a úročí kapitál na konci každého roku. Výsledkem je finální uspořená částka po zadaném počtu let.
            finalAmount += monthlyDeposit * 12; // Přičtení ročního vkladu
            finalAmount *= 1 + rate; // Úročení pro daný rok
        }

        finalAmount = Math.Round(finalAmount, 2);
        double totalRepayment = Math.Round(finalAmount - deposit, 2);


        var table = new Table();
        table.AddColumns("[grey35]Název[/]", "[grey35]Hodnota[/]");
        table.AddRow("[blue]Jistina[/]", $"{deposit} Kč");
        table.AddRow("[blue]Délka spoření[/]", $"{period.InYears} let");
        table.AddRow("[blue]Úroková sazba[/]", $"{Utils.RateInPercents} %");
        table.AddRow("[blue]Měsíční vklad[/]", $"{monthlyDeposit} Kč");
        table.AddRow("[yellow1]Celkový úrok[/]".ToUpper(), $"[yellow]{totalRepayment} Kč[/]");
        table.AddRow("[yellow1]Celková platba[/]".ToUpper(), $"[yellow]{finalAmount} Kč[/]");

        Jáchym.Klír();
        Console.WriteLine("\n");
        AnsiConsole.Write(table);
        Console.ReadKey();
    }



    public static void Main() {
        while (true) {
            
            Jáchym.Klír();

            AnsiConsole.Write(new FigletText($"ČSOB").LeftJustified().Color(Color.Blue));
            AnsiConsole.Write("\nVítej v bankovní kalkulačce!\n\n\n");
            


            string type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Vyber si, co budeš počítat.")
                .AddChoices(new string[] {"Splátky", "Spoření"})
            );

            switch(type) {
                case "Splátky": RunRepayments(); break;
                case "Spoření": RunSavings(); break;
            }
        }
    }
}