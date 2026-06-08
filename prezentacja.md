# Skrypt Prezentacji - Asset Store (Wersja 15-minutowa)

---

## 1. WPROWADZENIE I KONTEKST (ok. 3 minuty)

**[Akcja: Otwórz stronę główną aplikacji w przeglądarce]**

Stworzylismy aplikację o nazwie **Asset Store** – to taki sklep internetowy, ale zamiast fizycznych rzeczy, sprzedajemy cyfrowe zasoby. To mogą być modele 3D, tekstury, skrypty, pliki audio – właściwie wszystko, co jest plikiem i można to pobrać.
Całość napisaliśmy w ASP.NET Core 8 - MVC, a infrastruktura w całości stoi w chmurze Azure.
Zaczniemy od pokazania, jak to wygląda w praktyce, a potem przejdziemy do kodu i infrastruktury.

---

## 2. SZCZEGÓŁOWE DEMO APLIKACJI (ok. 6 minut)

### A. Perspektywa Gościa i UX

**[Akcja: Przewiń stronę główną powoli, pokaż karty produktów]**

"Zacznijmy od doświadczenia nowego użytkownika. Jako gość, od razu trafiam na stronę główną, gdzie prezentujemy 6 najnowszych assetów. Zależało nam na czystym, minimalistycznym designie. Przejdźmy do katalogu."

**[Akcja: Kliknij 'Catalog' w menu]**

"Tutaj użytkownik ma dostęp do wszystkich zasobów. Może je filtrować po kategorii lub sortować według ceny. Wejdźmy w szczegóły konkretnego produktu."

**[Akcja: Kliknij w asset, pokaż sekcję recenzji na dole]**

"W widoku szczegółów widzimy pełny opis, cenę oraz – co ważne – recenzje innych użytkowników. Jako gość mogę przeglądać te treści, ale przycisk pobierania jest niedostępny, a system zachęca mnie do zalogowania się."

### B. Cykl Życia Zakupu (Użytkownik)

**[Akcja: Zaloguj się jako 'michalkuchnicki@assetstore.local' (lub inny demo user)]**

"Zalogujmy się teraz jako kupujący. Proszę zwrócić uwagę na pasek nawigacji – dynamicznie dostosował się do mojej roli. Kupmy teraz wybrany przedmiot."

**[Akcja: Kliknij 'Purchase' na darmowym lub płatnym assecie]**

"W tym momencie w bazie danych została zarejestrowana transakcja, a dla mojego użytkownika wygenerowano trwałe uprawnienie do pobierania tego pliku. Teraz mogę kliknąć 'Download'. Co ważne – plik nie jest serwowany jako statyczny zasób, ale przechodzi przez kontroler, który sprawdza moje uprawnienia, zanim wyśle strumień danych."

**[Akcja: Pokaż sekcję 'Transactions']**

"Wszystkie nasze zakupy są zapisywane w historii, co pozwala na dostęp do plików w dowolnym momencie."

### C. Perspektywa Twórcy i Moderacja

**[Akcja: Zaloguj się jako 'creator1@assetstore.local']**

"Twórca ma zupełnie inne potrzeby. Teraz spójrzmy na platformę jego oczami. "

**[Akcja: Kliknij 'My assets']**

"Tutaj twórca widzi swoje statystyki – co dodał, w jakiej cenie. Może edytować opisy swoich modeli lub dodawać nowe. Formularz dodawania assetu obsługuje walidację po stronie klienta i serwera, zapewniając, że przesyłane pliki mają odpowiednie rozszerzenia (np. .zip, .fbx)."

Dodajmy jakis przykladowy przedmiot
[Zrobmy przyklad dodania jakiegos przedmiotu]

### D> Na koniec zalogujmy się jako Administrator.

**[Akcja: Zaloguj się jako 'admin']**

"Admin ma pełną władzę nad systemem. Może zarządzać kategoriami, moderować recenzje, a nawet zmieniać uprawnienia użytkowników czy ich blokować."

---

## 3. ARCHITEKTURA

"Przejdźmy teraz do tego, co dzieje się pod maską."

**[Akcja: Przełącz na Visual Studio]**

### A. Architektura N-Tier

**[Akcja: Pokaż strukturę folderów w Solution Explorerze]**

"Zastosowaliśmy klasyczny podział na warstwy.

- W folderze **Controllers** mamy punkty wejścia dla żądań HTTP.
- Folder **Services** zawiera całą logikę biznesową – to serce aplikacji.
- **Repositories** to nasza warstwa abstrakcji nad bazą danych."

### B. Dependency Injection i Storage

**[Akcja: Otwórz Services/LocalFileStorageService.cs oraz AzureBlobStorageService.cs]**

"To jest jeden z najmocniejszych punktów naszej architektury. Stworzyliśmy abstrakcję nad systemem plików.

Lokalnie korzystamy z `LocalFileStorageService`, który zapisuje pliki w folderze `App_Data`. Jednak w produkcji, poprzez prostą zmianę w `appsettings.json`, aplikacja zaczyna używać `AzureBlobStorageService`. Dzięki **Dependency Injection** w `Program.cs`, reszta systemu nie wie i nie musi wiedzieć, gdzie fizycznie leżą gigabajty danych – ona po prostu operuje na strumieniach."

### C. Autoryzacja i Role (RBAC)

**[Akcja: Otwórz plik Models/Constants/AppRoles.cs]**

"Bezpieczeństwo oparliśmy na **Role-Based Access Control**. Wykorzystujemy ASP.NET Identity, ale zdefiniowaliśmy własne role: User, Creator i Administrator. Dzięki temu zarządzanie dostępem jest bardzo proste."

**[Akcja: Otwórz CreatorController.cs i pokaż atrybut [Authorize(Roles = AppRoles.Creator)]]**

### D. Middleware i Bezpieczeństwo

**[Akcja: Otwórz Middleware/ExceptionHandlingMiddleware.cs]**

"Zależało nam na stabilności. Napisaliśmy własny Middleware do obsługi wyjątków. Jeśli w dowolnym miejscu aplikacji wydarzy się nieprzewidziany błąd, ten komponent go przechwyci, zaloguje szczegóły dla nas, a użytkownikowi wyświetli przyjazną stronę błędu."

---

## 4. CHMURA I PODSUMOWANIE

"Aplikacja jest w pełni gotowa do chmury. Wykorzystaliśmy ekosystem Azure:

- **Azure App Service** hostuje kod .NET.
- **Azure SQL** przechowuje relacyjne dane.
- **Azure Blob Storage** zajmuje się ciężkimi plikami binarnymi.
