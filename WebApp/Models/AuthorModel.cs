﻿using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string PhotoName { get; set; }

        public IEnumerable<BookModel> Books { get; set; }
    }
}
