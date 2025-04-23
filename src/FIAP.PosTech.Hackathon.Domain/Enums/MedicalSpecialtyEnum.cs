using System.ComponentModel;

namespace FIAP.PosTech.Hackathon.Domain.Enums;

public enum MedicalSpecialtyEnum
{
    [Description("Psicologia")]
    Psicologia = 1,

    [Description("Ortopedia")]
    Ortopedia = 2,

    [Description("Dermatologia")]
    Dermatologia = 3,

    [Description("Ginecologia")]
    Ginecologia = 4,

    [Description("Odontologia")]
    Odontologia = 5,

    [Description("Cardiologia")]
    Cardiologia = 6,

    [Description("Pediatria")]
    Pediatria = 7,

    [Description("Fonoaudiologia")]
    Fonoaudiologia = 8,

    [Description("Urologia")]
    Urologia = 9,

    [Description("Neurologia")]
    Neurologia = 10,

    [Description("Psiquiatria")]
    Psiquiatria = 11,

    [Description("Endocrinologia")]
    Endocrinologia = 12,

    [Description("Oncologia")]
    Oncologia = 13,

    [Description("Imunologia")]
    Imunologia = 14,

    [Description("Anestesiologia")]
    Anestesiologia = 15
}