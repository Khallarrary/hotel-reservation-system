namespace HotelApp.Domain;

public class Reserva
{
    public int Id { get; private set; }
    public int QuartoId { get; private set; }
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }
    public string NomeDoHospede { get; private set; }

    public bool ConflitaCom(Reserva outra)
    {
        return CheckIn < outra.CheckOut && CheckOut > outra.CheckIn;
    }
    public Reserva(DateTime checkIn, DateTime checkOut, string nomeDoHospede, int quartoId)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("Data de check-out deve ser superior a data de in   ");
        }
       
            CheckIn = checkIn;
            CheckOut = checkOut;
      

        if (string.IsNullOrWhiteSpace(nomeDoHospede))
        {
            throw new ArgumentException("Reserva deve conter um nome valido");
        }
        else { 
        
            NomeDoHospede = nomeDoHospede;
        }

        if (quartoId <= 0)
        {

            throw new ArgumentException("Reserva deve conter um quarto valido");
        }
        else { 
        
            QuartoId = quartoId;
        }


    }
}
