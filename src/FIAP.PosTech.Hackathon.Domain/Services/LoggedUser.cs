using FIAP.PosTech.Hackathon.Domain.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Services;

[ExcludeFromCodeCoverage]
public class LoggedUser : ILoggedUser
{
    #region Variables

    private bool? DoctorAccess { get; set; }

    private bool? PatientAccess { get; set; }

    private string? User { get; set; }

    #endregion

    #region Public Methods

    public string GetUser() => User ?? string.Empty;
    public void SetUser(string user) => User = user;

    public bool IsDoctorAccess() => DoctorAccess ?? false;
    public void SetDoctorAccess(bool doctorAccess) => DoctorAccess = doctorAccess;

    public bool IsPatientAccess() => PatientAccess ?? false;
    public void SetPatientAccess(bool patientAccess) => PatientAccess = patientAccess;

    public bool Validate() => !string.IsNullOrEmpty(User);

    #endregion
}
