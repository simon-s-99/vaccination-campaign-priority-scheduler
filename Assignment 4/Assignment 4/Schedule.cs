using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaccination;

// Samuel Lööf & Simon Sörqvist, uppgift 4

namespace Schedule
{
    public class Info
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
        public static Info ScheduleMenu(Info schedule)
        {
           
             //The schedule should be saved in a .Ics file.
             

            var newSchedule = schedule;

            while (true)
            {
                Console.WriteLine("Schemalägg vacinationer");
                Console.WriteLine("--------------------");
                Console.WriteLine("Mata in blankrad för att välja standardvärde.");

                int scheduleMenu = Vaccination.Program.ShowMenu("", new[]
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

                if (scheduleMenu == 0) //Change the start date for the vaccinations
                {
                    newSchedule.StartDate = VaccinationStartDate();
                }
                else if (scheduleMenu == 1)
                {
                    while (true)
                    {
                        TimeSpan startTime = new TimeSpan(8, 0, 0);
                        Console.WriteLine("Ange ny starttid. t.ex.: 12:00");
                        string input = Console.ReadLine();

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
                                Console.WriteLine("Felaktigt tidsformat. Använd formatet: HH:mm (timmar:minuter).");
                            }

                        }
                    }
                }
                else if (scheduleMenu == 3)
                {
                    while (true)
                    {
                        Console.WriteLine("Hur många personer ska kunna vaccineras samtidigt?");

                        try
                        {
                            int input = int.Parse(Console.ReadLine());
                            newSchedule.ConcurrentVaccinations = input;
                            Console.Clear();
                            break;
                        }
                        catch (FormatException)
                        {
                            Console.Clear();
                            Console.WriteLine("Felaktigt format. Vänligen ange ett heltal.");
                            Console.WriteLine();
                        }
                    }
                }
                else if (scheduleMenu == 4)
                {
                    while (true)
                    {
                        Console.WriteLine("Hur länge ska varje vaccination vara (i minuter)?");
                        string input = Console.ReadLine();

                        if (!string.IsNullOrEmpty(input))
                        {
                            try
                            {
                                int minutes = int.Parse(input);
                                if (minutes >= 0)
                                {
                                    TimeSpan vaccinationTime = new TimeSpan(0, minutes, 0);
                                    newSchedule.VaccinationTime = vaccinationTime;
                                    break;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine(" Felaktigt tidsformat. Ange ett positivt heltal."); //Keep or not?
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
                    Console.Clear();
                }
                else if (scheduleMenu == 5)
                {
                    while (true)
                    {
                        Console.WriteLine("Var vill du att .ics filen ska sparas?");
                    }
                }
                else { return newSchedule; } // exits this sub-menu and goes back to main-menu (main-loop) 
            }
        }
        public static DateTime VaccinationStartDate()
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
                    return startDate;
                }
                catch
                {
                    Console.WriteLine("Felaktigt datumformat. Använd formatet: YYYY-MM-DD (år-månad-dag).");
                }


                if (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                    }
                    catch
                    {
                        Console.WriteLine("Felaktigt datumformat. Använd formatet: YYYY-MM-DD (år-månad-dag).");
                    }

                }
                
            }
             
        }

    }
}
