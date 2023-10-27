using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedule;
using System;

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

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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
            string[] input =
            {

                };
            int doses = 50;
            bool vaccinateChildren = true;

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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

            string[] output = Vaccination.Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

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
    public class ScheduleTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            string[] priorityOrder = new string[] { "Name,Namesson,19950202-2244,1", "AnotherName,AnotherNamesson,19900101-1122,2" };
            Schedule.Info scheduleInfo = new Schedule.Info
            {
                StartDate = new DateTime(2023, 11, 1),
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                VaccinationTime = new TimeSpan(0, 5, 0)
            };

            // Act
            //string[] result = Schedule.PriorityOrderToICSRawText(priorityOrder, scheduleInfo);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }
    }
}
