from deepface import DeepFace
import sys
import json
import os
import cv2

# Desativa logs do TensorFlow
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3' 

def executar():
    # Captura os caminhos passados pelo C#
    img1_path = sys.argv[1]
    img2_path = sys.argv[2]
    
    # --- LÓGICA PARA MOSTRAR AS DUAS IMAGENS ---
    img_ref = cv2.imread(img1_path)
    img_cap = cv2.imread(img2_path)
    
    if img_ref is not None and img_cap is not None:
        # Mostra a foto que está no banco (Referência)
        cv2.imshow("1. Referencia (Banco de Dados)", img_ref)
        
        # Mostra a foto tirada agora (Webcam)
        cv2.imshow("2. Captura Atual (Webcam)", img_cap)
        
        # O waitKey(1) permite que as janelas apareçam enquanto a IA processa.
        # As janelas ficarão abertas durante os segundos em que o Facenet512 trabalha.
        cv2.waitKey(1) 
    # -------------------------------------------

    try:
        # Executa a comparação biométrica
        obj = DeepFace.verify(img1_path = img1_path, 
                              img2_path = img2_path, 
                              model_name = "Facenet512",
                              enforce_detection = False)
        
        # Retorna o resultado JSON para o C#
        print(json.dumps(obj))
        
    except Exception as e:
        print(json.dumps({"error": str(e)}))

if __name__ == "__main__":
    executar()