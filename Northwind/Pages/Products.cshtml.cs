using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

public class ProductsModel : PageModel
{
    public List<Product> Products { get; set; }

    public void OnGet()
    {
        Products = new List<Product>();
        string connectionString = "Server=localhost;Database=Northwind;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;";
       
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = @"SELECT p.ProductName, c.CategoryName, p.UnitPrice
                           FROM Products p
                           JOIN Categories c ON p.CategoryID = c.CategoryID";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products.Add(new Product
                        {
                            ProductName = reader.GetString(0),
                            CategoryName = reader.GetString(1),
                            UnitPrice = reader.GetDecimal(2)
                        });
                    }
                }
            }
        }
    }
}

public class Product
{
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public decimal UnitPrice { get; set; }
}