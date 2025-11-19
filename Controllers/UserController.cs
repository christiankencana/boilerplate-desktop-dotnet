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
    // internal class UserController
    // {
    // }

    public static class UserController
    {
        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, email, full_name, created_at, last_login FROM users ORDER BY username";
                    
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString(reader.GetOrdinal("full_name")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("last_login"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error getting all users", ex);
            }
            
            return users;
        }
        
        public static User GetUserById(int id)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, email, full_name, created_at, last_login FROM users WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Username = reader.GetString(reader.GetOrdinal("username")),
                                    Email = reader.GetString(reader.GetOrdinal("email")),
                                    FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString(reader.GetOrdinal("full_name")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                    LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("last_login"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting user by ID {id}", ex);
            }
            
            return null;
        }
        
        public static bool CreateUser(User user, string password)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO users (username, email, password_hash, full_name) 
                        VALUES (@username, @email, @passwordHash, @fullName)";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@passwordHash", PasswordHasher.HashPassword(password));
                        command.Parameters.AddWithValue("@fullName", (object)user.FullName ?? DBNull.Value);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"User {user.Username} created successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating user {user.Username}", ex);
            }
            
            return false;
        }
        
        public static bool UpdateUser(User user)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        UPDATE users 
                        SET username = @username, email = @email, full_name = @fullName 
                        WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", user.Id);
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@fullName", (object)user.FullName ?? DBNull.Value);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"User {user.Username} updated successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating user {user.Username}", ex);
            }
            
            return false;
        }
        
        public static bool DeleteUser(int id)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Delete user roles first
                            string deleteRolesQuery = "DELETE FROM user_has_roles WHERE user_id = @id";
                            using (var command = new SqlCommand(deleteRolesQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", id);
                                command.ExecuteNonQuery();
                            }
                            
                            // Delete user
                            string deleteUserQuery = "DELETE FROM users WHERE id = @id";
                            using (var command = new SqlCommand(deleteUserQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", id);
                                int result = command.ExecuteNonQuery();
                                
                                if (result > 0)
                                {
                                    transaction.Commit();
                                    Logger.LogInfo($"User with ID {id} deleted successfully");
                                    return true;
                                }
                            }
                            
                            transaction.Rollback();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting user with ID {id}", ex);
            }
            
            return false;
        }
        
        public static bool ChangePassword(int userId, string newPassword)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE users SET password_hash = @passwordHash WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", userId);
                        command.Parameters.AddWithValue("@passwordHash", PasswordHasher.HashPassword(newPassword));
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"Password changed for user ID {userId}");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error changing password for user ID {userId}", ex);
            }
            
            return false;
        }
    }
}
