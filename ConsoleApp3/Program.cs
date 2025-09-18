
using System;

namespace VirtualPetApp
{
    class Pet
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        // Scales are 1..10 inclusive
        // Hunger: 1 = full, 10 = starving
        public int Hunger { get; private set; }
        // Happiness: 1 = very sad, 10 = ecstatic
        public int Happiness { get; private set; }
        // Health: 1 = very poor, 10 = excellent
        public int Health { get; private set; }

        private Random rand = new Random();

        public Pet(string type, string name)
        {
            Type = Capitalize(type);
            Name = Capitalize(name);
            // Starting balanced stats
            Hunger = 5;
            Happiness = 6;
            Health = 8;

            Console.WriteLine();
            Console.WriteLine($"Welcome {Name} the {Type}! Your new pet is ready.");
            Console.WriteLine();
        }

        // Feed action: decreases hunger (better), slightly increases health
        public void Feed()
        {
            Console.WriteLine($"\nYou feed {Name}.");
            if (Hunger <= 1)
            {
                Console.WriteLine($"{Name} is already full and eats only a little.");
                Health = Clamp(Health + 0); // no change
            }
            else
            {
                Hunger = Clamp(Hunger - 3); // feeding reduces hunger
                Health = Clamp(Health + 1);
                Console.WriteLine($"{Name} seems satisfied.");
            }

            HourPasses(); // 1 hour passes
        }

        // Play action: increases happiness, slightly increases hunger
        public void Play()
        {
            Console.WriteLine($"\nYou try to play with {Name}.");

            // Pet refuses to play if very hungry (example of enhanced interaction logic)
            if (Hunger >= 8)
            {
                Console.WriteLine($"{Name} is too hungry to play and wants food first.");
                // Still consumes time (hour passes) but no happiness gain
                HourPasses();
                return;
            }

            // If health is poor, playing may be reduced
            if (Health <= 2)
            {
                Console.WriteLine($"{Name} is too weak to play. Rest first.");
                HourPasses();
                return;
            }

            Happiness = Clamp(Happiness + 2);
            Hunger = Clamp(Hunger + 1); // playing makes pet hungrier
            Console.WriteLine($"{Name} had fun! Happiness increased.");
            HourPasses();
        }

        // Rest action: improves health, slightly decreases happiness
        public void Rest()
        {
            Console.WriteLine($"\n{Name} rests for a while.");
            Health = Clamp(Health + 2);
            // Over-rest may make pet bored (slight happiness decrease)
            Happiness = Clamp(Happiness - 1);
            HourPasses();
        }

        // Called after every action to simulate time & consequences
        private void HourPasses()
        {
            Console.WriteLine("(One hour has passed.)");

            // Passive time effects
            Hunger = Clamp(Hunger + 1);          // gets hungrier over time
            Happiness = Clamp(Happiness - 1);    // happiness slowly drops over time

            // Consequences for neglect
            if (Hunger >= 9)
            {
                Console.WriteLine($"Warning: {Name} is starving. Health will drop if you don't feed soon.");
                Health = Clamp(Health - 2);
            }

            if (Happiness <= 2)
            {
                Console.WriteLine($"Warning: {Name} is very unhappy. Health suffers from long-term sadness.");
                Health = Clamp(Health - 1);
            }

            // Small chance of a positive random event (special messages)
            int chance = rand.Next(1, 21); // 1..20
            if (chance == 1)
            {
                Happiness = Clamp(Happiness + 2);
                Console.WriteLine($"{Name} found a hidden treat and got happier!");
            }

            // Show automatic status warning if extremes
            if (Hunger >= 8) Console.WriteLine($"CRITICAL: Hunger = {Hunger}/10. Feed {Name} soon!");
            if (Happiness <= 2) Console.WriteLine($"CRITICAL: Happiness = {Happiness}/10.");
            if (Health <= 3) Console.WriteLine($"CRITICAL: Health = {Health}/10.");

            Console.WriteLine();
        }

        // Prints the pet's current stats and warnings
        public void ShowStatus()
        {
            Console.WriteLine("----- Pet Status -----");
            Console.WriteLine($"Name     : {Name} the {Type}");
            Console.WriteLine($"Hunger   : {Hunger}/10  {(Hunger >= 8 ? "(High — critical!)" : "")}");
            Console.WriteLine($"Happiness: {Happiness}/10  {(Happiness <= 2 ? "(Low — needs attention!)" : "")}");
            Console.WriteLine($"Health   : {Health}/10  {(Health <= 3 ? "(Low — urgent!)" : "")}");
            Console.WriteLine("----------------------");
        }

        // Alive if health above 0
        public bool IsAlive()
        {
            return Health > 0;
        }

        // Simple utility to keep stats 1..10
        private int Clamp(int value)
        {
            if (value < 1) return 1;
            if (value > 10) return 10;
            return value;
        }

        private string Capitalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            s = s.Trim();
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Virtual Pet (Console) ===");
            Console.WriteLine("Stat scale: Hunger 1(full) -> 10(starving), Happiness 1 -> 10, Health 1 -> 10");
            Console.WriteLine();

            // Pet creation
            string[] validTypes = { "cat", "dog", "rabbit" };
            string type;
            while (true)
            {
                Console.Write("Choose pet type (cat/dog/rabbit): ");
                type = (Console.ReadLine() ?? "").Trim().ToLower();
                // if/else used to validate input
                if (Array.Exists(validTypes, t => t == type))
                    break;
                else
                    Console.WriteLine("Invalid type. Please enter cat, dog, or rabbit.");
            }

            Console.Write("Enter your pet's name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrEmpty(name)) name = "Buddy";

            Pet pet = new Pet(type, name);

            // Instructions
            Console.WriteLine("How to interact:");
            Console.WriteLine("1 - Feed\t2 - Play\t3 - Rest\t4 - Status\t5 - Quit");
            Console.WriteLine();

            // Main loop (while + if/else choices)
            while (pet.IsAlive())
            {
                Console.Write("Choose an action (1-5): ");
                string choice = (Console.ReadLine() ?? "").Trim();

                if (choice == "1" || choice.Equals("feed", StringComparison.OrdinalIgnoreCase))
                {
                    pet.Feed();
                }
                else if (choice == "2" || choice.Equals("play", StringComparison.OrdinalIgnoreCase))
                {
                    pet.Play();
                }
                else if (choice == "3" || choice.Equals("rest", StringComparison.OrdinalIgnoreCase))
                {
                    pet.Rest();
                }
                else if (choice == "4" || choice.Equals("status", StringComparison.OrdinalIgnoreCase))
                {
                    pet.ShowStatus();
                    // Viewing status should not advance time in this version; remove if you want it to pass time
                }
                else if (choice == "5" || choice.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Thanks for playing. Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Unrecognized option. Try again.");
                }

                // If health drops to 0 after consequences
                if (!pet.IsAlive())
                {
                    Console.WriteLine($"\nSadly, {name} has passed away due to poor health. Game over.");
                    break;
                }
            }

            Console.WriteLine("\n(Program finished.) Press Enter to exit.");
            Console.ReadLine();
        }
    }
}