using Microsoft.JSInterop;
using System.Text.Json;

namespace EventEaseApp.Services
{
    public class AttendanceService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly UserSessionService _userSessionService;
        private const string ATTENDANCE_KEY = "eventease_attendance";

        public AttendanceService(IJSRuntime jsRuntime, UserSessionService userSessionService)
        {
            _jsRuntime = jsRuntime;
            _userSessionService = userSessionService;
        }

        // Models
        public class EventRegistration
        {
            public string RegistrationId { get; set; } = Guid.NewGuid().ToString();
            public string UserId { get; set; } = string.Empty;
            public string EventId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string UserEmail { get; set; } = string.Empty;
            public string UserPhone { get; set; } = string.Empty;
            public DateTime RegistrationDate { get; set; } = DateTime.Now;
            public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;
            public string? SpecialRequests { get; set; }
            public bool EmailNotifications { get; set; } = true;
        }

        public class AttendanceRecord
        {
            public string AttendanceId { get; set; } = Guid.NewGuid().ToString();
            public string RegistrationId { get; set; } = string.Empty;
            public string EventId { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public DateTime CheckInTime { get; set; }
            public DateTime? CheckOutTime { get; set; }
            public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
            public string? Notes { get; set; }
        }

        public enum RegistrationStatus
        {
            Registered,
            Confirmed,
            Cancelled,
            Waitlisted
        }

        public enum AttendanceStatus
        {
            Present,
            Absent,
            Late,
            LeftEarly
        }

        // Events
        public event Action<EventRegistration>? OnRegistrationCreated;
        public event Action<AttendanceRecord>? OnAttendanceRecorded;

        // Registration methods
        public async Task<EventRegistration> RegisterForEventAsync(
            string eventId, 
            string userName, 
            string userEmail, 
            string userPhone = "", 
            string? specialRequests = null,
            bool emailNotifications = true)
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            var userId = session?.UserId ?? Guid.NewGuid().ToString();

            var registration = new EventRegistration
            {
                UserId = userId,
                EventId = eventId,
                UserName = userName,
                UserEmail = userEmail,
                UserPhone = userPhone,
                SpecialRequests = specialRequests,
                EmailNotifications = emailNotifications,
                RegistrationDate = DateTime.Now,
                Status = RegistrationStatus.Registered
            };

            // Save registration
            await SaveRegistrationAsync(registration);

            // Update user session with registered event
            await _userSessionService.AddRegisteredEventAsync(eventId);

            OnRegistrationCreated?.Invoke(registration);
            return registration;
        }

        public async Task<List<EventRegistration>> GetUserRegistrationsAsync()
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            if (session == null) return new List<EventRegistration>();

            var allRegistrations = await GetAllRegistrationsAsync();
            return allRegistrations.Where(r => r.UserId == session.UserId).ToList();
        }

        public async Task<List<EventRegistration>> GetEventRegistrationsAsync(string eventId)
        {
            var allRegistrations = await GetAllRegistrationsAsync();
            return allRegistrations.Where(r => r.EventId == eventId).ToList();
        }

        public async Task<EventRegistration?> GetRegistrationAsync(string eventId, string userId)
        {
            var allRegistrations = await GetAllRegistrationsAsync();
            return allRegistrations.FirstOrDefault(r => r.EventId == eventId && r.UserId == userId);
        }

        public async Task<bool> IsUserRegisteredForEventAsync(string eventId)
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            if (session == null) return false;

            var registration = await GetRegistrationAsync(eventId, session.UserId);
            return registration != null && registration.Status != RegistrationStatus.Cancelled;
        }

        public async Task<bool> CancelRegistrationAsync(string eventId)
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            if (session == null) return false;

            var allRegistrations = await GetAllRegistrationsAsync();
            var registration = allRegistrations.FirstOrDefault(r => r.EventId == eventId && r.UserId == session.UserId);
            
            if (registration != null)
            {
                registration.Status = RegistrationStatus.Cancelled;
                await SaveAllRegistrationsAsync(allRegistrations);
                return true;
            }

            return false;
        }

        // Attendance tracking methods
        public async Task<AttendanceRecord> CheckInAsync(string eventId, string? notes = null)
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            if (session == null) throw new InvalidOperationException("User not logged in");

            var registration = await GetRegistrationAsync(eventId, session.UserId);
            if (registration == null) throw new InvalidOperationException("User not registered for this event");

            var attendance = new AttendanceRecord
            {
                RegistrationId = registration.RegistrationId,
                EventId = eventId,
                UserId = session.UserId,
                CheckInTime = DateTime.Now,
                Status = AttendanceStatus.Present,
                Notes = notes
            };

            await SaveAttendanceAsync(attendance);
            OnAttendanceRecorded?.Invoke(attendance);
            return attendance;
        }

        public async Task<bool> CheckOutAsync(string eventId, string? notes = null)
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            if (session == null) return false;

            var allAttendance = await GetAllAttendanceAsync();
            var attendance = allAttendance.FirstOrDefault(a => a.EventId == eventId && a.UserId == session.UserId && a.CheckOutTime == null);
            
            if (attendance != null)
            {
                attendance.CheckOutTime = DateTime.Now;
                if (!string.IsNullOrEmpty(notes))
                    attendance.Notes = attendance.Notes + " | Checkout: " + notes;
                
                await SaveAllAttendanceAsync(allAttendance);
                return true;
            }

            return false;
        }

        public async Task<List<AttendanceRecord>> GetEventAttendanceAsync(string eventId)
        {
            var allAttendance = await GetAllAttendanceAsync();
            return allAttendance.Where(a => a.EventId == eventId).ToList();
        }

        public async Task<AttendanceRecord?> GetUserAttendanceAsync(string eventId)
        {
            var session = await _userSessionService.GetCurrentSessionAsync();
            if (session == null) return null;

            var allAttendance = await GetAllAttendanceAsync();
            return allAttendance.FirstOrDefault(a => a.EventId == eventId && a.UserId == session.UserId);
        }

        // Statistics methods
        public async Task<int> GetEventRegistrationCountAsync(string eventId)
        {
            var registrations = await GetEventRegistrationsAsync(eventId);
            return registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
        }

        public async Task<int> GetEventAttendanceCountAsync(string eventId)
        {
            var attendance = await GetEventAttendanceAsync(eventId);
            return attendance.Count(a => a.Status == AttendanceStatus.Present);
        }

        public async Task<double> GetEventAttendanceRateAsync(string eventId)
        {
            var registrationCount = await GetEventRegistrationCountAsync(eventId);
            var attendanceCount = await GetEventAttendanceCountAsync(eventId);
            
            return registrationCount > 0 ? (double)attendanceCount / registrationCount * 100 : 0;
        }

        // Storage methods
        private async Task SaveRegistrationAsync(EventRegistration registration)
        {
            var allRegistrations = await GetAllRegistrationsAsync();
            allRegistrations.Add(registration);
            await SaveAllRegistrationsAsync(allRegistrations);
        }

        private async Task<List<EventRegistration>> GetAllRegistrationsAsync()
        {
            try
            {
                var registrationsJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", ATTENDANCE_KEY + "_registrations");
                if (!string.IsNullOrEmpty(registrationsJson))
                {
                    return JsonSerializer.Deserialize<List<EventRegistration>>(registrationsJson) ?? new List<EventRegistration>();
                }
            }
            catch (Exception)
            {
                // Handle JSON deserialization errors
            }
            
            return new List<EventRegistration>();
        }

        private async Task SaveAllRegistrationsAsync(List<EventRegistration> registrations)
        {
            var registrationsJson = JsonSerializer.Serialize(registrations);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ATTENDANCE_KEY + "_registrations", registrationsJson);
        }

        private async Task SaveAttendanceAsync(AttendanceRecord attendance)
        {
            var allAttendance = await GetAllAttendanceAsync();
            allAttendance.Add(attendance);
            await SaveAllAttendanceAsync(allAttendance);
        }

        private async Task<List<AttendanceRecord>> GetAllAttendanceAsync()
        {
            try
            {
                var attendanceJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", ATTENDANCE_KEY + "_records");
                if (!string.IsNullOrEmpty(attendanceJson))
                {
                    return JsonSerializer.Deserialize<List<AttendanceRecord>>(attendanceJson) ?? new List<AttendanceRecord>();
                }
            }
            catch (Exception)
            {
                // Handle JSON deserialization errors
            }
            
            return new List<AttendanceRecord>();
        }

        private async Task SaveAllAttendanceAsync(List<AttendanceRecord> attendance)
        {
            var attendanceJson = JsonSerializer.Serialize(attendance);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ATTENDANCE_KEY + "_records", attendanceJson);
        }
    }
}