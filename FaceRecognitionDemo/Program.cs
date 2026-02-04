using System.Diagnostics;
using System.Text.Json;
using System.Globalization;
using OpenCvSharp; //

namespace FaceRecognitionDemo
{
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
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== BIOMETRIA FACIAL AVANÇADA (DEEPFACE) ===");

            // Define caminhos baseados na pasta de execução para evitar erros de diretório
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string pastaImages = Path.Combine(baseDir, "Images");

            if (!Directory.Exists(pastaImages)) Directory.CreateDirectory(pastaImages);

            string fotoReferencia = Path.Combine(pastaImages, "pessoa1.jpg"); //
            string fotoCapturada = Path.Combine(pastaImages, "captura_webcam.jpg");

            try
            {
                Console.WriteLine("\n[CÂMERA]: Abrindo webcam... Posicione seu rosto.");
                Console.WriteLine("[DICA]: Pressione ESPAÇO para tirar a foto ou ESC para cancelar.");

                if (CapturarFotoDaWebcam(fotoCapturada))
                {
                    Console.WriteLine("\n[SISTEMA]: Foto capturada com sucesso! Iniciando análise...");
                    ExecutarAnaliseBiometrica(fotoReferencia, fotoCapturada);
                }
                else
                {
                    Console.WriteLine("\n[AVISO]: Captura cancelada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERRO DE SISTEMA]: {ex.Message}");
            }
        }

        static bool CapturarFotoDaWebcam(string caminhoSalvar)
        {
            using var capture = new VideoCapture(0);
            if (!capture.IsOpened()) throw new Exception("Não foi possível detectar uma webcam.");

            using var window = new Window("Captura - Pressione ESPAÇO");
            using var frame = new Mat();

            while (true)
            {
                capture.Read(frame);
                if (frame.Empty()) break;

                window.ShowImage(frame);

                int key = Cv2.WaitKey(1);
                if (key == 32) // Espaço
                {
                    frame.SaveImage(caminhoSalvar);
                    return true;
                }
                if (key == 27) return false; // ESC
            }
            return false;
        }

        static void ExecutarAnaliseBiometrica(string foto1, string foto2)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Garante que o caminho do script seja absoluto para o processo pai
            string scriptPath = Path.Combine(baseDir, "reconhecimento.py");

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                // Aspas duplas nos caminhos evitam erros se houver espaços nas pastas
                Arguments = $"\"{scriptPath}\" \"{foto1}\" \"{foto2}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true, // Captura erros do Python
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(start))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Se o Python imprimir algo no console que não seja o JSON (avisos do TF, etc)
                    string jsonLine = "";
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Trim().StartsWith("{")) { jsonLine = line; break; }
                    }

                    if (!string.IsNullOrEmpty(jsonLine))
                    {
                        var resultado = JsonSerializer.Deserialize<DeepFaceResult>(jsonLine); //
                        if (resultado != null) ExibirRelatorio(resultado);
                    }
                    else
                    {
                        Console.WriteLine("\n[ERRO]: A IA não retornou um JSON válido.");
                        if (!string.IsNullOrEmpty(error))
                            Console.WriteLine("ERRO DO PYTHON: " + error);
                        else
                            Console.WriteLine("SAÍDA BRUTA: " + output);
                    }
                }
            }
        }

        static void ExibirRelatorio(DeepFaceResult res)
        {
            Console.Clear();
            Console.WriteLine("====================================================");
            Console.WriteLine("       RELATÓRIO DE VERIFICAÇÃO BIOMÉTRICA         ");
            Console.WriteLine("====================================================");

            if (res.verified) //
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("        RESULTADO: IDENTIDADE CONFIRMADA            ");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("        RESULTADO: ACESSO NEGADO / DIVERGENTE       ");
            }
            Console.ResetColor();

            // Cálculo de confiança baseado na distância de cosseno
            double conf = res.confidence > 0 ? res.confidence : (1 - (res.distance / res.threshold)) * 100;
            conf = Math.Clamp(conf, 0, 99.99);

            Console.WriteLine("\n----------------------------------------------------");
            Console.WriteLine($"🔍 Modelo: {res.model ?? "Facenet512"}"); //
            Console.WriteLine($"📊 Confiança: {conf:F2}%");
            Console.WriteLine($"📏 Distância: {res.distance:F4} (Limite: {res.threshold})"); //
            Console.WriteLine($"⏱️ Tempo: {res.time:F2}s");
            Console.WriteLine("----------------------------------------------------");

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}
