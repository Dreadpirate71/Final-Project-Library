using LibraryAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public interface IBookDao
    {
        
        public void GetBooks()
        {
            Console.WriteLine("Inside IBookDao GetBooks");
        }
    }
}
