using Dapper.Contrib.Extensions;

namespace eCommerce.API.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Sexo { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string NomeMae { get; set; }
        public string SituacaoCadastro { get; set; }
        public DateTimeOffset DataCadastro { get; set; }

        // Um Usuário tem um Contato associado ou composto a ele
        // Ou seja, Um relacionamento de Um para Um por meio de composição        
        [Write(false)]
        public Contato? Contato { get; set; }

        // Um Usuário tem vários Endereços
        // Ou seja, um relacionamento de Um para Muitos
        [Write(false)]
        public ICollection<EnderecoEntrega>? EnderecosEntrega { get; set; }

        // Vários Usuários tem vários Departamentos
        // Ou seja, um relacionamento de Muitos para Muitos
        [Write(false)]
        public ICollection<Departamento>? Departamentos { get; set; }
    }
}
