using System.Net.Http.Headers;

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

            int period = periodIndex switch {
                0 => 5,
                1 => 10,
                2 => 15,
                _ => 1
            };

            double rate = GetRate();
            double monthlyRepayment = deposit * (rate / 100 / 12) * Math.Pow(1 + (rate / 100 / 12), period) / (Math.Pow(1 + (rate / 100 / 12), period) - 1);
            double totalPayment = (monthlyRepayment * period) + deposit;
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