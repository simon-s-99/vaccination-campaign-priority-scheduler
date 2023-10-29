using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Samuel Lööf & Simon Sörqvist, uppgift 4

namespace Schedule
{
    public class Info
    {
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan VaccinationTime { get; set; }
        public int ConcurrentVaccinations { get; set; }
        public string FilePathICS { get; set; }

        public Info()
        {
            StartDate = DateTime.Today.AddDays(7);
            StartTime = new TimeSpan(8, 0, 0);
            EndTime = new TimeSpan(20, 0, 0);
            ConcurrentVaccinations = 2;
            VaccinationTime = new TimeSpan(0, 5, 0);
            FilePathICS = "C:\\Windows\\Temp\\Schedule.ics";
        }
    }

    public class SubMenu
    {
        // method for scheduling vaccinations, main menu points here and treats this as a sub-menu 
        public static Info ScheduleMenu(Schedule.Info schedule)
        {
            while (true)
            {
                Console.WriteLine("Schemalägg vaccinationer");
                Console.WriteLine("--------------------");
                Console.WriteLine("Mata in blankrad för att välja standardvärde.");

                int scheduleMenu = Vaccination.Program.ShowMenu("", new[]
                {
                    $"Startdatum: {schedule.StartDate.ToString("yyyy-MM-dd")}",
                    $"Starttid: {schedule.StartTime.ToString("hh\\:mm")}",
                    $"Sluttid: {schedule.EndTime.ToString("hh\\:mm")}",
                    $"Antal samtidiga vaccinationer: {schedule.ConcurrentVaccinations}",
                    $"Minuter per vaccination: {schedule.VaccinationTime.TotalMinutes}",
                    $"Kalenderfil: {schedule.FilePathICS}",
                    "Generera kalenderfil (.ics)",
                    "Gå tillbaka till huvudmeny"
                });

                Console.Clear();

                if (scheduleMenu == 0) //Change the start date for vaccinations
                {
                    schedule.StartDate = VaccinationStartDate();
                }
                else if (scheduleMenu == 1) //Change the start time for vaccinations
                {
                    schedule.StartTime = VaccinationStartTime(schedule);
                }
                else if (scheduleMenu == 2) //Change the the end time for vacciantions
                {
                    schedule.EndTime = VaccinationEndTime(schedule);
                }
                //Change the number of people that's allowed to get vaccinated at the same time
                else if (scheduleMenu == 3)
                {
                    schedule.ConcurrentVaccinations = ConcurrentVaccinations();
                }
                else if (scheduleMenu == 4) //Change how many minutes each vaccination should take.
                {
                    schedule.VaccinationTime = VaccinatonDuration();
                }
                else if (scheduleMenu == 5) //Choose where to save the calendar .ics file.
                {
                    Console.WriteLine("Var vill du att .ics filen ska sparas?");

                    schedule.FilePathICS = ChangeFilePathICS();
                }
                else if (scheduleMenu == 6) // generate the .ics file 
                {
                    CreateScheduleICSFile(schedule);
                }
                else { return schedule; } // exits this sub-menu and goes back to main-menu (main-loop) 
            }
        }

        public static DateTime VaccinationStartDate()
        {
            while (true)
            {
                Console.Write("Ange nytt startdatum (YYYY-MM-DD): ");
                string input = Console.ReadLine();
                Console.Clear();

                if (string.IsNullOrEmpty(input))
                {
                    return DateTime.Today.AddDays(7); // Set it to default value
                }

                try
                {
                    var startDate = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                    return startDate;
                }
                catch
                {
                    Console.WriteLine("Felaktigt datumformat. Använd formatet: YYYY-MM-DD (år-månad-dag).");
                }
            }
        }

        public static TimeSpan VaccinationStartTime(Schedule.Info schedule)
        {
            while (true)
            {
                Console.Write("Ange ny starttid (ex.: 12:00) : ");
                string input = Console.ReadLine();
                Console.Clear();

                if (string.IsNullOrEmpty(input))
                {
                    return new TimeSpan(8, 0, 0); // Set it to default value
                }

                var time = new TimeSpan();
                try
                {
                    time = TimeSpan.ParseExact(input, "h\\:mm", null);
                }
                catch
                {
                    Console.WriteLine("Felaktigt tidsformat. Använd formatet: HH:mm (timmar:minuter).");
                }

                if (time >= schedule.EndTime)
                {
                    Console.WriteLine("Starttiden måste vara tidigare än sluttiden.");
                }
                else
                {
                    return time;
                }

                Console.WriteLine();
            }
        }

        public static TimeSpan VaccinationEndTime(Schedule.Info schedule)
        {
            while (true)
            {
                Console.Write("Ange ny sluttid (ex.: 20:00) : ");
                string input = Console.ReadLine();
                Console.Clear();

                if (string.IsNullOrEmpty(input))
                {
                    return new TimeSpan(20, 0, 0); // Set it to default value
                }

                var time = new TimeSpan();
                try
                {
                    time = TimeSpan.ParseExact(input, "h\\:mm", null);
                }
                catch
                {
                    Console.WriteLine("Felaktigt tidsformat. Använd formatet: HH:mm (timmar:minuter).");
                }

                if (time <= schedule.StartTime)
                {
                    Console.WriteLine("Sluttiden måste vara senare än starttiden.");
                }
                else
                {
                    return time;
                }

                Console.WriteLine();
            }
        }

        public static int ConcurrentVaccinations()
        {
            while (true)
            {
                Console.Write("Hur många personer ska kunna vaccineras samtidigt: ");
                string input = Console.ReadLine();
                Console.Clear();

                if (string.IsNullOrEmpty(input))
                {
                    return 2; // Set it to default value
                }

                int inputAsNr = 0;

                try
                {
                    inputAsNr = int.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Felaktigt format. Vänligen ange ett heltal.");
                    Console.WriteLine();
                    continue;
                }

                if (inputAsNr > 0)
                {
                    return inputAsNr;
                }
                else
                {
                    Console.WriteLine("Felaktigt format. Vänligen ange ett positivt heltal.");
                    Console.WriteLine();
                }
            }
        }

        public static TimeSpan VaccinatonDuration()
        {

            while (true)
            {
                Console.Write("Hur länge ska varje vaccination vara (i minuter): ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.Clear();
                    return new TimeSpan(0, 5, 0); // Set it to default value
                }
                try
                {
                    int minutes = int.Parse(input);
                    if (minutes > 0)
                    {
                        Console.Clear();
                        TimeSpan vaccinationTime = new TimeSpan(0, minutes, 0);
                        return vaccinationTime;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Felaktigt tidsformat. Ange ett positivt heltal.");
                        Console.WriteLine();
                    }
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Felaktigt tidsformat. Ange vaccinationtiden i minuter.");
                    Console.WriteLine();
                }
            }
        }

        public static string ChangeFilePathICS()
        {
            while (true)
            {
                Console.WriteLine("(Ex.: C:\\Windows\\Temp\\exempel.ics)");
                Console.WriteLine("---------------");
                Console.Write("Ny filsökväg: ");
                string newPath = Console.ReadLine().Trim();

                Console.Clear();

                if (string.IsNullOrEmpty(newPath)) // standard value if user input is empty 
                {
                    return "C:\\Windows\\Temp\\Schedule.ics";
                }

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
                        string tempPath = newPath.Substring(0, newPath.LastIndexOf("\\"));
                        if (Directory.Exists(tempPath))
                        {
                            if (fileExtension == "ics" || fileExtension == "ICS")
                            {
                                return newPath;
                            }
                        }
                    }
                }

                // tell user to try again
                Console.WriteLine("Sökvägen du angett är ogiltig, ange en giltig filsökväg.");
                Console.WriteLine("Tänk på att välja rätt fil-ändelse (.ics/.ICS)");
                Console.WriteLine("Filnamnet får inte innehålla något av följande tecken: \\/:*?\"<>|");
                Console.WriteLine();
            }
        }

        // takes vaccination priority order as input (string[]) and returns the lines for the ics file
        // Func<int> rand parameter is for testing purposes, when this method is called it should be a Random()
        public static string[] PriorityOrderToICSRawText(string[] priorityOrder, Info scheduleInfo,
            Func<int> rand)
        {
            // priorityOrder must be greater than 0, throws ArgumentException if not
            if (priorityOrder.Length > 0)
            {
                // output list with the first template values of a raw text ics-file 
                var outputICS = new List<string>
                {
                    "BEGIN:VCALENDAR",
                    "VERSION:2.0",
                    "PRODID:-//hacksw/handcal//NONSGML v1.0//EN",
                };

                // initial values used to handle start/stop times
                DateTime currentDate = scheduleInfo.StartDate.Add(scheduleInfo.StartTime);
                DateTime timeLimit = scheduleInfo.StartDate.Add(scheduleInfo.EndTime);

                int concurrentVaccinationsDone = 0;
                for (int i = 0; i < priorityOrder.Length;) // counter increments in for-loop
                {
                    // updates currentDate after every set of vaccinations
                    if (concurrentVaccinationsDone == scheduleInfo.ConcurrentVaccinations)
                    {
                        currentDate = currentDate.Add(scheduleInfo.VaccinationTime);
                        concurrentVaccinationsDone = 0;
                    }

                    string[] vaccinationInfo = priorityOrder[i].Split(',');

                    var tempDate = currentDate.Add(scheduleInfo.VaccinationTime);
                    if (tempDate < timeLimit)
                    {
                        outputICS.Add("BEGIN:VEVENT");

                        string rawTextTimeFormat = currentDate.ToString("yyyyMMdd") +
                            "T" + currentDate.ToString("HHmmss");

                        string rawTextTimeFormatPlusVaccinationTime =
                            tempDate.ToString("yyyyMMdd") + "T" + tempDate.ToString("HHmmss");

                        // same identifier (UID) is technically possible but HIGHLY unlikely
                        outputICS.Add($"UID:{rawTextTimeFormat + rand()}@example.com");
                        outputICS.Add($"DTSTAMP:{rawTextTimeFormat}");
                        outputICS.Add($"DTSTART:{rawTextTimeFormat}");
                        outputICS.Add($"DTEND:{rawTextTimeFormatPlusVaccinationTime}");
                        outputICS.Add($"SUMMARY:{vaccinationInfo[0]},{vaccinationInfo[1]}," +
                            $"{vaccinationInfo[2]},Doser={vaccinationInfo[3]}");

                        outputICS.Add("END:VEVENT");

                        concurrentVaccinationsDone++;
                        i++; // <-------------------------- COUNTER INCREMENTS HERE (after adding an event)
                    }
                    else
                    {
                        // update currentdate and timeLimit when end of day is reached
                        currentDate = currentDate.AddDays(1);
                        currentDate = new DateTime(currentDate.Year, currentDate.Month,
                            currentDate.Day, 0, 0, 0);
                        currentDate = currentDate.Add(scheduleInfo.StartTime);
                        timeLimit = timeLimit.AddDays(1);
                    }
                }

                outputICS.Add("END:VCALENDAR"); // ends ics file-template 

                return outputICS.ToArray();
            }
            else
            {
                throw new ArgumentException("Parameter priorityOrder[] must be greater than 0.");
            }
        }

        public static void CreateScheduleICSFile(Schedule.Info schedule)
        {
            if (string.IsNullOrEmpty(Vaccination.Program.InputCSVFilepath) ||
                        Vaccination.Program.Doses < 1)
            {
                Console.WriteLine("Vänligen gå tillbaka till huvudmenyn och välj en");
                Console.WriteLine("indatafil och mata in mängden tillgängliga doser vaccin.");
                Console.WriteLine();
            }
            else
            {
                string[] inputCSV = File.ReadAllLines(Vaccination.Program.InputCSVFilepath);
                string[] priorityOrder = Vaccination.Program.CreateVaccinationOrder(
                    inputCSV,
                    Vaccination.Program.Doses,
                    Vaccination.Program.VaccinateChildren);

                var icsRawText = new List<string>();

                Console.Clear();

                try
                {
                    icsRawText = PriorityOrderToICSRawText(priorityOrder,
                        schedule, new Random().Next).ToList();
                }
                catch // here to catch ArgumentException if priorityOrder is empty (length < 0) 
                {
                    Console.WriteLine("Fel vid försök att skapa en prioritetsordning.");
                    Console.WriteLine("Inget schema har skapats, vänligen försök igen.");
                    Console.WriteLine();
                    return;
                }

                if (File.Exists(schedule.FilePathICS))
                {
                    int overwriteMenu = Vaccination.Program.ShowMenu($"Filen existerar redan. Vill du skriva över den?",
                        new[]
                    {
                        "Ja",
                        "Nej"
                    });

                    Console.Clear();

                    if (overwriteMenu == 0)
                    {
                        File.WriteAllLines(schedule.FilePathICS, icsRawText.ToArray());
                        Console.WriteLine($"Vaccinations-schemat har sparats i {schedule.FilePathICS}");
                        Console.WriteLine();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Inget schema har skapats, ändra kalenderns filsökväg om du vill " +
                            "skapa en .ics fil.");
                        Console.WriteLine();
                    }
                }
                else
                {
                    File.WriteAllLines(schedule.FilePathICS, icsRawText.ToArray());
                    Console.WriteLine($"Vaccinations-schemat har sparats i {schedule.FilePathICS}");
                    Console.WriteLine();
                }
            }
        }
    }
}
