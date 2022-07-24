namespace eCommerce.API.Models
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        // Vários Departamentos tem vários Usuários.
        // Ou seja, um relacionamento de Muitos para Muitos.
        // Como a tabela de relacionamento "UsuariosDepartamentos"
        // não tem mais campos além de seus relacionamentos,
        // não houve necessidade de criar uma nova classe de modelo.
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
