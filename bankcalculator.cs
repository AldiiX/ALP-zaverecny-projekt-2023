class BankCalculator {

    private static class Program {

        /// <returns>Vklad</returns>
        private static double GetDeposit() {
            Console.Clear();
            double deposit = AnsiConsole.Ask<double>("Zadej počáteční vklad: ");
            return deposit;
        }

        /// <returns>Úroková sazba</returns>
        private static double GetRate() {
            /*Console.Clear();
            double rate = AnsiConsole.Ask<double>("Zadej úrokovou sazbu: ");
            return rate;*/
            return 3.5;
        }

        public static void RunRepayments() {

            double deposit = GetDeposit();
            string[] periodStrings = new string[] { "5 let", "10 let", "15 let" };

            int periodIndex = Array.IndexOf(periodStrings, AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Vyber úrokové období")
                .AddChoices(periodStrings)
            ));

            int periodInMonths = periodIndex switch {
                0 => 5 * 12,
                1 => 10 * 12,
                2 => 15 * 12,
                _ => 1 * 12
            };

            double monthlyRate = (GetRate() / 100) / 12;
            double monthlyRepayment = Math.Round((deposit * monthlyRate) / (1 - Math.Pow(1 + monthlyRate, -periodInMonths)), 2);
            double totalPayment = Math.Round(monthlyRepayment * periodInMonths, 2);
            Console.WriteLine(monthlyRepayment);
            Console.WriteLine(totalPayment);
            Console.ReadKey();
        }

        public static void RunSavings() {

        }
    }

    public static void Main() {
        while (true) {
            
            Console.Clear();

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