using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Seed
{
    public static class SpecialtySeeder
    {
        public static async Task SeedAsync(MimaCareContext context)
        {
            if (await context.Specialties.AnyAsync())
            {
                return;
            }

            string[] names =
            [
                "Medicina General",
                "Cardiología",
                "Dermatología",
                "Pediatría",
                "Ginecología y Obstetricia",
                "Oftalmología",
                "Otorrinolaringología",
                "Traumatología y Ortopedia",
                "Neurología",
                "Psiquiatría",
                "Endocrinología",
                "Gastroenterología",
                "Neumología",
                "Urología",
                "Nefrología",
                "Reumatología",
                "Oncología",
                "Hematología",
                "Infectología",
                "Medicina Interna",
                "Medicina Familiar",
                "Cirugía General",
                "Anestesiología",
                "Radiología",
                "Nutriología",
                "Geriatría"
            ];

            foreach (var name in names)
            {
                context.Specialties.Add(new Specialty { Name = name });
            }

            await context.SaveChangesAsync();
        }
    }
}
