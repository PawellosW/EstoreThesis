from llama_cpp import Llama
import pyodbc
import datetime
import sys
import os


DB_CONFIG = (
    r"DRIVER={SQL Server};"
    r"SERVER=DESKTOP-550RU0O\SQLEXPRESS;"
    r"DATABASE=store;"
    r"Trusted_Connection=yes;"
)
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROMPT_FILE = os.path.join(SCRIPT_DIR, "aspects_rate_prompt.txt")
MODEL_PATH = "C:/Users/lenovo/Desktop/llama-bin/models/bielik"


def main():
    # 1. Inicjalizacja Modelu (Raz na początku)
    # n_ctx to długość pamięci (kontekstu) - 2048 wystarczy dla opinii
    
    if not os.path.exists(PROMPT_FILE):
        print(f"BŁĄD: Brak pliku promptu '{PROMPT_FILE}'")
        return
    
    with open(PROMPT_FILE, "r", encoding="utf-8") as f:
        # Wczytujemy całą treść instrukcji
        system_instruction = f.read().strip()
    
    
    try:
        llm = Llama(
            model_path=MODEL_PATH,
            n_ctx=4096,
            n_gpu_layers=0 
        )
    except Exception as e:
        print(f"BŁĄD MODELU: {e}")
        return

    # 2. Połączenie z Bazą Danych
    try:
        conn = pyodbc.connect(DB_CONFIG)
        cursor = conn.cursor()
        
        # Pobieramy ID i TEKST opinii, które NIE MAJĄ jeszcze analizy
        cursor.execute("""
            SELECT r.id, r.text 
            FROM reviews r 
            LEFT JOIN review_analysis ra ON r.id = ra.review_id 
            WHERE ra.review_id IS NULL
        """)
        reviews_to_analyze = cursor.fetchall()
    except Exception as e:
        print(f"BŁĄD BAZY: {e}")
        return

    if not reviews_to_analyze:
        print("BRAK_OPINII") # Sygnał dla C#, że nie ma nic do roboty
        return

    # 3. Pętla Analizy
    for row in reviews_to_analyze:
        review_id = row[0]
        review_text = row[1]
        final_prompt = system_instruction.replace("{opinia}", review_text)
        

        try:
            # Generowanie odpowiedzi
            response = llm(
               final_prompt, 
                max_tokens=512,
                temperature=0.1
            )
            
            raw_output = response['choices'][0]['text']

            # CZYSZCZENIE: Usuwamy znaki nowej linii, aby cała paczka była w JEDNEJ LINII dla C#
            clean_output = raw_output.replace("\n", " ").replace("\r", " ").strip()

            # KRZYK DO C#: Specjalny format komunikatu
            # Dzięki temu C# łatwo wyciągnie ID i tekst do Twojego parsera
            print(f"###ID:{review_id}###OUT:{clean_output}###")
            
            # WYMUSZENIE WYSYŁKI: C# musi dostać to natychmiast
            sys.stdout.flush()

        except Exception as e:
            # Jeśli jedna opinia zawiedzie, "krzyczymy" o błędzie dla tego ID i lecimy dalej
            print(f"###ID:{review_id}###ERROR:Analiza nieudana###")
            sys.stdout.flush()

    # Zamknięcie zasobów
    conn.close()

if __name__ == "__main__":
    main()