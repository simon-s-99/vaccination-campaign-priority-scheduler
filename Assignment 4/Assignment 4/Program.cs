using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

// Samuel Lööf & Simon Sörqvist, uppgift 4

namespace Vaccination
{
    public class Program
    {
        // global/static so that these can be reached from PriorityOrderToICSRawText() in Schedule.cs 
        public static int Doses = 0;
        public static bool VaccinateChildren = false;
        public static string InputCSVFilepath = string.Empty;

        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string outputCSVFilepath = string.Empty;
            var schedule = new Schedule.Info(); // holds all scheduling info 

            while (true)
            {
                Console.WriteLine("Huvudmeny");
                Console.WriteLine("----------");
                Console.WriteLine($"Antal tillängliga vaccindoser: {Doses}");

                string ageRestriction = VaccinateChildren ? "ja" : "nej";
                Console.WriteLine($"Vaccinering under 18 år: {ageRestriction}");
                Console.WriteLine($"Indatafil: {InputCSVFilepath}");
                Console.WriteLine($"Utdatafil: {outputCSVFilepath}");
                Console.WriteLine();

                int mainMenu = ShowMenu("Vad vill du göra?", new[]
                {
                    "Skapa prioritetsordning",
                    "Schemalägg vaccinationer",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta"
                });
                Console.Clear();

                if (mainMenu == 0) // create priority order 
                {
                    if (InputCSVFilepath != string.Empty &&
                        outputCSVFilepath != string.Empty &&
                        Doses >= 1)
                    {
                        string[] inputCSV = File.ReadAllLines(InputCSVFilepath);

                        string[] priorityOrder = CreateVaccinationOrder(inputCSV, Doses, VaccinateChildren);

                        PriorityOrderToCSV(priorityOrder, outputCSVFilepath);
                    }

                    if (InputCSVFilepath == string.Empty)
                    {
                        Console.WriteLine("Välj indatafil först.");
                    }

                    if (outputCSVFilepath == string.Empty)
                    {
                        Console.WriteLine("Välj utdatafil först.");
                    }

                    if (Doses < 1)
                    {
                        Console.WriteLine("Antalet tillgängliga doser måste vara 1 eller mer.");
                    }

                    Console.WriteLine();
                }
                else if (mainMenu == 1) // schedule vaccinations 
                {
                    schedule = Schedule.SubMenu.ScheduleMenu(schedule);
                }
                else if (mainMenu == 2) // change nr. of available doses 
                {
                    Doses = ChangeVaccineDosages();
                }
                else if (mainMenu == 3) // change age / vaccinate children? yes/no 
                {
                    VaccinateChildren = ChangeAgeRequirement();
                }
                else if (mainMenu == 4) // change input filepath
                {
                    InputCSVFilepath = ChangeFilePathCSV(isOutputPath: false);
                }
                else if (mainMenu == 5) // change output filepath 
                {
                    outputCSVFilepath = ChangeFilePathCSV(isOutputPath: true);
                }
                else // exit program 
                {
                    Console.Clear();
                    Console.WriteLine("Exiting program. Goodbye!");
                    Console.WriteLine();
                    break; // breaks main-loop 
                }
            } // <-- end of Main-loop 
        } // <-- end of Main() 

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // this is the list the method will later return as a string[] 
            var sortedPeople = new List<Patient.Person>();

            // list where we will store the input from the CSV file
            var people = new List<Patient.Person>();

            // set this to true if any line in the input CSV file (array in this case)
            // is incorrectly formatted
            bool anyLineIncorrect = false;

            // Read and parse the CSV data from the input array
            foreach (string line in input)
            {
                string[] values = line.Replace(" ", "").Split(',');

                bool incorrectFormat = false;

                if (values.Length == 6) // Make sure there are at least 6 values in the array.
                {
                    string idNumber = values[0];
                    string lastName = values[1];
                    string firstName = values[2];
                    int worksInHealthcare = int.Parse(values[3]);
                    int isInRiskGroup = int.Parse(values[4]);
                    int hasHadInfection = int.Parse(values[5]);

                    // try around the everything in this loop that uses person
                    // to avoid person being used out of scope 
                    try
                    {
                        // Create a Person object
                        var person = new Patient.Person(
                            idNumber,
                            lastName,
                            firstName,
                            worksInHealthcare,
                            isInRiskGroup,
                            hasHadInfection
                        );

                        // Store the person in the list
                        if (vaccinateChildren)
                        {
                            people.Add(person);
                        }
                        else
                        {
                            if (person.DateOfBirth.AddYears(18) <= DateTime.Now)
                            {
                                people.Add(person);
                            }
                        }
                    }
                    catch
                    {
                        incorrectFormat = true;
                    }
                }
                if (values.Length != 6 || incorrectFormat)
                {
                    Console.WriteLine("Inkorrekt format på en rad i input CSV-filen, ingen " +
                        "output kommer att skrivas.");
                    anyLineIncorrect = true;
                }
            }

            if (anyLineIncorrect)
            {
                return new string[0]; // returns an empty string-array 
            }
            // (else) if no incorrect lines are found continue below 

            // Sort the people based on the vaccination priority criteria
            // Priority order for vaccination:
            // 1. If the person works in healthcare
            sortedPeople.AddRange(people.Where(p => p.WorksInHealthcare == 1).
                OrderBy(p => p.DateOfBirth));
            people = people.Where(p => p.WorksInHealthcare == 0).ToList();

            // 2.people aged 65 and older
            sortedPeople.AddRange(people.Where(p =>
                p.DateOfBirth.AddYears(65) <= DateTime.Now).OrderBy(p => p.DateOfBirth));
            people = people.Where(p => p.DateOfBirth.AddYears(65) > DateTime.Now).ToList();

            // 3. If the person is in a risk group.
            sortedPeople.AddRange(people.Where(p => p.IsInRiskGroup == 1).
                OrderBy(p => p.DateOfBirth));
            people = people.Where(p => p.IsInRiskGroup == 0).ToList();

            // 4. Then by age in order (oldest to youngest).
            sortedPeople.AddRange(people.OrderBy(p => p.DateOfBirth));


            // Return-list
            var output = new List<string>();
            foreach (Patient.Person person in sortedPeople)
            {
                int administeredDose = 2; // default state is 2 doses 
                if (person.HasHadInfection == 1) { administeredDose = 1; }


                if (doses >= 2 || (doses == 1 && person.HasHadInfection == 1))
                {
                    string line = $"{person.IDNumber},{person.LastName}," +
                        $"{person.FirstName},{administeredDose}";
                    output.Add(line);
                    doses -= administeredDose;
                }
                else
                {
                    break; // break loop when doses run out 
                }
            }

            return output.ToArray(); // return as array 
        }

        // outputs array to filepath, associated with CreateVaccinationOrder() in main menu-context 
        public static void PriorityOrderToCSV(string[] priorityOrder, string filepath)
        {
            if (priorityOrder.Length == 0)
            {
                Console.WriteLine("Ingen prioritetsordning har skapats, ingen output skrivs " +
                    "till utdatafilen.");
                Console.WriteLine();
                return;
            }
            else if (File.Exists(filepath))
            {
                int overwriteMenu = ShowMenu($"Filen existerar redan. Vill du skriva över den?", new[]
                {
                    "Ja",
                    "Nej"
                });

                Console.Clear();

                if (overwriteMenu == 1)
                {
                    Console.WriteLine("Filen har inte sparats.");
                    Console.WriteLine("Ändra utdatafil från huvudmenyn om du vill skapa en prioritetsordning");
                    return;
                }
            }

            File.WriteAllLines(filepath, priorityOrder);
            Console.WriteLine($"Prioritetsordningen har sparats i {filepath}");
        }

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
                    Console.Clear();

                    if (newVaccineDosages > 0) // guarantees at least one dose 
                    {
                        return newVaccineDosages;
                    }
                    else { throw new FormatException(); }
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Vänligen ange vaccindoseringar som ett positivt heltal.");
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

            Console.Clear();

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
        public static string ChangeFilePathCSV(bool isOutputPath)
        {
            while (true)
            {
                if (isOutputPath)
                {
                    Console.WriteLine("Ändra utdatafil.");
                }
                else
                {
                    Console.WriteLine("Ändra indatafil.");
                }

                Console.WriteLine("(Ex.: C:\\Windows\\Temp\\exempel.CSV)");
                Console.WriteLine("---------------");
                Console.Write("Ny filsökväg: ");
                string newPath = Console.ReadLine().Trim();

                Console.Clear();

                if (Path.IsPathFullyQualified(newPath))
                {
                    // get file-extension if there is one
                    string fileName = Path.GetFileName(newPath);
                    string fileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1);

                    // for comparison of the illegal characters in a filename \/:*?"<>|
                    // IndexOfAny returns -1 if none of the chars are found in the string 
                    string illegalCharacters = "\\/:*?\"<>|";
                    if (fileName.IndexOfAny(illegalCharacters.ToCharArray()) == -1)
                    {
                        if (isOutputPath) // output handling
                        {
                            string tempPath = newPath.Substring(0, newPath.LastIndexOf("\\"));
                            if (Directory.Exists(tempPath))
                            {
                                if (fileExtension == "csv" || fileExtension == "CSV")
                                {
                                    return newPath;
                                }
                            }
                        }
                        else // input handling
                        {
                            if (fileExtension == "csv" || fileExtension == "CSV")
                            {
                                if (File.Exists(newPath)) { return newPath; }
                            }
                        }
                    }
                }

                // tell user to try again
                Console.WriteLine("Sökvägen du angett är ogiltig, ange en giltig filsökväg.");
                Console.WriteLine("Tänk på att välja rätt fil-ändelse (.csv/.CSV)");
                Console.WriteLine("Filnamnet får inte innehålla något av följande tecken: \\/:*?\"<>|");
                Console.WriteLine();
            }
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
}
