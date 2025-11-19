using boilerplate_desktop_dotnet.Models;

namespace boilerplate_desktop_dotnet.Utilities
{
    public static class SessionManager
    {
        public static User CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;
        public static bool IsAdmin { get; private set; }

        public static void SetCurrentUser(User user, bool isAdmin = false)
        {
            CurrentUser = user;
            IsAdmin = isAdmin;
        }

        public static void ClearSession()
        {
            CurrentUser = null;
            IsAdmin = false;
        }
    }
}