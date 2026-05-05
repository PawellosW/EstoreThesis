import pyodbc
import datetime

# --- KONFIGURACJA POŁĄCZENIA ---
# Używamy Twoich danych z appsettings.json
conn_str = (
    r"DRIVER={SQL Server};"
    r"SERVER=DESKTOP-550RU0O\SQLEXPRESS;"
    r"DATABASE=store;"
    r"Trusted_Connection=yes;"
)

def test_db_connection():
    try:
        print(f"[{datetime.datetime.now()}] Próba połączenia z bazą...")
        conn = pyodbc.connect(conn_str)
        cursor = conn.cursor()
        print("Sukces! Połączono z bazą.")

        # 1. Sprawdźmy czy są jakieś opinie w ogóle
        cursor.execute("SELECT TOP 1 id, text FROM reviews")
        review = cursor.fetchone()

        if not review:
            print("Baza połączona, ale tabela 'reviews' jest pusta. Dodaj opinię w sklepie!")
            return

        review_id = review[0]
        print(f"Znaleziono opinię o ID: {review_id}. Próba zapisu testowej analizy...")

        # 2. Próba zapisu "udawanej" analizy
        # Używamy nazw kolumn, które ustaliliśmy wcześniej
        insert_query = """
            INSERT INTO review_analysis 
            (review_id, price_score, quality_score, delivery_score, service_score, overall_score, is_urgent) 
            VALUES (?, ?, ?, ?, ?, ?, ?)
        """
        
        # Wstawiamy testowe "piątki"
        cursor.execute(insert_query, (review_id, 5.0, 5.0, 5.0, 5.0, 5.0, 0))
        
        conn.commit()
        print(f"ZAPISANO! Testowy rekord dla opinii {review_id} trafił do bazy.")
        
        # Zapis do pliku tekstowego (Twój backup)
        with open("aspects_rate_output_test.txt", "a", encoding="utf-8") as f:
            f.write(f"[{datetime.datetime.now()}] Test zakończony sukcesem dla ID: {review_id}\n")

    except Exception as e:
        print(f"BŁĄD: {e}")
        with open("aspects_rate_output_test.txt", "a", encoding="utf-8") as f:
            f.write(f"[{datetime.datetime.now()}] BŁĄD TESTU: {str(e)}\n")
    finally:
        if 'conn' in locals():
            conn.close()

if __name__ == "__main__":
    test_db_connection()