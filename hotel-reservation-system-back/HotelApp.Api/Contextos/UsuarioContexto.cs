using HotelApp.Application.Interfaces;
using System.Security.Claims;

namespace HotelApp.Api.Contextos
{
    public class UsuarioContexto : IUsuarioContexto
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UsuarioContexto(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public int? ObterUsuarioId()
        {
            var obterValor = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(obterValor, out var usuarioId) && usuarioId > 0)
            {
                return usuarioId;
            }
            return null;
        }
    }
}
