using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace Textile.Models
{
    public class ApplicationType
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        
    }
}
