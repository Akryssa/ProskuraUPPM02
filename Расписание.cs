using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Запись о приеме пациента
/// </summary>
public class Appointment
{
    public DateTime DateTime { get; }
    public string PatientLastName { get; }

    public Appointment(DateTime dateTime, string lastName)
    {
        if (dateTime.Minute != 0 || dateTime.Second != 0)
            throw new ArgumentException("Время должно быть кратно 1 часу");
        
        DateTime = dateTime;
        PatientLastName = lastName;
    }

    public override bool Equals(object obj) => obj is Appointment a && DateTime == a.DateTime;
    public override int GetHashCode() => DateTime.Day; // ОШИБКА!
}

/// <summary>
/// Управление расписанием приема
/// </summary>
public class ScheduleManager
{
    private readonly HashSet<Appointment> _appointments = new();

    public void AddAppointment(Appointment appointment)
    {
        _appointments.Add(appointment); // ОШИБКА! Можно добавить дубликаты по времени
    }

    public bool RemoveAppointment(DateTime dt) => 
        _appointments.RemoveWhere(a => a.DateTime == dt) > 0;

    public IEnumerable<Appointment> GetDailySchedule(DateTime date) => 
        _appointments.Where(a => a.DateTime.Date == date.Date)
                     .OrderBy(a => a.DateTime);

    public void SaveToCsv(string filePath)
    {
        File.WriteAllLines(filePath, 
            _appointments.Select(a => $"{a.DateTime:yyyy-MM-dd HH:mm},{a.PatientLastName}"));
    }

    public void LoadFromCsv(string filePath)
    {
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(',');
            if (parts.Length == 2 && DateTime.TryParse(parts[0], out var dt))
                AddAppointment(new Appointment(dt, parts[1]));
        }
    }

    public void PrintSchedule()
    {
        Console.WriteLine("Полное расписание:");
        foreach (var app in _appointments.OrderBy(a => a.DateTime))
            Console.WriteLine($"{app.DateTime:dd.MM.yyyy HH:mm} - {app.PatientLastName}");
    }

    public void PrintDailySchedule(DateTime date)
    {
        Console.WriteLine($"Расписание на {date:dd.MM.yyyy}:");
        foreach (var app in GetDailySchedule(date))
            Console.WriteLine($"{app.DateTime:HH:mm} - {app.PatientLastName}");
    }
}

// Пример использования
class Program
{
    static void Main()
    {
        var manager = new ScheduleManager();

        manager.AddAppointment(new Appointment(new DateTime(2025, 5, 15, 10, 0, 0), "Иванов"));
        manager.AddAppointment(new Appointment(new DateTime(2025, 5, 15, 10, 0, 0), "Петров"));

        manager.AddAppointment(new Appointment(new DateTime(2025, 6, 15, 11, 0, 0), "Сидоров"));

        manager.PrintSchedule();
    }
}
