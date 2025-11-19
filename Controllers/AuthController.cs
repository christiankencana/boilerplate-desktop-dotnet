using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using boilerplate_desktop_dotnet.Models;
using boilerplate_desktop_dotnet.Services;
using boilerplate_desktop_dotnet.Utilities;

namespace boilerplate_desktop_dotnet.Controllers
{
    // internal class AuthController
    // {
    // }

    public static class AuthController
    {
        public static User Login(string username, string password)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT u.id, u.username, u.email, u.password_hash, u.full_name, u.created_at, u.last_login
                        FROM users u 
                        WHERE u.username = @username OR u.email = @username";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Username = reader.GetString(reader.GetOrdinal("username")),
                                    Email = reader.GetString(reader.GetOrdinal("email")),
                                    PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                                    FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString(reader.GetOrdinal("full_name")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                    LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("last_login"))
                                };
                                
                                if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
                                {
                                    UpdateLastLogin(user.Id);
                                    Logger.LogInfo($"User {username} logged in successfully");
                                    return user;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Login error for user {username}", ex);
            }
            
            return null;
        }
        
        public static bool Register(string username, string email, string password, string fullName = null)
        {
            try
            {
                if (IsUserExists(username, email))
                {
                    return false;
                }
                
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO users (username, email, password_hash, full_name) 
                        VALUES (@username, @email, @passwordHash, @fullName)";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@passwordHash", PasswordHasher.HashPassword(password));
                        command.Parameters.AddWithValue("@fullName", (object)fullName ?? DBNull.Value);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"User {username} registered successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Registration error for user {username}", ex);
            }
            
            return false;
        }
        
        public static bool ResetPassword(string username, string newPassword)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        UPDATE users 
                        SET password_hash = @passwordHash 
                        WHERE username = @username OR email = @username";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@passwordHash", PasswordHasher.HashPassword(newPassword));
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"Password reset for user {username}");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Password reset error for user {username}", ex);
            }
            
            return false;
        }
        
        public static bool IsUserAdmin(int userId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM user_has_roles uhr 
                        INNER JOIN roles r ON uhr.role_id = r.id 
                        WHERE uhr.user_id = @userId AND r.name = 'Admin'";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error checking admin status for user {userId}", ex);
            }
            
            return false;
        }
        
        private static bool IsUserExists(string username, string email)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@email", email);
                        
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error checking user existence", ex);
                return true; // Return true to prevent registration on error
            }
        }
        
        private static void UpdateLastLogin(int userId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE users SET last_login = GETDATE() WHERE id = @userId";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating last login for user {userId}", ex);
            }
        }
    }
}
