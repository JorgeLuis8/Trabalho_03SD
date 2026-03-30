using System.Net.Http.Json;
using System.Collections.ObjectModel;
using SensorClient.Models;

namespace SensorClient;

public partial class MainPage : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    private const string ServerUrl = "http://localhost:5000/leitura";

    public ObservableCollection<string> Historico { get; set; } = new();

    public MainPage()
    {
        InitializeComponent();
        listHistorico.ItemsSource = Historico;
    }

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        // 1. Simulação
        var rnd = new Random();
        double temp = Math.Round(rnd.NextDouble() * 60 - 15, 1);
        
        var leitura = new Leitura { temperatura = temp, sensor_id = "WIN_SENSOR_01" };

        // Feedback de "Enviando"
        lblTemp.Text = temp.ToString("0.0");
        lblStatus.Text = "PROCESSANDO...";
        lblLiveTag.Text = "ENVIANDO";

        try 
        {
            var response = await client.PostAsJsonAsync(ServerUrl, leitura);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RespostaServidor>();
                string status = result?.status_logico ?? "NORMAL";

                // Atualiza a Interface com o estilo Dark
                AtualizarUI(status, temp);
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Servidor Offline", "OK");
            lblStatus.Text = "ERRO DE CONEXÃO";
            statusBadge.BackgroundColor = Color.FromArgb("#3D1014"); // Vermelho bem escuro
            lblStatus.TextColor = Color.FromArgb("#FF4D4D");
        }
    }

    private void AtualizarUI(string status, double temp)
    {
        lblStatus.Text = status.ToUpper();
        lblLiveTag.Text = "AO VIVO";

        // Lógica de cores para o tema Dark
        switch (status)
        {
            case "Crítico":
                statusBadge.BackgroundColor = Color.FromArgb("#3D1014");
                statusBadge.Stroke = Color.FromArgb("#FF4D4D");
                lblStatus.TextColor = Color.FromArgb("#FF4D4D");
                lblTemp.TextColor = Color.FromArgb("#FF4D4D");
                break;
            case "Alerta":
                statusBadge.BackgroundColor = Color.FromArgb("#3D2B10");
                statusBadge.Stroke = Color.FromArgb("#FFB84D");
                lblStatus.TextColor = Color.FromArgb("#FFB84D");
                lblTemp.TextColor = Color.FromArgb("#FFB84D");
                break;
            default:
                statusBadge.BackgroundColor = Color.FromArgb("#103D1A");
                statusBadge.Stroke = Color.FromArgb("#4DFF88");
                lblStatus.TextColor = Color.FromArgb("#4DFF88");
                lblTemp.TextColor = Color.FromArgb("#E8ECF0");
                break;
        }

        Historico.Insert(0, $"{DateTime.Now:HH:mm:ss}  >>  {temp}°C  [{status}]");
    }
}