using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using boilerplate_desktop_dotnet.Models;
using boilerplate_desktop_dotnet.Utilities;

namespace boilerplate_desktop_dotnet.Controllers
{
    // internal class RoleController
    // {
    // }

    public static class RoleController
    {
        public static List<Role> GetAllRoles()
        {
            var roles = new List<Role>();
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, name, description FROM roles ORDER BY name";
                    
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error getting all roles", ex);
            }
            
            return roles;
        }
        
        public static Role GetRoleById(int id)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, name, description FROM roles WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Role
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting role by ID {id}", ex);
            }
            
            return null;
        }
        
        public static bool CreateRole(Role role)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO roles (name, description) VALUES (@name, @description)";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", role.Name);
                        command.Parameters.AddWithValue("@description", (object)role.Description ?? DBNull.Value);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"Role {role.Name} created successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating role {role.Name}", ex);
            }
            
            return false;
        }
        
        public static bool UpdateRole(Role role)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE roles SET name = @name, description = @description WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", role.Id);
                        command.Parameters.AddWithValue("@name", role.Name);
                        command.Parameters.AddWithValue("@description", (object)role.Description ?? DBNull.Value);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"Role {role.Name} updated successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating role {role.Name}", ex);
            }
            
            return false;
        }
        
        public static bool DeleteRole(int id)
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
                            string deleteUserRolesQuery = "DELETE FROM user_has_roles WHERE role_id = @id";
                            using (var command = new SqlCommand(deleteUserRolesQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", id);
                                command.ExecuteNonQuery();
                            }
                            
                            // Delete role
                            string deleteRoleQuery = "DELETE FROM roles WHERE id = @id";
                            using (var command = new SqlCommand(deleteRoleQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", id);
                                int result = command.ExecuteNonQuery();
                                
                                if (result > 0)
                                {
                                    transaction.Commit();
                                    Logger.LogInfo($"Role with ID {id} deleted successfully");
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
                Logger.LogError($"Error deleting role with ID {id}", ex);
            }
            
            return false;
        }
        
        public static List<UserRole> GetUserRoles()
        {
            var userRoles = new List<UserRole>();
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT uhr.id, uhr.user_id, uhr.role_id, u.username, r.name as role_name
                        FROM user_has_roles uhr
                        INNER JOIN users u ON uhr.user_id = u.id
                        INNER JOIN roles r ON uhr.role_id = r.id
                        ORDER BY u.username, r.name";
                    
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userRoles.Add(new UserRole
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                UserName = reader.GetString(reader.GetOrdinal("username")),
                                RoleName = reader.GetString(reader.GetOrdinal("role_name"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error getting user roles", ex);
            }
            
            return userRoles;
        }
        
        public static bool AssignRoleToUser(int userId, int roleId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO user_has_roles (user_id, role_id) VALUES (@userId, @roleId)";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@roleId", roleId);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"Role {roleId} assigned to user {userId} successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error assigning role {roleId} to user {userId}", ex);
            }
            
            return false;
        }
        
        public static bool RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM user_has_roles WHERE user_id = @userId AND role_id = @roleId";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@roleId", roleId);
                        
                        int result = command.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            Logger.LogInfo($"Role {roleId} removed from user {userId} successfully");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error removing role {roleId} from user {userId}", ex);
            }
            
            return false;
        }
    }
}
