using System.ComponentModel.DataAnnotations;
using WebPBL3.Models;

namespace WebPBL3.Models
{
    public class Make
    {   
       
        
        [Key]
        [Display(Name = "Mã hãng")]
        public int MakeID { get; set; }

        [Display(Name = "Tên hãng")]
        [StringLength(maximumLength: 50)]
        public string MakeName { get; set; } = string.Empty;

        // colect navigation
        public List<Car>? Cars { get; set; }

    }
}