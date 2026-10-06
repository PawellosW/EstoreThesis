# Wdrożenie na Microsoft Azure

**Aplikacja:** https://estore-app.blackflower-539a1832.polandcentral.azurecontainerapps.io/

Dokument opisuje proces wdrożenia aplikacji w chmurze Microsoft Azure -
od utworzenia zasobów, przez publikację obrazu, po uruchomienie i weryfikację
działania.

Aplikacja to sklep internetowy napisany w ASP.NET Core 8 (MVC) z bazą
SQL Server, uruchamiany jako kontener Docker. Wcześniej został wdrożony
lokalnie przy użyciu Docker Compose, co pozwoliło przygotować i przetestować
konfigurację przed przeniesieniem jej do chmury.

## Proces wdrożenia

### 1. Utworzenie grupy zasobów

W interfejsie Azure, stworzyłem wspólną grupę (Resource Group) `estore-rg`.
Grupa pozwala zarządzać wszystkimi zasobami związanymi z wdrożeniem mojego projektu,
i w razie potrzeby usunąć całość jednym działaniem.

Podczas konfiguracji ustawiłem region na Poland Central, co daje 
najniższe opóźnienia dla polskich użytkowników, a także
pozwala wydajniej współpracować zasobom z tego samego regionu (ruch nie idzie przez pół Europy).



### 2. Utworzenie serwera logicznego i bazy danych

Pierwszy zasób który trafił do grupy, to serwer logiczny `estore-sql-wachowski`. 
Baza w Azure SQL nie istnieje samodzielnie, musi należeć do serwera.

Podczas konfiguracji stworzyłem konto administratora, ponieważ logowanie odbywa się przez SQL Authentication.
Skonfigurowałem również reguły zapory (która sprawdza czy IP jest na liście), dodając swój adres IP, abym mógł łączyć się
z bazą przez SSMS. 

Serwer udostępnia bazę pod adresem `estore-sql-wachowski.database.windows.net`. 
Tego adresu używałem później zarówno przy łączeniu się z bazą z poziomu SSMS, jak i w connection stringu aplikacji.

Następnie utworzyłem bazę danych `store` w wariancie serverless. Wariant ten 
automatycznie wstrzymuje bazę przy braku aktywności i wznawia ją przy pierwszym zapytaniu. 
Ustawiłem także wstrzymanie bazy po przekroczeniu miesięcznego limitu darmowej oferty,
co wyklucza powstanie nieoczekiwanych kosztów.



### 3. Połączenie z bazą i dodanie danych

Za pomocą oprogramowania SQL Server Management Studio, połączyłem się z utworzoną
na Azure bazą danych, podając adres serwera logicznego, oraz login i hasło utworzonego konta administratora.

Na uruchomionej bazie wykonałem używany wcześniej (na potrzeby wdrożenia
lokalnego) skrypt `init.sql`, tworzący tabele.

Usunąłem z niego CREATE DATABASE oraz USE, ponieważ Azure nie obsługuje tych poleceń,
jak również nie są one potrzebne. W Azure SQL baza jest osobnym zasobem, utworzonym wcześniej
w portalu, a połączenie prowadzi bezpośrednio do niej. Kontekst jest więc ustalony
już w momencie połączenia.



### 4. Utworzenie rejestru


Po stworzeniu bazy z serwerem, kolejnym zasobem który utworzyłem, był Azure Container Registry 
`estoreregistrywachowski`, będący magazynem obrazów Docker.

Jest niezbędny, ponieważ usługa hostingowa nie ma dostępu do dysku lokalnego.
To tutaj trafia obraz mojej aplikacji zbudowany za pomocą Dockera. Stąd Azure pobiera go, 
a następnie uruchamia w Container App.

W konfiguracji wybrałem najtańszą wersję Basic, w zupełności wystarczającą dla jednego projektu.

W ustawieniach włączyłem konto administratora, dzięki czemu mogłem lokalnie uwierzytelnić się 
podczas wypychania obrazu do utworzonego rejestru, za pomocą docker login.




### 5. Zbudowanie i wypchnięcie obrazu do rejestru


Przed zbudowaniem i wysłaniem obrazu, uwierzytelniłem Dockera w rejestrze :


```powershell
docker login estoreregistrywachowski.azurecr.io
```

W tym momencie Docker zapytał mnie o login i hasło do konta administratora, tworzonego
z pozycji interfejsu Azure podczas tworzenia rejestru.

Następnie zbudowałem obraz na podstawie tego samego pliku Dockerfile,
którego używałem przy wdrożeniu lokalnym:

```powershell
docker build -t estoreregistrywachowski.azurecr.io/estore-app:v1 .
```

Nazwa obrazu zawiera pełny adres rejestru, po czym Docker
rozpozna przy wypychaniu, dokąd wysłać obraz.

Człon `v1` to tag wersji. Kolejne wydania aplikacji otrzymują nowe tagi, co pozwala w razie potrzeby
wrócić do wcześniejszej wersji.
Na końcu polecenia należy podać lokalizację plików do budowania. Kropka w moim przypadku
oznacza bieżący katalog.

Na koniec wypchnąłem obraz do rejestru:

```powershell
docker push estoreregistrywachowski.azurecr.io/estore-app:v1
```


### 6. Utworzenie Container App

Rejestr jest wyłącznie magazynem, gdzie obrazy leżą bezczynnie.
Container App to z kolei maszyna, która pobiera ten obraz, a następnie go uruchamia, 
dając mu procesor, pamięć i publiczny adres.

Azure rozdziela tutaj dwie warstwy, środowiska - czyli sieć wirtualna, oraz
aplikacja - czyli konkretny kontener z własnym obrazem i adresem publicznym.

Najpierw utworzyłem środowisko `estore-env`, a następnie w tym środowisku
utworzyłem samą aplikację `estore-app`.

W konfiguracji wskazałem źródło obrazu: rejestr `estoreregistrywachowski`,
wypchnięty obraz `estore-app` i tag `v1`.

Aplikacja pobiera obraz z rejestru przy użyciu managed identity.
Azure nadaje jej własną tożsamość z uprawnieniem do odczytu rejestru, 
więc w konfiguracji nie trzeba przechowywać żadnego hasła.

Przydzieliłem aplikacji 0,5 rdzenia procesora i 1 GiB pamięci w profilu
Consumption, rozliczanym według faktycznego zużycia.

Włączyłem również ingress, czyli dostęp z internetu, wskazując port
docelowy **8080**, ten sam, który ustawiłem w Dockerfile poprzez
`ENV ASPNETCORE_URLS`. Bez zgodności tych wartości aplikacja działałaby,
ale ruch z zewnątrz nie trafiałby do kontenera.

Connection string do bazy przekazałem jako zmienną środowiskową
`ConnectionStrings__DefaultConnection`. Podwójny podkreślnik w nazwie
odwzorowuje strukturę pliku `appsettings.json`, dzięki czemu aplikacja
odczytuje tę wartość tak samo jak konfigurację lokalną.



### 7. Uruchomienie i weryfikacja

Publiczny adres aplikacji znalazłem w zakładce Overview utworzonej
Container App, w polu Application Url. Pod tym adresem sklep był dostępny
z poziomu przeglądarki.




