using System.ComponentModel;

namespace FIAP.PosTech.Hackathon.Domain.Enums;

public enum AppointmentStatusEnum
{
    [Description("Confirmado")]
    Confirmed = 1,

    [Description("Agendado")]
    Pending = 2,

    [Description("Recusado")]
    Refused = 3,

    [Description("Cancelado")]
    Canceled = 4,
}