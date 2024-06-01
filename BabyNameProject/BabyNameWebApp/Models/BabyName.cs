using System;

namespace BabyNameWebApp.Models
{
    public class BabyName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}