using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

// Samuel Lööf & Simon Sörqvist, uppgift 4

/* 
 * hej :)
 */

namespace Vaccination
{
    public class Person
    {
        public int Age = 0;
        public bool WorksInHealthcare = false;
        public bool IsInRiskGroup = false;
        public bool HasHadInfection = false;
        public bool HasHadFirstDose = false;
    }
    public class Program
    {
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            int vaccineDosages = 0; 
            bool vaccinateChildren = false;

            string inputCSVFilepath = string.Empty;
            string outputCSVFilepath = string.Empty;

            while (true)
            {
                Console.WriteLine("Huvudmeny");
                Console.WriteLine("----------");
                Console.WriteLine($"Antal tillängliga vaccindoser {vaccineDosages}");

                string ageRestriction = vaccinateChildren ? "ja" : "nej";
                Console.WriteLine($"Vaccinering under 18 år: {ageRestriction}");
                Console.WriteLine($"Indatafil: {inputCSVFilepath}");
                Console.WriteLine($"Utdatafil: {outputCSVFilepath}");
                Console.WriteLine();

                int mainMenu = ShowMenu("Vad vill du göra?", new[]
                {
                    "Skapa prioritetsordning ",
                    "Schemalägg vaccinationer", // <-- fr. VG-delen 
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta"
                });
                Console.Clear();

                if (mainMenu == 0)
                {
                    // Prioritesordning
                }
                else if (mainMenu == 1)
                {
                    // Schemalägg vaccinationer
                    // schemalägg är fr. VG-delen 
                }

                else if (mainMenu == 2)
                {
                    vaccineDosages = ChangeVaccineDosages();
                    Console.Clear();
                }
                else if (mainMenu == 3)
                {
                    vaccinateChildren = ChangeAgeRequirement();
                    Console.Clear();
                }
                else if (mainMenu == 4)
                {
                    ChangeFilePath(isOutputFilePath: false);
                }
                else if (mainMenu == 5)
                {
                    ChangeFilePath(isOutputFilePath: true);
                }
                else 
                {
                    Console.Clear();
                    Console.WriteLine("Exiting program. Goodbye!");
                    Console.WriteLine();
                    break; // breaks main-loop 
                }
            } // <-- end of Main-loop 
        } // <-- end of Main() 

        public static int ChangeVaccineDosages()
        {
            while (true)
            {
                Console.WriteLine("Ändra antal vaccindoser");
                Console.WriteLine("-----------------");
                Console.Write("Ange nytt antal doser: ");

                try
                {
                    int newVaccineDosages = int.Parse(Console.ReadLine());
                    Console.WriteLine($"Nytt antal vaccindoser: {newVaccineDosages}");
                    return newVaccineDosages; // Return the new value of vaccine dosages, changed by the user.
                }
                catch (FormatException)
                {
                    Console.Clear();    
                    Console.WriteLine("Vänligen ange vaccindoseringarna i heltal.");
                    Console.WriteLine();
                }            
            }
        }

        public static bool ChangeAgeRequirement()
        {
            int ageMenu = ShowMenu("Ska personer under 18 vaccineras?", new[]
            {
                "Ja",
                "Nej"
            });

            //Returns the new updated vaccination age. 
            if (ageMenu == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // ChangeFilePath lets the user enter a filepath and makes sure it is valid
        public static string ChangeFilePath(bool isOutputFilePath)
        {
            while (true)
            {
                Console.Write("Ny filsökväg: ");
                string newPath = Console.ReadLine().Trim();

                char[] invalidPathChars = Path.GetInvalidPathChars();
                bool containsInvalidChars = false;
                foreach (char c in invalidPathChars)
                {
                    if (newPath.Contains(c))
                    {
                        Console.Clear();
                        Console.WriteLine("Filsökvägen innehåller ogiltiga tecken, försök igen.");
                        Console.WriteLine("(Exempel på ogiltiga tecken: " +
                            string.Join(' ', invalidPathChars) + ")");
                        Console.WriteLine();
                        break; // breaks foreach loop 
                    }
                }
                if (containsInvalidChars) { continue; } // starts new iteration of this methods main-loop

                // might need to change this if we enter a fully qualified filepath (with an actual FILE-name)
                // might throw error if a directory is not the final target for path 
                if (Directory.Exists(newPath))
                {
                    if (isOutputFilePath) { return newPath; }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Mappen finns inte, ange en giltig filsökväg.");
                    Console.WriteLine();
                    continue; // starts new iteration for this methods main-loop 
                }

                // guard clause for inputFilepath handling, maybe not needed 
                if (!isOutputFilePath)
                {
                    if (File.Exists(newPath)) { return newPath; }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Filen finns inte, ange en giltig filsökväg.");
                        Console.WriteLine();
                        // continue not needed, this is the last if-statement 
                    }
                }
            }
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // Replace with your own code.
            return new string[0];
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }

    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void exTest()
        {

        }
    }

    // Jakobs tests vv
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void BaseFunctionalityTest()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }
    }
    // Jakobs tests ^^^
}
