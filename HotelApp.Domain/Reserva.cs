namespace HotelApp.Domain;

public class Reserva
{
    public int Id { get; private set; }
    public int QuartoId { get; private set; }
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }
    public string NomeDoHospede { get; private set; }


    /// <summary>
    /// Verifica se esta reserva entra em conflito com outra reserva do mesmo quarto.
    /// Duas reservas conflitam quando há sobreposição de datas.
    /// </summary>
    public bool ConflitaCom(Reserva outra)
    {
        if (outra is null)
            throw new ArgumentNullException(nameof(outra));

        // Reservas de quartos diferentes nunca entram em conflito
        if (QuartoId != outra.QuartoId)
            return false;

        // Regra de sobreposição de intervalo de datas
        return CheckIn < outra.CheckOut && CheckOut > outra.CheckIn;
    }

    /// <summary>
    /// Cria uma nova reserva validando regras de negócio:
    /// - Check-out deve ser maior que check-in
    /// - Check-in não pode estar no passado
    /// - Nome do hóspede deve ser válido
    /// - Quarto deve ser válido
    /// </summary>
    public Reserva(DateTime checkIn, DateTime checkOut, string nomeDoHospede, int quartoId)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("Data de check-out deve ser superior a data de in   ");
        }

        // Evita reservas com datas já expiradas
        if (checkIn < DateTime.UtcNow)
        {
            throw new ArgumentException("A data de check-in não pode estar no passado.");
        }   

        if (string.IsNullOrWhiteSpace(nomeDoHospede))
        {
            throw new ArgumentException("Reserva deve conter um nome valido");
        }
        
        if (quartoId <= 0)
        {
            throw new ArgumentException("Reserva deve conter um quarto valido");
        }

        CheckIn = checkIn;
        CheckOut = checkOut;
        NomeDoHospede = nomeDoHospede.Trim();
        QuartoId = quartoId;


    }
}
