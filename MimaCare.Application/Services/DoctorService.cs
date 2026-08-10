using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Application.Dtos.Doctor;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Constants;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly IHttpContextAccessor
        _httpContextAccessor;

    private readonly IDoctorRepository
        _doctorRepository;

    private readonly IUnitOfWork
        _unitOfWork;
    private readonly IUserService _userService;
    private readonly ISpecialtyRepository _specialtyRepository;

    public DoctorService(
        UserManager<ApplicationUser> userManager,
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor,
        ISpecialtyRepository specialtyRepository)
    {
        _userManager = userManager;

        _doctorRepository = doctorRepository;

        _httpContextAccessor = httpContextAccessor;

        _unitOfWork = unitOfWork;
        _userService = userService;
        _specialtyRepository = specialtyRepository;
    }

    public async Task<string> CreateDoctor(
        CreateDoctorDto dto)
    {
        await _unitOfWork
            .BeginTransactionAsync();

        try
        {
            var clinicIdClaim =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst("clinicId")?
                    .Value;

            if (string.IsNullOrEmpty(clinicIdClaim))
            {
                throw new UnauthorizedException(
                    "Clinic not found.");
            }

            if (!Guid.TryParse(
                clinicIdClaim,
                out var clinicId))
            {
                throw new BadRequestException(
                    "Invalid clinic id.");
            }

            var user =
                await _userService
                    .CreateUserAsync(
                        new CreateUserDto
                        {
                            FirstName = dto.FirstName,
                            LastName = dto.LastName,
                            Email = dto.Email,
                            Password = string.IsNullOrWhiteSpace(dto.Password)
                                ? "123456"
                                : dto.Password,
                            PhoneNumber = dto.PhoneNumber,
                            ClinicId = clinicId,
                            Role = Roles.Doctor
                        });

            var doctor =
                dto.Adapt<Doctor>();

            doctor.UserId = user.Id;

            doctor.ClinicId = clinicId;

            await _doctorRepository
                .CreateDoctor(doctor);

            await _unitOfWork
                .CommitAsync();

            return "Doctor creado con exito";
        }
        catch
        {
            await _unitOfWork
                .RollbackAsync();

            throw;
        }
    }

    public async Task<IEnumerable<DoctorDto>> GetAllDoctor()
    {
        var clinicIdClaim =
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("clinicId")?
                .Value;

        if (string.IsNullOrEmpty(
            clinicIdClaim))
        {
            throw new UnauthorizedException(
                "Clinic not found.");
        }

        if (!Guid.TryParse(
            clinicIdClaim,
            out var clinicId))
        {
            throw new BadRequestException(
                "Invalid clinic id.");
        }

        var doctors =
            await _doctorRepository
                .GetDoctors(clinicId);

        return doctors
            .Adapt<IEnumerable<DoctorDto>>();
    }

    public async Task<DoctorDto> GetDoctorById(Guid id)
    {
        var doctor =
            await _doctorRepository
                .GetDoctorById(id);

        if (doctor == null)
        {
            throw new NotFoundException(
                "Doctor not found.");
        }

        return doctor
            .Adapt<DoctorDto>();
    }

    public async Task<string>UpdateDoctor(Guid doctorId,UpdateDoctorDto dto)
    {
        var doctor =
            await _doctorRepository
                .GetDoctorById(doctorId)
            ?? throw new NotFoundException(
                "Doctor no existe");


        var emailExists =
            await _userManager
                .FindByEmailAsync(dto.Email);

        if (emailExists != null &&
            emailExists.Id != doctor.UserId)
        {
            throw new ConflictException(
                "Email already exists");
        }


        dto.Adapt(doctor.User);

    

        var result =
            await _userManager
                .UpdateAsync(doctor.User);

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                string.Join(
                    ", ",
                    result.Errors
                        .Select(x =>
                            x.Description)));
        }

        dto.Adapt(doctor);

        await _doctorRepository
            .UpdateDoctor(doctor);


        await _unitOfWork
            .CommitAsync();

        return "Doctor actualizado correctamente";
    }

    public async Task<string> DeleteDoctor(Guid doctorId)
    {
        var doctor = await _doctorRepository.GetDoctorById(doctorId)
            ?? throw new NotFoundException("Doctor no existe");

        await _unitOfWork
           .BeginTransactionAsync();

        await _userManager.DeleteAsync(doctor.User);

        //await _doctorRepository.DeleteDoctor(doctor);

        await _unitOfWork
           .CommitAsync();


        return "Doctor eliminado correctamente";


    }

    public async Task<string> ResetDoctorPassword(Guid doctorId)
    {
        var doctor = await _doctorRepository.GetDoctorById(doctorId)
            ?? throw new NotFoundException("Doctor no existe");

        var token = await _userManager
            .GeneratePasswordResetTokenAsync(doctor.User);

        var result = await _userManager
            .ResetPasswordAsync(
                doctor.User,
                token,
                "123456");

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                string.Join(
                    ", ",
                    result.Errors
                        .Select(x =>
                            x.Description)));
        }

        return "Contraseña restablecida correctamente. La nueva contraseña es 123456.";
    }

    public async Task<IEnumerable<string>> GetSpecialties()
    {
        var specialties = await _specialtyRepository.GetAllSpecialties();
        return specialties.Select(s => s.Name);
    }
}