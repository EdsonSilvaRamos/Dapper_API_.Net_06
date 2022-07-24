using Dapper.FluentMap;
using eCommerce.API.Mappers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuração feita para realizar o mapeamento da tabela Usuarios e o Objeto UsuarioDiferente (Criado para realização dos testes)
FluentMapper.Initialize(config =>
{
    config.AddMap(new UsuarioDiferenteMap());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();