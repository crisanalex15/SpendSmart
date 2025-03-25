using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Valoarea este obligatorie")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valoarea trebuie să fie mai mare decât 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [StringLength(200, ErrorMessage = "Descrierea nu poate depăși 200 de caractere")]
        public string Description { get; set; }
    }
}
