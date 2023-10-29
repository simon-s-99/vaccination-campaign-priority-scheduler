using System;

// Samuel Lööf & Simon Sörqvist, uppgift 4

namespace Patient
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
}
