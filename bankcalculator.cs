using JK;

class BankCalculator {

    private static class Program {

        public static double RateInPercents { get; private set; }



        /// <returns>Vklad</returns>
        private static double GetDeposit() {
            Jáchym.Klír();
            double deposit = AnsiConsole.Prompt<double>(
                new TextPrompt<double>("Zadej [blue]počáteční vklad[/]: ")
                .Validate(input => {
                    if (input > 15000000 || input < 1) return ValidationResult.Error("\n[red][invert]Zadej číslo od 1 do 15M.[/][/]\n");
                    return ValidationResult.Success();
                })
            );
            return deposit;
        }

        /// <returns>Úroková sazba</returns>
        private static double GetRate() {
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
            );

            double r = Math.Round(rate, 2);
            RateInPercents = r;
            return r;
        }

        public static void RunRepayments() {

            double deposit = GetDeposit();
            string[] periodStrings = new string[] { "5 let", "10 let", "15 let" };

            Console.WriteLine();
            int periodIndex = Array.IndexOf(periodStrings, AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Vyber úrokové období")
                .AddChoices(periodStrings)
            ));

            int periodInYears = periodIndex switch {
                0 => 5,
                1 => 10,
                2 => 15,
                _ => 1
            };

            int periodInMonths = periodInYears * 12;



            double monthlyRate = (GetRate() / 100) / 12;
            double monthlyRepayment = Math.Round((deposit * monthlyRate) / (1 - Math.Pow(1 + monthlyRate, -periodInMonths)), 2);
            double totalPayment = Math.Round(monthlyRepayment * periodInMonths, 2);



            var table = new Table();
            table.AddColumns("[grey35]Název[/]", "[grey35]Hodnota[/]");
            table.AddRow("[blue]Jistina[/]", deposit.ToString());
            table.AddRow("[blue]Úrokové období[/]", $"{periodInYears} let");
            table.AddRow("[blue]Úroková sazba[/]", $"{RateInPercents} %");
            table.AddRow("[yellow1]Měsíční úrok[/]".ToUpper(), $"[yellow]{monthlyRepayment} Kč[/]");
            table.AddRow("[yellow1]Celková platba[/]".ToUpper(), $"[yellow]{totalPayment} Kč[/]");

            Jáchym.Klír();
            Console.WriteLine("\n");
            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        public static void RunSavings() {

        }
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
                case "Splátky": Program.RunRepayments(); break;
                case "Spoření": Program.RunSavings(); break;
            }
        }
    }
}