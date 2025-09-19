using Contact_Manager_Application.Db;
using Contact_Manager_Application.Interfaces;
using Contact_Manager_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Contact_Manager_Application.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly AdmissionsDbContext _context;

        public FileProcessingService(AdmissionsDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? ErrorMessage)> ProcessFileAsync(IFormFile file)
        {
            bool success = true;
            string? errorMessage = null;

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            bool isFirstRow = true;
            int rowNumber = 0;

            while (!reader.EndOfStream)
            {
                rowNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length < 5)
                {
                    success = false;
                    errorMessage = $"Row {rowNumber}: Not enough columns.";
                    break;
                }

                string name = parts[0].Trim();
                string dobString = parts[1].Trim();
                string marriedString = parts[2].Trim();
                string phone = parts[3].Trim();
                string salaryString = parts[4].Trim();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(dobString) ||
                    string.IsNullOrEmpty(marriedString) || string.IsNullOrEmpty(phone) ||
                    string.IsNullOrEmpty(salaryString))
                {
                    success = false;
                    errorMessage = $"Row {rowNumber}: One or more fields are empty.";
                    break;
                }

                if (!DateTime.TryParse(dobString, out var dob))
                {
                    success = false;
                    errorMessage = $"Row {rowNumber}: Invalid DateOfBirth.";
                    break;
                }

                if (!bool.TryParse(marriedString, out var married))
                {
                    success = false;
                    errorMessage = $"Row {rowNumber}: Invalid Married value.";
                    break;
                }

                if (!decimal.TryParse(salaryString.Replace(".",","), out var salary))
                {
                    success = false;
                    errorMessage = $"Row {rowNumber}: Invalid Salary value.";
                    break;
                }

                var user = new User
                {
                    Name = name,
                    DateOfBirth = dob,
                    Married = married,
                    Phone = phone,
                    Salary = salary
                };

                bool exists = await _context.Users.AnyAsync(u =>
                    u.Name == user.Name && u.DateOfBirth == user.DateOfBirth
                );

                if (!exists)
                {
                    _context.Users.Add(user);
                }
            }

            if (success)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    success = false;
                    errorMessage = "Database error: " + ex.Message;
                }
            }

            return (success, errorMessage);
        }




        public Task MergeTempApplicationsAsync()
        {
            return Task.CompletedTask;
        }

        private DateTime TryParseDate(string? value)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return DateTime.MinValue;
        }

        private bool TryParseBool(string? value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            return value.Trim().ToLower() switch
            {
                "true" => true,
                "1" => true,
                "yes" => true,
                "так" => true,
                _ => false
            };
        }

        private decimal TryParseDecimal(string? value)
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0m;
        }
    }
}
