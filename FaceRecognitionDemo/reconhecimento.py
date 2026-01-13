from deepface import DeepFace
import sys
import json

# Desativa logs desnecessários do TensorFlow para não sujar a saída do C#
import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3' 

def executar():
    img1 = sys.argv[1]
    img2 = sys.argv[2]
    
    try:
        # O modelo Facenet512 é um dos melhores do mundo
        obj = DeepFace.verify(img1_path = img1, 
                              img2_path = img2, 
                              model_name = "Facenet512",
                              enforce_detection = False)
        print(json.dumps(obj))
    except Exception as e:
        print(json.dumps({"error": str(e)}))

if __name__ == "__main__":
    executar()