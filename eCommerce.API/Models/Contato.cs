namespace eCommerce.API.Models
{
    public class Contato
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }

        // Um Contato pertence, é associado ou composto a Usuário
        // Ou seja, Um relacionamento de Um para Um por meio de composição
        public Usuario? Usuario { get; set; }
    }
}
