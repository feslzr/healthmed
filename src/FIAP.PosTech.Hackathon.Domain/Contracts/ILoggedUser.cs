namespace FIAP.PosTech.Hackathon.Domain.Contracts;

public interface ILoggedUser
{
    string GetUser();
    void SetUser(string user);

    bool IsDoctorAccess();
    void SetDoctorAccess(bool doctorAccess);

    bool IsPatientAccess();
    void SetPatientAccess(bool patientAccess);

    bool Validate();
}