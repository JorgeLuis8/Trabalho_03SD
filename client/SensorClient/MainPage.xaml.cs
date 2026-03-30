using System.Net.Http.Json;
using System.Collections.ObjectModel;
using SensorClient.Models;

namespace SensorClient;

public partial class MainPage : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    
    // URL do servidor (Use localhost se estiver no mesmo PC)
    private const string ServerUrl = "http://localhost:5000/leitura";

    public ObservableCollection<string> Historico { get; set; } = new();

    public MainPage()
    {
        InitializeComponent();
        listHistorico.ItemsSource = Historico;
    }

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        var rnd = new Random();
        double temp = Math.Round(rnd.NextDouble() * 50 - 10, 2);
        
        var leitura = new Leitura { temperatura = temp };
        lblTemp.Text = $"{temp} °C";
        lblStatus.Text = "Enviando...";

        try 
        {
            var response = await client.PostAsJsonAsync(ServerUrl, leitura);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RespostaServidor>();
                
                // Usando o operador '?' para evitar crash se o result for nulo
                string status = result?.status_logico ?? "Erro na resposta";
                
                lblStatus.Text = $"Status: {status}";
                Historico.Insert(0, $"{DateTime.Now:HH:mm:ss} | {temp}°C | {status}");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro de Conexão", "Servidor Flask offline. Certifique-se de que o app.py está rodando.", "OK");
            lblStatus.Text = "Erro ao conectar.";
        }
    }
}