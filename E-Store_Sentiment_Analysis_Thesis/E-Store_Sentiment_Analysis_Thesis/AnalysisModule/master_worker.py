from llama_cpp import Llama
import pyodbc
import sys

from aspects_rate_analysis import analyze_aspects
from alerts_analysis import analyze_alerts

MODEL_PATH = "C:/Users/lenovo/Desktop/llama-bin/models/bielik"

DB_CONFIG = (
    r"DRIVER={SQL Server};"
    r"SERVER=DESKTOP-550RU0O\SQLEXPRESS;"
    r"DATABASE=store;"
    r"Trusted_Connection=yes;"
)


#1. URUCHOMIENIE MODELU
 
llm = Llama(
            model_path=MODEL_PATH,
            n_ctx=4096,
            n_gpu_layers=0 
        ) 



 #2. POŁĄCZENIE Z BAZĄ I POBRANIE OPINII DO ANALIZY
 
conn = pyodbc.connect(DB_CONFIG)
cursor = conn.cursor() 
 
cursor.execute("""
    SELECT r.id, r.text 
    FROM reviews r 
    LEFT JOIN review_analysis ra ON r.id = ra.review_id 
    WHERE ra.review_id IS NULL
""")
        
reviews_to_analyze = cursor.fetchall()



#3. PĘTLA PRZETWARZANIA PIERWSZEGO POZIOMU ANALIZY

for row in reviews_to_analyze:
    review_id = row[0]
    review_text = row[1]
    
    aspects_result = analyze_aspects(llm, review_text)
    
    print(f"###ID:{review_id}###OUT:{aspects_result}###")
    sys.stdout.flush()

#4. PĘTLA PRZETWARZANIA DRUGIEGO POZIOMU ANALIZY


for row in reviews_to_analyze:
    review_id = row[0]
    review_text = row[1]
    
    alerts_result = analyze_alerts(llm, review_text)
    
    print(f"###ID:{review_id}###OUT:{alerts_result}###")
    sys.stdout.flush()
    
conn.close()