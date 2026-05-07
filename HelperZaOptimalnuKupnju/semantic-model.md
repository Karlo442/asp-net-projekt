# Semantički model baze podataka - HelperZaOptimalnuKupnju

## Popis entiteta (klasa/tablica)

### 1. **User** (Korisnici)
Predstavlja korisnike aplikacije s različitim ulogama.

#### Svojstva:
- `Id` (int, PK) - Jedinstveni identifikator
- `FirstName` (string, 100) - Ime korisnika
- `LastName` (string, 100) - Prezime korisnika
- `Email` (string, 200) - Email adresa (unique)
- `Role` (enum: Buyer, Supplier, Admin) - Uloga korisnika
- `Phone` (string, 50) - Telefonski broj
- `RegisteredAt` (DateTime) - Datum registracije
- `IsActive` (bool) - Je li korisnik aktivan

#### Veze:
- **1:N sa Order** - Jedan korisnik (Buyer) može imati više narudžbi

---

### 2. **Order** (Narudžbe)
Predstavlja narudžbe koje daju kupci.

#### Svojstva:
- `Id` (int, PK) - Jedinstveni identifikator
- `BuyerId` (int, FK → User) - ID kupca
- `CreatedAt` (DateTime) - Datum kreiranja narudžbe
- `ExpectedDeliveryDateTime` (DateTime) - Očekivani datum dostave
- `Status` (enum: Pending, Confirmed, Shipped) - Status narudžbe
- `TotalAmount` (decimal) - Ukupan iznos narudžbe

#### Veze:
- **N:1 sa User** - Veza na kupca (Buyer)
- **1:N sa OrderItem** - Jedna narudžba može imati više stavki

---

### 3. **OrderItem** (Stavke narudžbi)
Predstavlja pojedine stavke (proizvode) u narudžbi.

#### Svojstva:
- `Id` (int, PK) - Jedinstveni identifikator
- `OrderId` (int, FK → Order) - ID narudžbe
- `ProductId` (int, FK → Product) - ID proizvoda
- `Quantity` (int) - Količina produkta u narudžbi
- `UnitPrice` (decimal) - Cijena po jedinici

#### Veze:
- **N:1 sa Order** - Veza na redoslijednu narudžbu
- **N:1 sa Product** - Veza na proizvod

---

### 4. **Product** (Proizvodi)
Predstavlja proizvode dostupne za narudžbu.

#### Svojstva:
- `Id` (int, PK) - Jedinstveni identifikator
- `Name` (string, 200) - Naziv proizvoda
- `Description` (string, 1000) - Opis proizvoda
- `UnitPrice` (decimal) - Cijena po jedinici

#### Veze:
- **1:N sa OrderItem** - Jedan proizvod može biti u više stavki narudžbi
- **N:N sa Store** - Jedan proizvod može biti dostupan u više trgovina

---

### 5. **Store** (Trgovine)
Predstavlja fizičke trgovine gdje su proizvodi dostupni.

#### Svojstva:
- `Id` (int, PK) - Jedinstveni identifikator
- `Name` (string, 200) - Naziv trgovine
- `Brand` (string, 100) - Robna marka (npr. Konzum, Lidl, Spar)
- `Address` (string, 300) - Adresa trgovine
- `City` (string, 100) - Grad
- `Country` (string, 100) - Država
- `OpeningHours` (string, 100) - Radno vrijeme

#### Veze:
- **N:N sa Product** - Jedna trgovina može imati više proizvoda dostupnih

---

## Dijagram veza

```
User (1) ────────────────(N) Order
                              │
                              │(1)
                              │
                          OrderItem (N)
                              │
                              │(N)
                              │
                          Product (N)
                              │
                              │(N)
                              │
                          Store (1)
```

## Napomene

- **Inicijalni podaci**: Aplikacija je seedana s test podacima:
  - 4 korisnika (raznih uloga)
  - 3 trgovine (Konzum, Lidl, Spar)
  - 3 proizvoda (Jaja, Losos, Mlijeko)
  - 3 narudžbe s vezama na korisnike
  - 3 stavke narudžbi

- **ORM**: Entity Framework Core s SQLite bazom
- **Veze**: Sve veze su pravilno definirane s `virtual` svojstvima i `ForeignKey` atributima
- **Validacija**: Koriste se `[Required]`, `[StringLength]`, `[EmailAddress]` atributi
