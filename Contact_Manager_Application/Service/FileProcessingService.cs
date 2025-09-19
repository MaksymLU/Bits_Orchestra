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

        public async Task ProcessFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            bool isFirstRow = true;
            while (!reader.EndOfStream)
            {
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
                    continue;

                var user = new User
                {
                    Name = parts[0],
                    DateOfBirth = TryParseDate(parts[1]),
                    Married = TryParseBool(parts[2]),
                    Phone = parts[3],
                    Salary = TryParseDecimal(parts[4])
                };

                // Перевірка на дублікати по Name + DateOfBirth
                bool exists = await _context.Users.AnyAsync(u =>
                    u.Name == user.Name &&
                    u.DateOfBirth == user.DateOfBirth
                );

                if (!exists)
                {
                    _context.Users.Add(user);
                }
            }

            await _context.SaveChangesAsync();
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
