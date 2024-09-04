namespace NII_Test.MDL
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DoctorName { get; set; }  // Имя врача вместо Id
    }
}
