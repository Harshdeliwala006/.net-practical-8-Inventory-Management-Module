using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using Practical_8.Models;

namespace Practical_8.Controllers
{
    public class ProductController : Controller
    {
        private readonly string cs =
            @"Data Source=(localdb)\MSSQLLocalDB;
              Initial Catalog=inventoryDB;
              Integrated Security=True";

        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Products ORDER BY ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = Convert.ToInt32(dr["ProductId"]),
                            ProductName = dr["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(dr["Quantity"]),
                            Price = Convert.ToDecimal(dr["Price"])
                        });
                    }
                }
            }

            return View(products);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product p)
        {
            if (!ModelState.IsValid)
                return View(p);

            string query = @"INSERT INTO Products
                 (ProductName, Quantity, Price)
                 VALUES
                 (@ProductName, @Quantity, @Price)";

            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(txtQuantity.Text));
                    cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(txtPrice.Text));

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Product p = null;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query =
                    "SELECT * FROM Products WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", id);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        p = new Product
                        {
                            ProductId = Convert.ToInt32(dr["ProductId"]),
                            ProductName = dr["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(dr["Quantity"]),
                            Price = Convert.ToDecimal(dr["Price"])
                        };
                    }
                }
            }

            if (p == null)
                return HttpNotFound();

            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product p)
        {
            if (!ModelState.IsValid)
                return View(p);

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = @"
                    UPDATE Products
                    SET ProductName = @ProductName,
                        Quantity = @Quantity,
                        Price = @Price
                    WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ProductId", p.ProductId);
                cmd.Parameters.AddWithValue("@ProductName", p.ProductName);
                cmd.Parameters.AddWithValue("@Quantity", p.Quantity);
                cmd.Parameters.AddWithValue("@Price", p.Price);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM Products WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}