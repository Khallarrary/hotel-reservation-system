using HotelApp.Application.Interfaces;

namespace HotelApp.Api.Contextos
{
    public class HotelContexto : IHotelContexto
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HotelContexto(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor ;
        }

        public int? ObterHotelId()
        {
            var obterValor = _contextAccessor.HttpContext?.User.FindFirst("hotelId")?.Value;

            if(int.TryParse(obterValor, out var hotelId) && hotelId > 0)
            {
                return hotelId;
            }
            
            return null;
        }
    }
}
