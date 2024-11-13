   using Microsoft.AspNetCore.Mvc;
   using Microsoft.AspNetCore.Mvc.RazorPages;
   using Microsoft.AspNetCore.Mvc.Rendering;
   using Microsoft.Extensions.Configuration;
   using System.Data.SqlClient;
 
   namespace Northwind.Pages
   {
       public class EmployeeManagementModel : PageModel
       {
           //Connection string for the database connection
           private readonly string _connectionString;
        //Create a new model configration and setting the connection string
           public EmployeeManagementModel(IConfiguration configuration)
           {
               _connectionString = configuration.GetConnectionString("NorthwindConnection");
           }
 
           //Binding the form fields to the data we retrieve
           [BindProperty]
           public int? SelectedEmployeeId { get; set; }
 
           public List<SelectListItem> EmployeeList { get; set; }
           public dynamic SelectedEmployee { get; set; }
 
           //Populates the employee list when the page loads
           //Get = HTTP request 
           public void OnGet()
           {
               LoadEmployeeList();
           }
 
           //OnePost is called when the user submits the form
           public IActionResult OnPost()
           {
               //Update the employee list
               LoadEmployeeList();
               //If the user selected an employee
               if (SelectedEmployeeId.HasValue)
               {
                   //Gets the employee record from the database based on the
                   //employee ID that the user selected in the dropdown
                   SelectedEmployee = GetEmployeeById(SelectedEmployeeId.Value);
               }

               //Returns the page 
               return Page();
           }
 
           //This method is called when the user has changed the employee information
           //in the form and clicked the Update button
           public IActionResult OnPostUpdate(int EmployeeID, string Title, string City)
           {
               //Calls update employee to actually update the record in the database
               UpdateEmployee(EmployeeID, Title, City);
               //Redirect back to the same web page
               return RedirectToPage();
           }
 
           //Called when the user clicks the delete button
           public IActionResult OnPostDelete(int EmployeeID)
           {
               //Deletes the employee record from the database based on the
               //employee id that the user selected
               DeleteEmployee(EmployeeID);
               return RedirectToPage();
           }
 
           //Method that is called when the user clicks the add employee button
           public IActionResult OnPostAdd(string NewFirstName, string NewLastName, string NewTitle)
           {
               //This method will insert the new employee record into the database
               AddEmployee(NewFirstName, NewLastName, NewTitle);
               return RedirectToPage();
           }
 
           //Gets the list of employees from the employees table in the Northwind database
           private void LoadEmployeeList()
           {
               //Create a new List
               EmployeeList = new List<SelectListItem>();

               //Create a new database connection
               using (SqlConnection connection = new SqlConnection(_connectionString))
               {
                   //Open the connection to the database  - to the SQL Server and the
                   //Northwind database that is a part of our SQL Server
                   connection.Open();
                   //SQL command to run
                   string sql = "SELECT EmployeeID, FirstName, LastName FROM Employees";

                
                   using (SqlCommand command = new SqlCommand(sql, connection))
                   {
                       using (SqlDataReader reader = command.ExecuteReader())
                       {
                           //Execute the SQL data reader and loop through the collection 
                           //of records that is returned from the database
                           while (reader.Read())
                           {
                               //Add the employee to the list
                               //Create a new SelectListItem and add that object to 
                               //the dropdown list of employees
                               EmployeeList.Add(new SelectListItem
                               {
                                   //Value and the text
                                   //Value is the employee ID (primary key for the employees table - 
                                   //so it uniquely identifies the record)
                                   //Text that is actually displayed in the list
                                   //is the employee firstname and lastname
                                   Value = reader["EmployeeID"].ToString(),
                                   Text = $"{reader["FirstName"]} {reader["LastName"]}"
                               });
                           }
                       }
                   }
               }
           }
 
           //Create a connection to the database and retrieve an employee record
           //based on the employee ID
           private dynamic GetEmployeeById(int id)
           {
               using (SqlConnection connection = new SqlConnection(_connectionString))
               {
                   connection.Open();
                   //SQL statement to execute against the database
                   string sql = "SELECT EmployeeID, FirstName, LastName, Title, City FROM Employees WHERE EmployeeID = @Id";
                   using (SqlCommand command = new SqlCommand(sql, connection))
                   {
                       //Add a parameter to the command (a variable value)
                       //The parameter is the employee ID
                       command.Parameters.AddWithValue("@Id", id);
                       using (SqlDataReader reader = command.ExecuteReader())
                       {
                           //Execute the reader to read the data from the database
                           //Reader.Read() will return false if there are no
                           //records returned - if there is no employee with that ID
                           if (reader.Read())
                           {
                            //Set the attributes for our form based on the values that
                            //were in the record from the database
                               return new
                               {
                                   EmployeeID = (int)reader["EmployeeID"],
                                   FirstName = reader["FirstName"].ToString(),
                                   LastName = reader["LastName"].ToString(),
                                   Title = reader["Title"].ToString(),
                                   City = reader["City"].ToString()
                               };
                           }
                       }
                   }
               }
               return null;
           }
 
            //Create a connection to the database and update the employee record
            //based on the employee ID
            private void UpdateEmployee(int employeeId, string title, string city)
           {
               using (SqlConnection connection = new SqlConnection(_connectionString))
               {
                   connection.Open();
                   string sql = "UPDATE Employees SET Title = @Title, City = @City WHERE EmployeeID = @EmployeeID";
                   //Execute the SQL update command and add the parameters - populating them from the form
                   using (SqlCommand command = new SqlCommand(sql, connection))
                   {
                       command.Parameters.AddWithValue("@EmployeeID", employeeId);
                       command.Parameters.AddWithValue("@Title", title);
                       command.Parameters.AddWithValue("@City", city);
                       command.ExecuteNonQuery();
                   }
               }
           }

           //Create a database connection and delete an employee record based on the ID
           private void DeleteEmployee(int id)
           {
               using (SqlConnection connection = new SqlConnection(_connectionString))
               {
                   connection.Open();
                   //SQL command to execute
                   string sql = "DELETE FROM Employees WHERE EmployeeID = @Id";
                   using (SqlCommand command = new SqlCommand(sql, connection))
                   {
                       //Execute the SQL command and add a parameter that contains our employee id from the page
                       command.Parameters.AddWithValue("@Id", id);
                       command.ExecuteNonQuery();
                   }
               }
           }
 
           //Create a connection and execute a SQL command to insert a new
           //employee record in the employees table
           private void AddEmployee(string firstName, string lastName, string title)
           {
               using (SqlConnection connection = new SqlConnection(_connectionString))
               {
                   connection.Open();
                   //SQL statement to execute
                   string sql = @"INSERT INTO Employees (FirstName, LastName, Title, BirthDate, HireDate, Address, City, Country)
                                  VALUES (@FirstName, @LastName, @Title, '1980-01-01', GETDATE(), '123 Main St', 'Anytown', 'USA')";
                   using (SqlCommand command = new SqlCommand(sql, connection))
                   {
                       //Execute the SQL command and adding the values from the webpage as parameters
                       command.Parameters.AddWithValue("@FirstName", firstName);
                       command.Parameters.AddWithValue("@LastName", lastName);
                       command.Parameters.AddWithValue("@Title", title);
                       command.ExecuteNonQuery();
                   }
               }
           }
       }
   }
 

