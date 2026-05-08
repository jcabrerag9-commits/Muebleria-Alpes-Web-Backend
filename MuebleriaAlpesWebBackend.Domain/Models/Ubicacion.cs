using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class Pais
    {
        public int Id { get; set; }
        
        [StringLength(10)]
        public string? Codigo { get; set; }
        
        [Required, StringLength(100)]
        public string Nombre { get; set; }
        
        public string Estado { get; set; } = "ACTIVO";
    }

    public class Departamento
    {
        public int Id { get; set; }
        
        [Required]
        public int PaisId { get; set; }
        
        [StringLength(10)]
        public string? Codigo { get; set; }
        
        [Required, StringLength(100)]
        public string Nombre { get; set; }
        
        public string Estado { get; set; } = "ACTIVO";
    }

    public class Ciudad
    {
        public int Id { get; set; }
        
        [Required]
        public int DepartamentoId { get; set; }
        
        [StringLength(10)]
        public string? Codigo { get; set; }
        
        [Required, StringLength(100)]
        public string Nombre { get; set; }
        
        public string Estado { get; set; } = "ACTIVO";
    }

    public class Idioma
    {
        public int Id { get; set; }
        
        [StringLength(5)]
        public string? Codigo { get; set; }
        
        [Required, StringLength(50)]
        public string Nombre { get; set; }
        
        public string Estado { get; set; } = "ACTIVO";
    }

    public class Moneda
    {
        public int Id { get; set; }
        
        [StringLength(5)]
        public string? Codigo { get; set; }
        
        [Required, StringLength(50)]
        public string Nombre { get; set; }
        
        [Required, StringLength(5)]
        public string Simbolo { get; set; }
        
        public string Estado { get; set; } = "ACTIVO";
    }
}
