using System.Diagnostics;
using System.Text.Json;

namespace FaceRecognitionDemo
{
    // Classe para mapear o resultado da IA (JSON -> C#)
    public class DeepFaceResult
    {
        public bool verified { get; set; }
        public double distance { get; set; }
        public double threshold { get; set; }
        public double confidence { get; set; }
        public string? model { get; set; }
        public double time { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== BIOMETRIA FACIAL AVANÇADA (DEEPFACE) ===");

            // Caminhos das imagens
            string foto1 = "Images/pessoa1.jpg";
            string foto2 = "Images/pessoa2.jpg";

            // Configuração para chamar o Python
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"reconhecimento.py {foto1} {foto2}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            Console.WriteLine("A analisar rostos com redes neurais... Aguarde.");

            try
            {
                using (Process? process = Process.Start(start))
                {
                    if (process != null)
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string resultText = reader.ReadToEnd();

                            // Tentamos converter o JSON recebido do Python
                            var resultado = JsonSerializer.Deserialize<DeepFaceResult>(resultText);

                            if (resultado != null)
                            {
                                ExibirRelatorio(resultado);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERRO DE SISTEMA]: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void ExibirRelatorio(DeepFaceResult res)
        {
            Console.Clear();
            Console.WriteLine("====================================================");
            Console.WriteLine("       RELATÓRIO DE VERIFICAÇÃO BIOMÉTRICA         ");
            Console.WriteLine("====================================================");

            if (res.verified)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("        RESULTADO: IDENTIDADE CONFIRMADA            ");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("        RESULTADO: ACESSO NEGADO / DIVERGENTE       ");
            }
            Console.ResetColor();

            Console.WriteLine("\n----------------------------------------------------");
            Console.WriteLine($"🔍 Modelo Utilizado:  {res.model}");
            Console.WriteLine($"📊 Precisão (Conf.):  {res.confidence:F2}%");
            Console.WriteLine($"📏 Distância Facial:  {res.distance:F4}");
            Console.WriteLine($"📉 Limite Aceitável:  {res.threshold}");
            Console.WriteLine($"⏱️ Tempo de Resposta: {res.time:F2} segundos");
            Console.WriteLine("----------------------------------------------------");

            if (res.verified)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[INFO]: Os padrões biométricos coincidem com o registo.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ALERTA]: Risco de fraude ou pessoa não autorizada.");
            }

            Console.ResetColor();
            Console.WriteLine("====================================================\n");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}