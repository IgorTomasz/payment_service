# Payment Microservice

## 📝 Opis projektu
Mikroserwis Payment jest częścią większego systemu kasyna online, odpowiedzialny za zarządzanie płatnościami, portfelami użytkowników oraz transakcjami.

Serwis jest jednym z czterech mikroserwisów tworzących kompletny system:
- 🎮 Game Service - obsługa logiki gier
- 👤 Account Service - zarządzanie kontami użytkowników
- 💰 Payment Service (ten projekt) - obsługa płatności
- 🔀 Gateway Service - zarządzanie komunikacją między serwisami

## 🛠 Technologie
- ASP.NET Core
- Entity Framework Core
- MS SQL Server
- Docker
- REST API

## 🔒 Zabezpieczenia
System implementuje wielopoziomowe zabezpieczenia:
1. **IP Whitelist**
   - Filtrowanie requestów na podstawie dozwolonych adresów IP
   - Konfiguracja w pliku appsettings.json

2. **Custom Header Validation**
   - Walidacja specjalnego nagłówka w każdym żądaniu
   - Wartość nagłówka porównywana z konfiguracją w appsettings.json

## 📋 Wymagania systemowe
- .NET 6.0 lub nowszy
- Docker Desktop
- MS SQL Server (opcjonalnie, jeśli nie używamy Dockera)

## ⚙️ Konfiguracja i uruchomienie

### Przy użyciu Dockera:
```bash
# Sklonuj repozytorium
git clone https://github.com/IgorTomasz/payment-service.git

# Przejdź do katalogu projektu
cd payment_service

# Zbuduj i uruchom kontenery
docker-compose up --build
```

### Lokalne uruchomienie:
1. Sklonuj repozytorium
2. Zaktualizuj connection string w `appsettings.json`
3. Wykonaj migracje bazy danych:
```bash
dotnet ef database update
```
4. Uruchom aplikację:
```bash
dotnet run
```

## 🚀 Endpointy API

### Zarządzanie kontami płatniczymi
```http
# Pobranie wszystkich kont
GET /payment/payment/account/all

# Utworzenie nowego konta
POST /payment/payment/account/create
```
Request body dla utworzenia konta:
```json
{
    "userId": "guid"
}
```

### Zarządzanie portfelem
```http
# Pobranie salda użytkownika
GET /payment/payment/wallet/balance/{userId}

# Pobranie historii transakcji użytkownika
GET /payment/payment/wallet/transactions/{userId}
```

### Zarządzanie transakcjami
```http
# Pobranie wszystkich transakcji
GET /payment/payment/transactions/all

# Obsługa płatności
POST /payment/payment/payments/handle-payment
```
Request body dla obsługi płatności:
```json
{
    "userId": "guid",
    "amount": "decimal",
    "metaData": {
        "transactionType": "string",
        "paymentMethod": "string"
    }
}
```

### Limity transakcji
- Minimalna kwota dla wszystkich typów transakcji: 1.00
- Brak górnego limitu kwoty transakcji

### Statusy transakcji
- **Completed** - transakcja zakończona sukcesem
- **Failed** - transakcja zakończona niepowodzeniem
- **Processing** - transakcja w trakcie przetwarzania

### Wymagane pola MetaData
- **TransactionType** - typ transakcji (Deposit, Withdraw, GameBet, GameWin)
- **PaymentMethod** - metoda płatności (Card, Paypal, Blik, System)

Uwaga: Metoda płatności "System" jest zarezerwowana dla transakcji wewnętrznych (np. GameBet, GameWin) i nie powinna być używana dla wpłat/wypłat zewnętrznych.
```

## 💳 Metody płatności i typy transakcji

### Przykłady użycia

#### Wpłata przez PayPal:
```json
{
    "userId": "guid",
    "amount": 100.00,
    "metaData": {
        "transactionType": "Deposit",
        "paymentMethod": "Paypal"
    }
}
```

#### Wypłata na kartę:
```json
{
    "userId": "guid",
    "amount": 500.00,
    "metaData": {
        "transactionType": "Withdraw",
        "paymentMethod": "Card"
    }
}
```

#### Zakład w grze (transakcja systemowa):
```json
{
    "userId": "guid",
    "amount": 50.00,
    "metaData": {
        "transactionType": "GameBet",
        "paymentMethod": "System"
    }
}
```

## 📤 Struktura odpowiedzi API
Każdy endpoint zwraca ujednoliconą strukturę odpowiedzi w formacie:

```csharp
public class HttpResponseModel
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public object? Message { get; set; }
}
```

### Przykładowe odpowiedzi

#### Sukces - utworzenie transakcji wpłaty:
```json
{
    "success": true,
    "error": null,
    "message": {
        "transactionId": "guid",
        "status": "Processing",
        "amount": 100.00,
        "metaData": {
            "transactionType": "Deposit",
            "paymentMethod": "CreditCard"
        }
    }
}
```

#### Sukces - pobranie salda:
```json
{
    "success": true,
    "error": null,
    "message": 1000.50
}
```

#### Błąd - niewystarczające środki:
```json
{
    "success": false,
    "error": "No sufficient funds on the account",
    "message": null
}
```

#### Sukces - utworzenie transakcji:
```json
{
    "success": true,
    "message": "guid-transakcji"
}
```

## 🔄 Proces obsługi transakcji
1. Weryfikacja istnienia konta
2. Sprawdzenie dostępności środków (dla wypłat i zakładów)
3. Utworzenie rekordu transakcji
4. Aktualizacja salda konta
5. Finalizacja transakcji

## 🛡️ Zabezpieczenia transakcji
- Walidacja dostępności środków przed realizacją wypłat i zakładów
- Weryfikacja istnienia konta użytkownika
- Atomowe operacje na saldzie
- Pełna historia transakcji
- Metadane transakcji dla audytu

## 🔄 Integracja z pozostałymi serwisami
- Wykorzystanie Gateway Service jako punkt wejścia do systemu

## 📈 Możliwości rozwoju
- Implementacja dodatkowych metod płatności
- Rozbudowa systemu raportowania
- Implementacja systemów lojalnościowych
- Integracja z zewnętrznymi dostawcami płatności

## 👨‍💻 Autor
Igor Tomaszewski
