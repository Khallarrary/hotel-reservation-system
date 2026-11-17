namespace HotelApp.Application.DTOs;

public class ReservaHospedeDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool Titular { get; set; }
}
