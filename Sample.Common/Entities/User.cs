using Sample.Common.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Common.Entities
{
    public class User : BaseEntity<int>
    {
        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string? FirstName { get; set; }

        [MaxLength(255)]
        public string? Lastname { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
