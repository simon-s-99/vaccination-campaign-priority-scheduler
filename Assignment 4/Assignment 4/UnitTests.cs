using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// Samuel Lööf & Simon Sörqvist, uppgift 4

namespace Test
{
    [TestClass]
    public class CreateVaccinationOrder
    {
        [TestMethod]
        public void BaseFunctionalityTest() // Jakobs test from starter code 
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
            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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

            string[] expected =
            {
                "19860301-1212,Smittadsson,Kent,1",
                "19921112-1912,Ek,Pontus,2",
                "19340501-1234,Nilsson,Peter,2",
                "19400706-6666,Svensson,Jan,2",
                "19700225-1234,Bok,Ida,1",
                "19730606-1111,Eriksson,Petra,1",
                "19980904-1944,Sten,Kajsa,2",
                "19970420-1910,Olsson,Hans,2",
            };

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            CollectionAssert.AreEqual(expected, output);
        }

        [TestMethod]
        public void VaccinateChildrenTrue()
        {
            string[] input =
            {
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

            string[] expected =
            {
                "19980904-1944,Sten,Kajsa,2",
                "20100102-1445,Blad,Hanna,1",
                "20111010-1111,Ekblom,Josy,2",
                "19970420-1910,Olsson,Hans,2",
                "20140101-1111,Svensson,Joel,2",
                "20200330-1990,Malm,Lennie,1",
                "20220202-0754,Palme,Lisbeth,2",
                "20220204-1399,Palme,Olof,2",
            };

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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
            string[] input = Array.Empty<string>();
            int doses = 50;
            bool vaccinateChildren = true;

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expected = Array.Empty<string>();

            CollectionAssert.AreEqual(expected, output);
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

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput = Array.Empty<string>();

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
                "197306061111,Eriksson,Petra,0,1,0",
                "19340501-1234,Nilsson,Peter,0,0,0"
            };

            int doses = 9; // only 8 doses administered, the last one is not used 
            bool vaccinateChildren = false;

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            string[] expectedOutput = Array.Empty<string>();

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

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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

    [TestClass]
    public class PriorityOrderToICSRawText
    {
        // this is used for testing PriorityOrderToICSRawText so we get a reliable/testable UID value 
        private int FixedUIDAddon() { return 99; }

        /*
         * IMPORTANT:
         * Outputs from the following tests CAN NOT be put into an .ics (iCalendar) file
         * the UID is not unique due to the use of FixedUIDAddon().
         * Use the actual program or the actual PriorityOrderToICSRawText() with a random UID-addon
         * if you want a functional .ics file. 
         */

        [TestMethod]
        public void BaseFunctionalityTest() // basic test with default values for schedule 
        {
            var schedule = new Schedule.Info();
            schedule.StartDate = new DateTime(2023, 11, 1);
            schedule.StartTime = new TimeSpan(8, 0, 0);
            schedule.EndTime = new TimeSpan(20, 0, 0);
            schedule.VaccinationTime = new TimeSpan(0, 5, 0);
            schedule.ConcurrentVaccinations = 2;

            string[] priorityOrder =
            {
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
            };

            string[] result = Schedule.SubMenu.PriorityOrderToICSRawText(priorityOrder,
                schedule, FixedUIDAddon);

            string[] expected =
            {
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "PRODID:-//hacksw/handcal//NONSGML v1.0//EN",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "END:VCALENDAR"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AllVaccinationsConcurrent()
        {
            var schedule = new Schedule.Info();
            schedule.StartDate = new DateTime(2023, 11, 1);
            schedule.StartTime = new TimeSpan(8, 0, 0);
            schedule.EndTime = new TimeSpan(20, 0, 0);
            schedule.VaccinationTime = new TimeSpan(0, 5, 0);
            schedule.ConcurrentVaccinations = 50;

            string[] priorityOrder =
            {
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
            };

            string[] result = Schedule.SubMenu.PriorityOrderToICSRawText(priorityOrder,
                schedule, FixedUIDAddon);

            string[] expected =
            {
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "PRODID:-//hacksw/handcal//NONSGML v1.0//EN",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "END:VCALENDAR"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OneVaccinationPerDay()
        {
            var schedule = new Schedule.Info();
            schedule.StartDate = new DateTime(2023, 11, 1);
            schedule.StartTime = new TimeSpan(8, 0, 0);
            schedule.EndTime = new TimeSpan(8, 5, 0);
            schedule.VaccinationTime = new TimeSpan(0, 4, 0);
            schedule.ConcurrentVaccinations = 1;

            string[] priorityOrder =
            {
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
                "19320101-1122,Svensson,Janne,2",
                "19940202-2244,Berg,Ida,1",
            };

            string[] result = Schedule.SubMenu.PriorityOrderToICSRawText(priorityOrder,
                schedule, FixedUIDAddon);

            string[] expected =
            {
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "PRODID:-//hacksw/handcal//NONSGML v1.0//EN",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080400",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231102T08000099@example.com",
                "DTSTAMP:20231102T080000",
                "DTSTART:20231102T080000",
                "DTEND:20231102T080400",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231103T08000099@example.com",
                "DTSTAMP:20231103T080000",
                "DTSTART:20231103T080000",
                "DTEND:20231103T080400",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20231104T08000099@example.com",
                "DTSTAMP:20231104T080000",
                "DTSTART:20231104T080000",
                "DTEND:20231104T080400",
                "SUMMARY:19940202-2244,Berg,Ida,Doser=1",
                "END:VEVENT",
                "END:VCALENDAR"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EmptyPriorityOrder() //an empty priorityOrder throws ArgumentException, passes if it is thrown
        {
            var schedule = new Schedule.Info();
            schedule.StartDate = new DateTime(2023, 11, 1);
            schedule.StartTime = new TimeSpan(8, 0, 0);
            schedule.EndTime = new TimeSpan(8, 5, 0);
            schedule.VaccinationTime = new TimeSpan(0, 4, 0);
            schedule.ConcurrentVaccinations = 1;

            string[] priorityOrder = Array.Empty<string>();

            string[] result = Array.Empty<string>();

            try
            {
                result = Schedule.SubMenu.PriorityOrderToICSRawText(priorityOrder,
                    schedule, FixedUIDAddon);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(true, true); // pass test, ArgumentException is expected behaviour
            }
            catch
            {
                Assert.AreEqual(true, false); // fail test if any other type of argument is thrown 
            }

            if (result != priorityOrder)
            {
                Assert.AreEqual(true, false); // fail test if result[] is not empty 
            }
        }

        [TestMethod]
        public void OneVaccination()
        {
            var schedule = new Schedule.Info();
            schedule.StartDate = new DateTime(2023, 11, 1);
            schedule.StartTime = new TimeSpan(8, 0, 0);
            schedule.EndTime = new TimeSpan(20, 0, 0);
            schedule.VaccinationTime = new TimeSpan(0, 5, 0);
            schedule.ConcurrentVaccinations = 2;

            string[] priorityOrder =
            {
                "19320101-1122,Svensson,Janne,2",
            };

            string[] result = Schedule.SubMenu.PriorityOrderToICSRawText(priorityOrder,
                schedule, FixedUIDAddon);

            string[] expected =
            {
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "PRODID:-//hacksw/handcal//NONSGML v1.0//EN",
                "BEGIN:VEVENT",
                "UID:20231101T08000099@example.com",
                "DTSTAMP:20231101T080000",
                "DTSTART:20231101T080000",
                "DTEND:20231101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "END:VCALENDAR"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void BigPriorityOrder() // not really a BIG priorityOrder but bigger than the other tests
        {
            var schedule = new Schedule.Info();
            schedule.StartDate = new DateTime(2020, 1, 1);
            schedule.StartTime = new TimeSpan(8, 0, 0);
            schedule.EndTime = new TimeSpan(18, 0, 0);
            schedule.VaccinationTime = new TimeSpan(0, 5, 0);
            schedule.ConcurrentVaccinations = 2;

            string[] priorityOrder =
            {
                "19320101-1122,Svensson,Janne,2",
                "19400225-1324,Jansson,Bo,2",
                "19450505-1123,Bengtsson,Ulrika,1",
                "19470303-1929,Hansson,Hugo,2",
                "19801111-1634,Olsson,Selma,2",
                "19810324-1632,Lindqvist,Alma,1",
                "19890419-2934,Sandberg,Noah,1",
                "19910911-3922,Berg,Lilly,2",
                "19990720-5443,Engström,Astrid,2",
                "20010613-6151,Persson,Valter,1",
            };

            string[] result = Schedule.SubMenu.PriorityOrderToICSRawText(priorityOrder,
                schedule, FixedUIDAddon);

            string[] expected =
            {
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "PRODID:-//hacksw/handcal//NONSGML v1.0//EN",
                "BEGIN:VEVENT",
                "UID:20200101T08000099@example.com",
                "DTSTAMP:20200101T080000",
                "DTSTART:20200101T080000",
                "DTEND:20200101T080500",
                "SUMMARY:19320101-1122,Svensson,Janne,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08000099@example.com",
                "DTSTAMP:20200101T080000",
                "DTSTART:20200101T080000",
                "DTEND:20200101T080500",
                "SUMMARY:19400225-1324,Jansson,Bo,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08050099@example.com",
                "DTSTAMP:20200101T080500",
                "DTSTART:20200101T080500",
                "DTEND:20200101T081000",
                "SUMMARY:19450505-1123,Bengtsson,Ulrika,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08050099@example.com",
                "DTSTAMP:20200101T080500",
                "DTSTART:20200101T080500",
                "DTEND:20200101T081000",
                "SUMMARY:19470303-1929,Hansson,Hugo,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08100099@example.com",
                "DTSTAMP:20200101T081000",
                "DTSTART:20200101T081000",
                "DTEND:20200101T081500",
                "SUMMARY:19801111-1634,Olsson,Selma,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08100099@example.com",
                "DTSTAMP:20200101T081000",
                "DTSTART:20200101T081000",
                "DTEND:20200101T081500",
                "SUMMARY:19810324-1632,Lindqvist,Alma,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08150099@example.com",
                "DTSTAMP:20200101T081500",
                "DTSTART:20200101T081500",
                "DTEND:20200101T082000",
                "SUMMARY:19890419-2934,Sandberg,Noah,Doser=1",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08150099@example.com",
                "DTSTAMP:20200101T081500",
                "DTSTART:20200101T081500",
                "DTEND:20200101T082000",
                "SUMMARY:19910911-3922,Berg,Lilly,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08200099@example.com",
                "DTSTAMP:20200101T082000",
                "DTSTART:20200101T082000",
                "DTEND:20200101T082500",
                "SUMMARY:19990720-5443,Engström,Astrid,Doser=2",
                "END:VEVENT",
                "BEGIN:VEVENT",
                "UID:20200101T08200099@example.com",
                "DTSTAMP:20200101T082000",
                "DTSTART:20200101T082000",
                "DTEND:20200101T082500",
                "SUMMARY:20010613-6151,Persson,Valter,Doser=1",
                "END:VEVENT",
                "END:VCALENDAR"
            };

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
