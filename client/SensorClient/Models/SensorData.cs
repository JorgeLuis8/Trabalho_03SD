namespace SensorClient.Models;

public class Leitura
{
    // ID Único para garantir a Idempotência (Regra da Atividade)
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string sensor_id { get; set; } = "MAUI_DESKTOP_01";
    public double temperatura { get; set; }
}

public class RespostaServidor
{
    // O '?' evita o aviso de que a propriedade pode ser nula antes da resposta
    public string? status_logico { get; set; }
}