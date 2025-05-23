using C__Day4.Interfaces;
using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Repositories
{
    public static class ProductDataStore
    {
        public static List<Product> Products { get; } = new List<Product>();
    }
}
