using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sprint3API; 

namespace Sprint3APITeste
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestGetMotos_ReturnsOk()
        {
            // Arrange: Cria um cliente HTTP para chamar a API
            var client = _factory.CreateClient();

            // Act: Chama GET /motos
            var response = await client.GetAsync("/motos");

            // Assert: Verifica se o status é 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestCreateMoto_ReturnsCreated()
        {
            // Arrange: Cria cliente e payload JSON
            var client = _factory.CreateClient();
            var content = new StringContent(
                "{\"placa\":\"TEST-123\",\"cor\":\"Verde\",\"status\":\"Pronta\",\"tempoLimite\":0}",
                Encoding.UTF8,
                "application/json");

            // Act: Chama POST /motos
            var response = await client.PostAsync("/motos", content);

            // Assert: Verifica se o status é 201 Created
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}