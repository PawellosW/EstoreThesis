# Sklep internetowy z inteligentnym przetwarzaniem opinii

Platforma e-commerce z wbudowanym modułem inteligentnej analizy opinii klientów. 
System umożliwia przeglądanie i zamawianie produktów wraz z koszykiem, wystawianie opinii.
Administrator posiada dedykowany panel transakcji, funkcjonalności zarządzania stanem magazynowym oraz dashboard analityczny, w którym może uruchomić analizę opinii klientów przy użyciu lokalnego modelu językowego. Wyniki prezentowane są administratorowi na bieżąco w trakcie trwania analizy.

---

## Technologie

| Technologia | Zastosowanie |
|---|---|
| C# / ASP.NET Core MVC | Framework aplikacji webowej, architektura MVC z warstwą serwisową |
| Entity Framework Core | ORM, obsługa bazy danych i relacji między encjami |
| Microsoft SQL Server | Relacyjna baza danych systemu |
| Python | Worker analityczny — komunikacja z modelem językowym |
| pyodbc | Połączenie workera z bazą danych |
| Bielik 4.5B Q8 | Lokalny model językowy do analizy opinii |
| Docker | Konteneryzacja aplikacji |
