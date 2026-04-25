using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        
        [Required]
        public int TipoClienteId { get; set; }
        
        [Required]
        public int TipoDocumentoId { get; set; }
        
        public string Codigo { get; set; } // Generado por DB
        
        [Required, StringLength(50)]
        public string NumeroDocumento { get; set; }
        
        [Required, StringLength(100)]
        public string PrimerNombre { get; set; }
        
        public string SegundoNombre { get; set; }
        
        [Required, StringLength(100)]
        public string PrimerApellido { get; set; }
        
        public string SegundoApellido { get; set; }
        
        public string RazonSocial { get; set; }
        
        public DateTime? FechaNacimiento { get; set; }
        
        [RegularExpression("^[MFO]$")]
        public string Genero { get; set; }
        
        public string PasswordPlano { get; set; } // Solo para creación
        
        public string Estado { get; set; } = "ACTIVO";
    }

    public class ClienteEmail
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        public string EsPrincipal { get; set; } = "N";
        public string Estado { get; set; } = "NO_VERIFICADO";
    }

    public class ClienteTelefono
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        
        [Required]
        public string Tipo { get; set; } // CELULAR, CASA, TRABAJO
        
        [Required, Phone]
        public string Numero { get; set; }
        
        public string EsPrincipal { get; set; } = "N";
        public string Estado { get; set; } = "ACTIVO";
    }

    public class ClienteDireccion
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int PaisId { get; set; }
        public int DepartamentoId { get; set; }
        public int CiudadId { get; set; }
        
        [Required]
        public string Tipo { get; set; } // FACTURACION, ENVIO
        
        [Required]
        public string DireccionLinea1 { get; set; }
        public string DireccionLinea2 { get; set; }
        public string CodigoPostal { get; set; }
        public string Referencia { get; set; }
        public string EsPrincipal { get; set; } = "N";
        public string Estado { get; set; } = "ACTIVO";
    }

    public class ClientePreferencia
    {
        public int ClienteId { get; set; }
        public int? IdiomaId { get; set; }
        public int? MonedaId { get; set; }
        public string AceptaMarketing { get; set; } = "N";
        public string AceptaSms { get; set; } = "N";
        public string AceptaEmail { get; set; } = "S";
    }

    // DTO para vista detallada del cliente
    public class ClienteDetalleDto : Cliente
    {
        public string NombreCompleto { get; set; }
        public string TipoClienteNombre { get; set; }
        public string TipoDocumentoNombre { get; set; }
        public List<ClienteEmail> Emails { get; set; } = new();
        public List<ClienteTelefono> Telefonos { get; set; } = new();
        public List<ClienteDireccion> Direcciones { get; set; } = new();
        public ClientePreferencia Preferencias { get; set; }
    }
}
