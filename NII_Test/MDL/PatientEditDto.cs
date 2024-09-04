namespace NII_Test.MDL
{
    public class PatientEditDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int DoctorId { get; set; }  // Id врача для редактирования
    }
}
