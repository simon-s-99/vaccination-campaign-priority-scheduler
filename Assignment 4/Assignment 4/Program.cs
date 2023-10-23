using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

// Samuel Lööf & Simon Sörqvist, uppgift 4
/*
 * Tjo!
 * 
 * Skriv 5 - 10 tester som testar en metod.
 * Dokumentation.
 * Omorganisera metoderna i rätt ordning.
 * 
 */


namespace Vaccination
{
    public class Person
    {
        public DateTime DateOfBirth { get; private set; } 
        private string idNumber;
        public string IDNumber
        {
            get { return idNumber; }
            set
            {

                // Remove any dashes or plus-characters (apparantly valid id-numbers can have this) 
                string idNr = value.Replace("-", "").Replace("+", "").Trim();
                

                int year, month, day;

                if (idNr.Length == 10)
                {
                    idNr = "19" + idNr;
                }

                if (idNr.Length == 12)
                {
                    year = int.Parse(idNr.Substring(0, 4));
                    month = int.Parse(idNr.Substring(4, 2));
                    day = int.Parse(idNr.Substring(6, 2));
                }
                else
                {
                    throw new ArgumentException("Identification number format is invalid.");
                }

                DateOfBirth = new DateTime(year, month, day);
                idNumber = idNr.Substring(0, 8) + "-" + idNr.Substring(8);
            }
        }
        public string LastName { get; private set; } 
        public string FirstName { get; private set; }
        public int WorksInHealthcare { get; private set; }
        public int IsInRiskGroup { get; private set; }
        public int HasHadInfection { get; private set; }

        public Person(string idNr, string lastName, string firstName, 
            int worksInHealthCare, int isInRiskGroup, int hasHadInfection) 
        {
            IDNumber = idNr;
            LastName = lastName;
            FirstName = firstName;

            if (worksInHealthCare == 1) { WorksInHealthcare = worksInHealthCare; }
            else if (worksInHealthCare == 0) { WorksInHealthcare = worksInHealthCare; }
            else
            {
                throw new ArgumentException("Value is not in accepted range.");
            }

            if (isInRiskGroup == 1) { IsInRiskGroup = isInRiskGroup; }
            else if (isInRiskGroup == 0) { IsInRiskGroup = isInRiskGroup; }
            else
            {
                throw new ArgumentException("Value is not in accepted range.");
            }

            if (hasHadInfection == 1) { HasHadInfection = hasHadInfection; }
            else if (hasHadInfection == 0) { HasHadInfection = hasHadInfection; }
            else
            {
                throw new ArgumentException("Value is not in accepted range.");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            int doses = 0;
            bool vaccinateChildren = false;

            string inputCSVFilepath = string.Empty;
            string outputCSVFilepath = string.Empty;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Huvudmeny");
                Console.WriteLine("----------");
                Console.WriteLine($"Antal tillängliga vaccindoser {doses}");

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
                    string[] inputCSV = File.ReadAllLines(inputCSVFilepath);

                    string[] priorityOrder = CreateVaccinationOrder(inputCSV, doses, vaccinateChildren);

                    PriorityOrderToCSV(priorityOrder, outputCSVFilepath);
                }
                else if (mainMenu == 1)
                {
                    // Schemalägg vaccinationer
                    // schemalägg är fr. VG-delen 
                }

                else if (mainMenu == 2)
                {
                    doses = ChangeVaccineDosages();
                }
                else if (mainMenu == 3)
                {
                    vaccinateChildren = ChangeAgeRequirement();
                }
                else if (mainMenu == 4)
                {
                    inputCSVFilepath = ChangeFilePath(isOutputPath: false);
                }
                else if (mainMenu == 5)
                {
                    outputCSVFilepath = ChangeFilePath(isOutputPath: true);
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
        public static string ChangeFilePath(bool isOutputPath)
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

                Console.WriteLine("(Ex.: C:\\Windows\\Temp\\exempel.txt )");
                Console.WriteLine("---------------");
                Console.Write("Ny filsökväg: ");
                string newPath = Console.ReadLine().Trim();

                if (Path.IsPathFullyQualified(newPath))
                {
                    // get file-extension if there is one
                    string fileName = Path.GetFileName(newPath);
                    string fileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1);

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

                // tell user to try again
                Console.Clear();
                Console.WriteLine("Sökvägen du angett är ogiltig, ange en giltig filsökväg.");
                Console.WriteLine("Tänk på att välja rätt fil-ändelse (.csv/.CSV)");
                Console.WriteLine();
            }
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // this is the list the method will later return as a string[] 
            List<Person> sortedPeople = new List<Person>();

            // list where we will store the input from the CSV file
            List<Person> people = new List<Person>();

            // Read and parse the CSV data from the input array
            foreach (string line in input)
            {
                string[] values = line.Replace(" ", "").Split(',');

                if (values.Length == 6) // Make sure there are at least 6 values in the array.
                {
                    string idNumber = values[0];
                    string lastName = values[1];
                    string firstName = values[2];
                    int worksInHealthcare = int.Parse(values[3]);
                    int isInRiskGroup = int.Parse(values[4]);
                    int hasHadInfection = int.Parse(values[5]);

                    // Create a Person object
                    Person person = new Person(
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
                else
                {
                    // add handling for incorrect amount of fields 
                    //      (should write to console) 
                }
            }

            // Sort the people based on the vaccination priority criteria
            // Priority order for vaccination:
            // 1. If the person works in healthcare
            sortedPeople.AddRange(people.Where(p => p.WorksInHealthcare == 1));
            people = people.Where(p => p.WorksInHealthcare == 0).ToList();

            // 2.people aged 65 and older
            sortedPeople.AddRange(people.Where(p =>
                p.DateOfBirth.AddYears(65) <= DateTime.Now));
            people = people.Where(p => p.DateOfBirth.AddYears(65) > DateTime.Now).ToList();

            // 3. If the person is in a risk group.
            sortedPeople.AddRange(people.Where(p => p.IsInRiskGroup == 1));
            people = people.Where(p => p.IsInRiskGroup == 0).ToList();

            // 4. Then by age in order (oldest to youngest).
            sortedPeople.AddRange(people.OrderBy(p => p.DateOfBirth));


            // Return-list
            var output = new List<string>();
            foreach (Person person in sortedPeople)

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

        public static void PriorityOrderToCSV(string[] priorityOrder, string filePath)
        {
            if (File.Exists(filePath))
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
                    Console.WriteLine();
                    return;
                }
            }
            
            File.WriteAllLines(filePath, priorityOrder);
            Console.WriteLine("Prioritetsordningen har sparats.");
            Console.WriteLine();
        }

        public static int VaccinationSchedule()
        {
            /*The first vaccination should take place on a date selected by the user.
             * Two people can be vaccinated at the same time.
             * Every vaccination takes 5 minutes.
             * Vaccination should be done cotiniously in the same speed from 8:00 to 20:00, every day of the week.
             * The schedule should only contain the first dose for every person.
             * The schedule should be saved in a .Ics file.
             * 
             * The user should be able to decide/change the follwing :
             * Which date the vaccination should start (standard value: one week after current dateTime.Now
             * Start time for vaccination (8:00 standard value.)
             * End time for vaccination (20:00 standard value.)
             * How many people that can be vaccinated at the same time (standard value :2)
             * How long a vaccination should take (standard value: 5 minutes)
             * Where the file should be saved (Standard value: C:\Windows\Temp\Schedule.ics)
             */
            return 1;
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
        public void BaseFunctionalityTest() // Jakobs test 
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
}

