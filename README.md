# 🛡️ Sistema de Biometria Facial Avançada

Este projeto realiza o reconhecimento facial comparando duas imagens utilizando **C#** e a biblioteca de IA **DeepFace**.

## 🚀 Tecnologias
- **C# (.NET 8):** Interface e lógica de controlo.
- **Python:** Motor de Inteligência Artificial.
- **DeepFace (Facenet512):** Modelo de rede neural para comparação biométrica.

## 📊 Como Funciona
O sistema integra C# e Python via subprocesso, capturando dados em formato JSON. 
O modelo Facenet512 analisa 512 pontos faciais, garantindo uma precisão superior ao comparar a **Distância de Cosseno**.

## 🛠️ Requisitos
- Python 3.10+
- Bibliotecas: `pip install deepface tf-keras opencv-python`
