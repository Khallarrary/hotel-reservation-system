namespace HotelApp.Domain;

public class Reserva
{
    public int Id { get; private set; }
    public int QuartoId { get; private set; }
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }
    public string NomeDoHospede { get; private set; }
    public ReservaStatus Status { get; private set; }
    public int HotelId { get; set; }
    private Reserva()
    {
        NomeDoHospede = string.Empty;
        Status = ReservaStatus.Pendente;
    }

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

        if (Status == ReservaStatus.Cancelada || outra.Status == ReservaStatus.Cancelada)
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
    public Reserva(DateTime checkIn, DateTime checkOut, string nomeDoHospede, int quartoId, int hotelId, DateOnly dataAtual)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("Data de check-out deve ser superior a data de in   ");
        }

        // Evita reservas com datas já expiradas
        if (DateOnly.FromDateTime(checkIn) < dataAtual)
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

        if ((checkOut.Date - checkIn.Date).TotalDays > 30)
        {
            throw new ArgumentException("Reserva não pode ultrapassar 30 dias.");
        }

        if (hotelId < 1)
        {
            throw new ArgumentException("O quarto deve conter um hotel");
        }

        CheckIn = checkIn;
        CheckOut = checkOut;
        NomeDoHospede = nomeDoHospede.Trim();
        QuartoId = quartoId;
        HotelId =  hotelId;


    }

    public void RealizarCheckIn(DateOnly dataAtual)
    {
        var dataCheckIn = DateOnly.FromDateTime(CheckIn);
        var dataCheckOut = DateOnly.FromDateTime(CheckOut);

        if (Status != ReservaStatus.Pendente)
        {
            throw new ArgumentException("Reserva deve estar com status Pendente para dar check-in");
        }

        if (dataCheckIn > dataAtual) {
            throw new ArgumentException("Não é possivel realizar check-in em reservas futuras. Troque a data da reserva");

        }

        if (dataCheckOut <= dataAtual)
        {
            throw new ArgumentException("Não é possivel realizar check-in em reservas passadas. Troque a data da reserva");
        }

       Status = ReservaStatus.CheckIn;
       
    }

    public void RealizarCheckOut(DateOnly dataAtual)
    {
        var dataCheckOut = DateOnly.FromDateTime(CheckOut);

        if (Status != ReservaStatus.CheckIn)
        {
            throw new ArgumentException("Reserva deve estar com status check-in para dar check-out");
        }

        if (dataAtual < dataCheckOut)
        {
            throw new ArgumentException("Não é possivel realizar check-out antecipado. Troque a data da reserva");

        }

        Status = ReservaStatus.CheckOut;

    }

    public void Cancelar()
    {
        if (Status != ReservaStatus.Pendente)
        {
            throw new ArgumentException("Não é possivel cancelar reserva com esse status");
        }

        
        Status = ReservaStatus.Cancelada;
    }
}
