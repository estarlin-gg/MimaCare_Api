using Mapster;
using MimaCare.Application.Dtos.Doctor;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Mappers
{
    public class DoctorMapper : IRegister
    {
        public void Register(
            TypeAdapterConfig config)
        {
            // DOCTOR -> DTO
            config.NewConfig<
                Doctor,
                DoctorDto>()

                .Map(
                    dest => dest.Id,
                    src => src.UserId)

                .Map(
                    dest => dest.FullName,
                    src => $"{src.User.FirstName} {src.User.LastName}")

                .Map(
                    dest => dest.Email,
                    src => src.User.Email)

                .Map(
                    dest => dest.PhoneNumber,
                    src => src.User.PhoneNumber)

                .Map(
                    dest => dest.Specialty,
                    src => src.Specialty)

                .Map(
                    dest => dest.LicenseNumber,
                    src => src.LicenseNumber)

                .Map(
                    dest => dest.ProfileImageUrl,
                    src => src.ProfileImageUrl)

                .Map(
                    dest => dest.IsAvailable,
                    src => src.IsAvailable)

                .Map(
                    dest => dest.ClinicName,
                    src => src.Clinic.Name);


            // CREATE DTO -> USER
            config.NewConfig<
                CreateDoctorDto,
                ApplicationUser>()

                .Map(
                    dest => dest.FirstName,
                    src => src.FirstName)

                .Map(
                    dest => dest.LastName,
                    src => src.LastName)

                .Map(
                    dest => dest.PhoneNumber,
                    src => src.PhoneNumber);

            // CREATE DTO -> DOCTOR

            config.NewConfig<
                CreateDoctorDto,
                Doctor>()

                .Map(
                    dest => dest.Specialty,
                    src => src.Specialty)

                .Map(
                    dest => dest.LicenseNumber,
                    src => src.LicenseNumber);


            // UPDATE DTO -> USER
            config.NewConfig<
                UpdateDoctorDto,
                ApplicationUser>()

                .Map(
                    dest => dest.FirstName,
                    src => src.FirstName)

                .Map(
                    dest => dest.LastName,
                    src => src.LastName)

                .Map(
                    dest => dest.Email,
                    src => src.Email)

                .Map(
                    dest => dest.UserName,
                    src => src.Email)

                .Map(
                    dest => dest.PhoneNumber,
                    src => src.PhoneNumber);



            // UPDATE DTO -> DOCTOR
            config.NewConfig<
                UpdateDoctorDto,
                Doctor>()

                .Map(
                    dest => dest.Specialty,
                    src => src.Specialty)

                .Map(
                    dest => dest.LicenseNumber,
                    src => src.LicenseNumber)

                .Map(
                    dest => dest.IsAvailable,
                    src => src.IsAvailable);
        }
    }
}