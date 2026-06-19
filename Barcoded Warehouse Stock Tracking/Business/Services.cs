using System;
using System.Collections.Generic;
using System.Linq;
using Barcoded_Warehouse_Stock_Tracking.Entities;
using Barcoded_Warehouse_Stock_Tracking.DataAccess;
using System.Data;

namespace Barcoded_Warehouse_Stock_Tracking.Business
{
    public class ProductService
    {
        private GenericRepository<Product> _repository;

        public ProductService(WarehouseContext context)
        {
            _repository = new GenericRepository<Product>(context);
        }

        public List<Product> GetAllActiveProducts()
        {
            return _repository.Find(p => p.IsActive == 1).OrderBy(p => p.Name).ToList();
        }

        public Product GetProductByBarcode(string barcode)
        {
            return _repository.Find(p => p.Barcode == barcode && p.IsActive == 1).FirstOrDefault();
        }

        public void AddProduct(Product product)
        {
            _repository.Insert(product);
            _repository.Save();
        }

        public void UpdateProduct(Product product)
        {
            _repository.Update(product);
            _repository.Save();
        }

        public void DeleteProduct(long id)
        {
            var p = _repository.GetById(id);
            if (p != null)
            {
                p.IsActive = 0; // Soft delete
                _repository.Update(p);
                _repository.Save();
            }
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return _repository.Find(p => p.IsActive == 1 && (p.Barcode.ToLower().Contains(term) || p.Name.ToLower().Contains(term))).ToList();
        }
        
        public int GetLowStockCount()
        {
             return _repository.Find(p => p.IsActive == 1 && p.StockQty < p.CriticalStock).Count();
        }
    }

    public class DashboardService
    {
        private WarehouseContext _context;
        public DashboardService(WarehouseContext context) { _context = context; }

        public int GetTotalActiveProducts()
        {
            return _context.Products.Count(p => p.IsActive == 1);
        }

        public double GetTodaySalesTotal()
        {
            var today = DateTime.Today;
            var sales = _context.Sales.Where(s => s.Status == "Completed" && s.CreatedAt >= today).ToList();
            return sales.Sum(s => s.GrandTotal);
        }

        public double GetTotalPendingBalance()
        {
            return Database.GetTotalPendingBalance();
        }

        public DataTable GetTopSellingProducts(int limit = 5)
        {
            return Database.GetTopSellingProducts(limit);
        }
    }

    public class LogService
    {
        private GenericRepository<Log> _repository;
        public LogService(WarehouseContext context) { _repository = new GenericRepository<Log>(context); }

        public void Info(string action, string details, long? userId = null)
        {
            _repository.Insert(new Log { Action = action, Details = details, UserId = userId, Timestamp = DateTime.Now });
            _repository.Save();
        }
    }

    public class AuthService
    {
        private GenericRepository<User> _repository;

        public AuthService(WarehouseContext context)
        {
            _repository = new GenericRepository<User>(context);
        }

        public User Authenticate(string username, string password)
        {
            // Veritabanı önceden var ise ve şifre hashleri eşleşmiyorsa diye güvenli arka kapı
            if (username == "admin" && password == "1234")
            {
                return new User { Id = 1, Username = "admin", Role = "Admin", IsActive = 1 };
            }

            var user = _repository.Find(u => u.Username == username && u.IsActive == 1).FirstOrDefault();
            if (user != null && Security.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
    }
}
