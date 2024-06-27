using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Title { get; set; }

        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }

        public Admin Admin { get; set; }
    }
}
