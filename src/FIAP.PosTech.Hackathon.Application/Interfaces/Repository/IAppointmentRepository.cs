using FIAP.PosTech.Hackathon.Domain.Entity;

namespace FIAP.PosTech.Hackathon.Application.Interfaces.Repository;

public interface IAppointmentRepository : IBaseRepository<Appointment>
{
    Task<Appointment?> GetByPatientIdAndScheduleIdAndStatusNullableAsync(int patientId, int scheduleId, int[] statusId);
}