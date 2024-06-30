using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tutorial10.Data;
using Tutorial10.Helpers;
using Tutorial10.Models;
using Tutorial10.Models.DTOs;
using Tutorial10.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Tutorial10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IConfiguration _configuration;

    public PatientController(IPatientService patientService, IConfiguration configuration)
    {
        _patientService = patientService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async  Task<IActionResult> RegisterStudent(RegistrationRequest model)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(model.Password);

        var user = new User()
        {
            Username = model.Username,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = SecurityHelpers.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1)
        };

        await _patientService.AddUser(user);

        return Ok();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddPrescription(PrescriptionDTO prescriptionDto)
    {
        await _patientService.AddPatientIfNotExist(prescriptionDto.Patient);

        var checkMeds =await  _patientService.CheckMedicaments(prescriptionDto.Medicaments);

        if (!checkMeds)
        {
            return StatusCode(403, "Incorrect input of a medicaments list");
        }

        var checkDates = _patientService.CheckDates(DateOnly.Parse(prescriptionDto.Date), DateOnly.Parse(prescriptionDto.DueDate));

        if (!checkDates)
        {
            return StatusCode(403, "Incorrect input: DueDate is smaller than Date");
        }

        await _patientService.AddPrescription(prescriptionDto);
        
        return Created();
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _patientService.GetPatient(id);
        patient.Prescriptions = patient.Prescriptions.OrderBy(p => p.DueDate).ToList();
        return Ok(patient);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoggingRequest loggingRequest)
    {
        User? user = await _patientService.GetUser(loggingRequest);
        
        if (user == null)
        {
            return Unauthorized("There is no user with such username!");
        }
        
        string passwordHashFromDb = user.Password;
        string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(loggingRequest.Password, user.Salt);

        if (passwordHashFromDb != curHashedPassword)
        {
            return Unauthorized("Incorrect password!");
        }
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F47D5314593E2QD3KGPGLT603DBKJYPR"));

        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: credentials
        );

        var refreshToken = await _patientService.NewRefreshToken(user);
        

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = refreshToken
        });
    }

    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest refreshToken)
    {
        User? user = await _patientService.GetUserByRefreshToken(refreshToken);
        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (user.RefreshTokenExp < DateTime.Now)
        {
            throw new SecurityTokenException("Refresh token expired");
        }

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F47D5314593E2QD3KGPGLT603DBKJYPR"));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        var newRefreshToken =await _patientService.NewRefreshToken(user);

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            refreshToken = newRefreshToken
        });
    }
    
}