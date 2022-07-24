using Dapper.FluentMap.Mapping;
using eCommerce.API.Models;

namespace eCommerce.API.Mappers
{
    public class UsuarioDiferenteMap : EntityMap<UsuarioDiferente>
    {
        public UsuarioDiferenteMap()
        {
            Map(m => m.Cod).ToColumn("Id");
            Map(m => m.NomeCompleto).ToColumn("Nome");
            Map(m => m.NomeCompletoMae).ToColumn("NomeMae");
            Map(m => m.Situacao).ToColumn("SituacaoCadastro");
        }
    }
}
