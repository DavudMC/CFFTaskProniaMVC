using System.ComponentModel.DataAnnotations;
using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.Entities
{
    public class InfoCard:BaseEntity
    {
        
        //[MaxLength(20)]
        //[MinLength(3)]
        //[Required]
        public string Title { get; set; } 
        public string Description { get; set; } 
        //[Required]
        public string ImagePath { get; set; } 
    }
}
