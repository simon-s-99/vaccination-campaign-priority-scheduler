using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

// Samuel Lööf & Simon Sörqvist, uppgift 4

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

    public class Schedule
    {
        private DateTime _StartDate { get; set; }
        public DateTime StartDate 
        { 
            get { return _StartDate; }
            
            set
            {
                // updates startdate hours/mins/seconds when the value is changed 
                _StartDate = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                _StartDate.Add(StartTime);
            }
        }
        private TimeSpan _StartTime { get; set; }
        public TimeSpan StartTime
        {
            get { return _StartTime; }

            set
            {
                _StartTime = value;

                // update startdate with new hours/mins/seconds when starttime is changed 
                _StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                _StartDate.Add(value);
            }
        }
        public TimeSpan EndTime { get; set; }
        public TimeSpan VaccinationTime { get; set; }
        public int ConcurrentVaccinations { get; set; }
        public string FilePathICS { get; set; }

        public Schedule()
        {
            StartDate = DateTime.Today.AddDays(7);
            StartTime = new TimeSpan(8, 0, 0);
            EndTime = new TimeSpan(20, 0, 0);
            VaccinationTime = new TimeSpan(0, 5, 0);
            ConcurrentVaccinations = 2;
            FilePathICS = "C:\\Windows\\Temp\\Schedule.ics";
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

            Schedule schedule = new Schedule(); // holds all scheduling info 

            while (true)
            {
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
                    Console.Clear();

                    if (inputCSVFilepath != string.Empty &&
                        outputCSVFilepath != string.Empty &&
                        doses >= 1)
                    {
                        string[] inputCSV = File.ReadAllLines(inputCSVFilepath);

                        string[] priorityOrder = CreateVaccinationOrder(inputCSV, doses, vaccinateChildren);

                        PriorityOrderToCSV(priorityOrder, outputCSVFilepath);
                    }

                    if (inputCSVFilepath == string.Empty)
                    {
                        Console.WriteLine("Välj indatafil först.");
                    }

                    if (outputCSVFilepath == string.Empty)
                    {
                        Console.WriteLine("Välj utdatafil först.");
                    }

                    if (doses < 1)
                    {
                        Console.WriteLine("Antalet tillgängliga doser måste vara 1 eller mer.");
                    }

                    Console.WriteLine();
                }
                else if (mainMenu == 1) // schedule vaccinations 
                {
                    schedule = ScheduleVaccinations(schedule);
                }
                else if (mainMenu == 2) // change nr. of available doses 
                {
                    doses = ChangeVaccineDosages();
                }
                else if (mainMenu == 3) // change age / vaccinate children? yes/no 
                {
                    vaccinateChildren = ChangeAgeRequirement();
                }
                else if (mainMenu == 4) // change input filepath
                {
                    inputCSVFilepath = ChangeFilePathCSV(isOutputPath: false);
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

        // method for scheduling vaccinations, main menu points here and treats this as a sub-menu 
        public static Schedule ScheduleVaccinations(Schedule schedule)
        {
            /*The first vaccination should take place on a date selected by the user.
             * Two people can be vaccinated at the same time.
             * Every vaccination takes 5 minutes.
             * Vaccination should be done cotiniously in the same speed from 8:00 to 20:00, every day of the week.
             * The schedule should only contain the first dose for every person.
             * The schedule should be saved in a .Ics file.
             */

            var newSchedule = schedule;

            while (true)
            {
                Console.WriteLine("Schemalägg vacinationer");
                Console.WriteLine("--------------------");
                Console.WriteLine("Mata in blankrad för att välja standardvärde.");

                int scheduleMenu = ShowMenu("", new[]
                {
                    $"Startdatum: {newSchedule.StartDate}", 
                    $"Starttid: {newSchedule.StartTime}",
                    $"Sluttid: {newSchedule.EndTime}",
                    $"Antal samtidiga vaccinationer: {newSchedule.ConcurrentVaccinations}",
                    $"Minuter per vaccination: {newSchedule.VaccinationTime}",
                    $"Kalenderfil: {newSchedule.FilePathICS}",
                    "Gå tillbaka till huvudmeny"
                });

                Console.Clear();

                if (scheduleMenu == 0)
                {
                    while (true)
                    {
                        Console.Write("Ange nytt startdatum (YYYY-MM-DD): ");
                        string input = Console.ReadLine();
                        var startDate = new DateTime();
                        Console.Clear();

                        try
                        {
                            startDate = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                            newSchedule.StartDate = startDate;
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("Felaktigt datumformat. Använd formatet: YYYY-MM-DD (år-månad-dag)");
                        }


                    if (!string.IsNullOrEmpty(input))
                    {
                        try
                        {
                            startDate = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                        }
                        catch
                        {
                            Console.WriteLine("Felaktigt datumformat. Använd formatet: YYYY-MM-DD (år-månad-dag)");
                        }

                    }
                    
                }
                else if (scheduleMenu == 1)
                {
                    while (true)
                    {
                    Console.WriteLine("Ange ny starttid. t.ex.: 12:00");
                    string input = Console.ReadLine();
                    TimeSpan startTime = new TimeSpan(8, 0, 0);
                    Console.Clear();

                        if (!string.IsNullOrEmpty(input))
                        {
                            try
                            {
                                DateTime time = DateTime.ParseExact(input, "HH:mm", null);
                                startTime = time.TimeOfDay;
                                newSchedule.StartTime = startTime;
                                break;
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Felaktigt tidsformat. Använd formated: HH:mm (timmar:minuter)");
                            }
                            
                        }
                    }

                }

                else if (scheduleMenu == 2)
                {
                    while (true)
                    {
                        TimeSpan endTime = new TimeSpan(20, 0, 0);

                        Console.WriteLine("Ange ny sluttid. t.ex.: 20:00");
                        string input = Console.ReadLine();

                        Console.Clear();

                        if (!string.IsNullOrEmpty(input))
                        {
                            try
                            {
                                DateTime time = DateTime.ParseExact(input, "HH:mm", null);
                                endTime = time.TimeOfDay;
                                newSchedule.EndTime = endTime;
                                break;
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Felaktigt tidsformat. Använd formated: HH:mm (timmar:minuter");
                            }
                            
                        }
                    }
                }
                else { return newSchedule; } // exits this sub-menu and goes back to main-menu (main-loop) 
            }


        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // this is the list the method will later return as a string[] 
            List<Person> sortedPeople = new List<Person>();

            // list where we will store the input from the CSV file
            List<Person> people = new List<Person>();

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

        // outputs array to filepath, associated with CreateVaccinationOrder() in main menu-context 
        public static void PriorityOrderToCSV(string[] priorityOrder, string filePath)
        {
            if (priorityOrder.Length == 0)
            {
                Console.WriteLine("Ingen prioritetsordning har skapats, ingen output skrivs " +
                    "till utdatafilen.");
                Console.WriteLine();
                return;
            }
            else if (File.Exists(filePath))
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

                Console.WriteLine("(Ex.: C:\\Windows\\Temp\\exempel.txt )");
                Console.WriteLine("---------------");
                Console.Write("Ny filsökväg: ");
                string newPath = Console.ReadLine().Trim();

                Console.Clear();

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
                Console.WriteLine("Sökvägen du angett är ogiltig, ange en giltig filsökväg.");
                Console.WriteLine("Tänk på att välja rätt fil-ändelse (.csv/.CSV)");
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

        [TestMethod]
        public void VaccinateChildrenFalse()
        {
            string[] input =
            {
                "9704201910,Olsson,Hans,0,0,0",
                "921112-1912,Ek,Pontus,1,0,0",
                "9809041944,Sten,Kajsa,0,1,0",
                "19860301-1212,Smittadsson,Kent,1,0,1",
                "197002251234,Bok,Ida,0,1,1",
                "20100810-5555,Barnsson,Barnet,0,0,0",
                "201110101111,Ekblom,Josy,0,1,0",
                "201001021445,Blad,Hanna,0,1,1",
                "19400706-6666,Svensson,Jan,0,0,0",
                "197306061111,Eriksson,Petra,0,1,1",
                "19340501-1234,Nilsson,Peter,0,0,0"
            };
            int doses = 100;
            bool vaccinateChildren = false;

            string[] expected = {
                "19860301-1212,Smittadsson,Kent,1",
                "19921112-1912,Ek,Pontus,2",
                "19340501-1234,Nilsson,Peter,2",
                "19400706-6666,Svensson,Jan,2",
                "19700225-1234,Bok,Ida,1",
                "19730606-1111,Eriksson,Petra,1",
                "19980904-1944,Sten,Kajsa,2",
                "19970420-1910,Olsson,Hans,2",
            };

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            CollectionAssert.AreEqual(expected, output);

        }

        [TestMethod]
        public void VaccinateChildrenTrue()
        {
            string[] input = {
                "9704201910,Olsson,Hans,0,0,0",
                "201110101111,Ekblom,Josy,0,1,0",
                "201001021445,Blad,Hanna,0,1,1",
                "20200330-1990,Malm,Lennie,0,0,1",
                "20140101-1111,Svensson,Joel,0,0,0",
                "9809041944,Sten,Kajsa,0,1,0",
                "20220204-1399,Palme,Olof,0,0,0", // 2 children born 2 days apart
                "202202020754,Palme,Lisbeth,0,0,0" // this is in itself an interesting test 
            };
            int doses = 50;
            bool vaccinateChildren = true;

            string[] expected = {
                "19980904-1944,Sten,Kajsa,2",
                "20100102-1445,Blad,Hanna,1",
                "20111010-1111,Ekblom,Josy,2",
                "19970420-1910,Olsson,Hans,2",
                "20140101-1111,Svensson,Joel,2",
                "20200330-1990,Malm,Lennie,1",
                "20220202-0754,Palme,Lisbeth,2",
                "20220204-1399,Palme,Olof,2",
            };

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            CollectionAssert.AreEqual(expected, output);
        }

        [TestMethod]
        public void VaccinateOnlyChildren()
        {
            string[] input =
            {
                "201110101111,Ekblom,Josy,0,1,0",
                "201001021445,Blad,Hanna,0,1,1",
                "20200330-1990,Malm,Lennie,0,0,1",
                "20140101-1111,Svensson,Joel,0,0,0",
            };
            int doses = 50;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput =
            {
                "20100102-1445,Blad,Hanna,1",
                "20111010-1111,Ekblom,Josy,2",
                "20140101-1111,Svensson,Joel,2",
                "20200330-1990,Malm,Lennie,1"
            };
            CollectionAssert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void EmptyList()
        {
            string[] input =
            {

            };
            int doses = 50;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput =
            {

            };
            CollectionAssert.AreEqual(expectedOutput, output);
        }
        [TestMethod]
        public void OnlyChildren() // List with only children while "vaccinateChildren = false;"
        {
            string[] input =
            {
                "20100102-1445,Blad,Hanna,0,0,0",
                "20111010-1111,Ekblom,Josy,0,0,0",
                "20140101-1111,Svensson,Joel,0,1,0",
                "202003301990,Malm,Lennie,0,0,1"
            };

            int doses = 50;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput =
            {

            };

            CollectionAssert.AreEqual(expectedOutput, output);
        }
        [TestMethod]
        public void LastDoseToInfectedPerson()
        {
            string[] input =
            {
                "9704201910,Olsson,Hans,0,0,0",
                "921112-1912,Ek,Pontus,1,0,0",
                "9809041944,Sten,Kajsa,0,1,0",
                "19860301-1212,Smittadsson,Kent,1,0,1",
                "197002251234,Bok,Ida,0,1,1",
                "20100810-5555,Barnsson,Barnet,0,0,0",
                "201110101111,Ekblom,Josy,0,1,0",
                "201001021445,Blad,Hanna,0,1,1",
                "19400706-6666,Svensson,Jan,0,0,0",
                "197306061111,Eriksson,Petra,0,1,1",
                "19340501-1234,Nilsson,Peter,0,0,0"
            };

            int doses = 8;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput =
            {
                "19860301-1212,Smittadsson,Kent,1",
                "19921112-1912,Ek,Pontus,2",
                "19340501-1234,Nilsson,Peter,2",
                "19400706-6666,Svensson,Jan,2",
                "19700225-1234,Bok,Ida,1"

            };
            CollectionAssert.AreEqual(expectedOutput, output);

        }
        [TestMethod]
        public void NoDoses()
        {
            string[] input =
            {
                "9704201910,Olsson,Hans,0,0,0",
                "921112-1912,Ek,Pontus,1,0,0",
                "9809041944,Sten,Kajsa,0,1,0",
                "19860301-1212,Smittadsson,Kent,1,0,1",
                "197002251234,Bok,Ida,0,1,1",
                "20100810-5555,Barnsson,Barnet,0,0,0",
                "201110101111,Ekblom,Josy,0,1,0",
                "201001021445,Blad,Hanna,0,1,1",
                "19400706-6666,Svensson,Jan,0,0,0",
                "197306061111,Eriksson,Petra,0,1,1",
                "19340501-1234,Nilsson,Peter,0,0,0"
            };

            int doses = 0;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput =
            {


            };
            CollectionAssert.AreEqual(expectedOutput, output);

        }
        [TestMethod]
        public void LastDoseToNonInfectedPerson()
        {
            string[] input =
            {
                "9704201910,Olsson,Hans,0,0,0",
                "921112-1912,Ek,Pontus,1,0,0",
                "9809041944,Sten,Kajsa,0,1,0",
                "19860301-1212,Smittadsson,Kent,1,0,1",
                "197002251234,Bok,Ida,0,1,1",
                "20100810-5555,Barnsson,Barnet,0,0,0",
                "201110101111,Ekblom,Josy,0,1,0",
                "201001021445,Blad,Hanna,0,1,1",
                "19400706-6666,Svensson,Jan,0,0,0",
                "197306061111,Eriksson,Petra,0,1,1",
                "19340501-1234,Nilsson,Peter,0,0,0"
            };

            int doses = 7;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput =
            {
                "19860301-1212,Smittadsson,Kent,1",
                "19921112-1912,Ek,Pontus,2",
                "19340501-1234,Nilsson,Peter,2",
                "19400706-6666,Svensson,Jan,2",
            };
            CollectionAssert.AreEqual(expectedOutput, output);

        }
    }
}

