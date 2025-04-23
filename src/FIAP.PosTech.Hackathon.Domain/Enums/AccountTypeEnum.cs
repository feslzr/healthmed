using System.ComponentModel;

namespace FIAP.PosTech.Hackathon.Domain.Enums;

public enum AccountTypeEnum
{
    [Description("Médico")]
    doctor = 1,

    [Description("Paciente")]
    patient = 2,
}