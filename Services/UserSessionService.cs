using Microsoft.JSInterop;
using System.Text.Json;

namespace EventEaseApp.Services
{
    public class UserSessionService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string USER_SESSION_KEY = "eventease_user_session";
        private const string USER_PREFERENCES_KEY = "eventease_user_preferences";

        public UserSessionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // User session model
        public class UserSession
        {
            public string UserId { get; set; } = Guid.NewGuid().ToString();
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public DateTime SessionStart { get; set; } = DateTime.Now;
            public DateTime LastActivity { get; set; } = DateTime.Now;
            public bool IsLoggedIn { get; set; } = false;
            public List<string> RegisteredEvents { get; set; } = new();
        }

        public class UserPreferences
        {
            public string Theme { get; set; } = "light";
            public bool EmailNotifications { get; set; } = true;
            public string Language { get; set; } = "en";
        }

        // Current session
        private UserSession? _currentSession;
        private UserPreferences? _currentPreferences;

        // Events
        public event Action<UserSession?>? OnSessionChanged;
        public event Action<UserPreferences?>? OnPreferencesChanged;

        // Session management
        public async Task<UserSession?> GetCurrentSessionAsync()
        {
            if (_currentSession != null)
                return _currentSession;

            try
            {
                var sessionJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", USER_SESSION_KEY);
                if (!string.IsNullOrEmpty(sessionJson))
                {
                    _currentSession = JsonSerializer.Deserialize<UserSession>(sessionJson);
                    if (_currentSession != null)
                    {
                        _currentSession.LastActivity = DateTime.Now;
                        await SaveSessionAsync(_currentSession);
                    }
                }
            }
            catch (Exception)
            {
                // Handle JSON deserialization errors
                _currentSession = null;
            }

            return _currentSession;
        }

        public async Task<bool> LoginUserAsync(string name, string email, string phoneNumber = "")
        {
            var session = new UserSession
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                IsLoggedIn = true,
                SessionStart = DateTime.Now,
                LastActivity = DateTime.Now
            };

            await SaveSessionAsync(session);
            _currentSession = session;
            OnSessionChanged?.Invoke(_currentSession);
            return true;
        }

        public async Task LogoutUserAsync()
        {
            _currentSession = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_SESSION_KEY);
            OnSessionChanged?.Invoke(null);
        }

        public async Task UpdateSessionAsync(UserSession session)
        {
            session.LastActivity = DateTime.Now;
            await SaveSessionAsync(session);
            _currentSession = session;
            OnSessionChanged?.Invoke(_currentSession);
        }

        private async Task SaveSessionAsync(UserSession session)
        {
            var sessionJson = JsonSerializer.Serialize(session);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_SESSION_KEY, sessionJson);
        }

        // Preferences management
        public async Task<UserPreferences> GetUserPreferencesAsync()
        {
            if (_currentPreferences != null)
                return _currentPreferences;

            try
            {
                var prefsJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", USER_PREFERENCES_KEY);
                if (!string.IsNullOrEmpty(prefsJson))
                {
                    _currentPreferences = JsonSerializer.Deserialize<UserPreferences>(prefsJson);
                }
            }
            catch (Exception)
            {
                // Handle JSON deserialization errors
            }

            _currentPreferences ??= new UserPreferences();
            return _currentPreferences;
        }

        public async Task SaveUserPreferencesAsync(UserPreferences preferences)
        {
            _currentPreferences = preferences;
            var prefsJson = JsonSerializer.Serialize(preferences);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_PREFERENCES_KEY, prefsJson);
            OnPreferencesChanged?.Invoke(_currentPreferences);
        }

        // Utility methods
        public async Task<bool> IsUserLoggedInAsync()
        {
            var session = await GetCurrentSessionAsync();
            return session?.IsLoggedIn ?? false;
        }

        public async Task<string> GetUserNameAsync()
        {
            var session = await GetCurrentSessionAsync();
            return session?.Name ?? "Guest";
        }

        public async Task<string> GetUserEmailAsync()
        {
            var session = await GetCurrentSessionAsync();
            return session?.Email ?? "";
        }

        // Event registration tracking
        public async Task AddRegisteredEventAsync(string eventId)
        {
            var session = await GetCurrentSessionAsync();
            if (session != null && !session.RegisteredEvents.Contains(eventId))
            {
                session.RegisteredEvents.Add(eventId);
                await UpdateSessionAsync(session);
            }
        }

        public async Task<bool> IsRegisteredForEventAsync(string eventId)
        {
            var session = await GetCurrentSessionAsync();
            return session?.RegisteredEvents.Contains(eventId) ?? false;
        }

        public async Task<List<string>> GetRegisteredEventsAsync()
        {
            var session = await GetCurrentSessionAsync();
            return session?.RegisteredEvents ?? new List<string>();
        }
    }
}