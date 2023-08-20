﻿using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Token { get; set; }

        public string? Role { get; set; }
    }
}
