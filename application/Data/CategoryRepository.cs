using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using application.Models;
using System.IO;

namespace application.Data
{
    public class CategoryRepository
    {
        private readonly string _connectionString;
        private readonly string _dbPath = "quiz_database.db";
        
        public CategoryRepository()
        {
            // Create database directory if it doesn't exist
            string dbDirectory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
            
            _connectionString = $"Data Source={_dbPath};";
            EnsureDatabase();
        }
        
        private void EnsureDatabase()
        {
            bool dbExists = File.Exists(_dbPath);
            
            if (!dbExists)
            {
                // SQLite file will be created automatically when first accessed
                // SqliteConnection equivalent will create the file if it doesn't exist
                File.Create(_dbPath).Close();
            }
            
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                // Create the Categories table if it doesn't exist
                string createCategoriesTable = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT
                    );";
                
                using (var command = new SqliteCommand(createCategoriesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                
                // Create the Questions table if it doesn't exist
                string createQuestionsTable = @"
                    CREATE TABLE IF NOT EXISTS Questions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CategoryId INTEGER NOT NULL,
                        Text TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        FOREIGN KEY (CategoryId) REFERENCES Categories (Id) ON DELETE CASCADE
                    );";
                
                using (var command = new SqliteCommand(createQuestionsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                
                // Create the AnswerOptions table if it doesn't exist
                string createAnswerOptionsTable = @"
                    CREATE TABLE IF NOT EXISTS AnswerOptions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        QuestionId INTEGER NOT NULL,
                        Text TEXT NOT NULL,
                        IsCorrect INTEGER NOT NULL,
                        FOREIGN KEY (QuestionId) REFERENCES Questions (Id) ON DELETE CASCADE
                    );";
                
                using (var command = new SqliteCommand(createAnswerOptionsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                string query = "SELECT Id, Name, CreatedAt, UpdatedAt FROM Categories ORDER BY Name";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var category = new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                            };
                            
                            if (!reader.IsDBNull(reader.GetOrdinal("UpdatedAt")))
                            {
                                category.UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());
                            }
                            
                            categories.Add(category);
                        }
                    }
                }
            }
            
            return categories;
        }
        
        public Category GetCategoryById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                string query = "SELECT Id, Name, CreatedAt, UpdatedAt FROM Categories WHERE Id = @Id";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var category = new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                            };
                            
                            if (!reader.IsDBNull(reader.GetOrdinal("UpdatedAt")))
                            {
                                category.UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());
                            }
                            
                            return category;
                        }
                    }
                }
            }
            
            return null;
        }
        
        public int AddCategory(Category category)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                string query = @"
                    INSERT INTO Categories (Name, CreatedAt)
                    VALUES (@Name, @CreatedAt);
                    SELECT last_insert_rowid();";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@CreatedAt", category.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        
        public void UpdateCategory(Category category)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                category.UpdatedAt = DateTime.Now;
                
                string query = @"
                    UPDATE Categories
                    SET Name = @Name, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", category.Id);
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@UpdatedAt", category.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void DeleteCategory(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                // Begin a transaction to ensure all related data is deleted together
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First delete all answer options for questions in this category
                        string deleteAnswerOptionsQuery = @"
                            DELETE FROM AnswerOptions 
                            WHERE QuestionId IN (SELECT Id FROM Questions WHERE CategoryId = @CategoryId)";
                        
                        using (var command = new SqliteCommand(deleteAnswerOptionsQuery, connection))
                        {
                            command.Parameters.AddWithValue("@CategoryId", id);
                            command.ExecuteNonQuery();
                        }
                        
                        // Then delete all questions in this category
                        string deleteQuestionsQuery = "DELETE FROM Questions WHERE CategoryId = @CategoryId";
                        
                        using (var command = new SqliteCommand(deleteQuestionsQuery, connection))
                        {
                            command.Parameters.AddWithValue("@CategoryId", id);
                            command.ExecuteNonQuery();
                        }
                        
                        // Finally delete the category
                        string deleteCategoryQuery = "DELETE FROM Categories WHERE Id = @CategoryId";
                        
                        using (var command = new SqliteCommand(deleteCategoryQuery, connection))
                        {
                            command.Parameters.AddWithValue("@CategoryId", id);
                            command.ExecuteNonQuery();
                        }
                        
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
