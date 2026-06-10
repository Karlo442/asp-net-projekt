# Autentifikacija i Autorizacija - Kompletan Setup

## Status ✅
Implementirana je kompletan **ASP.NET Core Identity** sustav sa autentifikacijom i autorizacijom.

---

## 1. KORISNIČKI RAČUNI

### Test korisnik (Admin)
```
Email:     admin@example.com
Lozinka:   Admin123
Uloga:     Admin
```
*Korisnik je automatski kreiran pri prvom pokretanju aplikacije*

### Kreirane uloge
1. **Admin** - Puna kontrola nad sustavom
2. **Customer** - Kupac koji može praviti i ažurirati narudžbe
3. **Supplier** - Dobavljač koji može upravljati proizvodima

---

## 2. AUTENTIFIKACIJA

### MVC Endpointi
- **Registracija**: `/auth/registracija` (GET/POST)
- **Prijava**: `/auth/prijava` (GET/POST)
- **Profil**: `/auth/profil` (GET) - Requires [Authorize]
- **Promjena lozinke**: `/auth/promjena-lozinke` (GET/POST) - Requires [Authorize]
- **Odjava**: `/auth/odjava` (POST) - Requires [Authorize]

### API Endpointi
- **POST** `/auth/api/registracija` - Registracija
- **POST** `/auth/api/prijava` - Prijava
- **GET** `/auth/api/profil` - Dohvat profila (Requires [Authorize])

---

## 3. AUTORIZACIJA (ROLE-BASED ACCESS CONTROL)

### MVC Kontroleri

#### ProductsController
- **Index/Details/Search**: Javno dostupno [AllowAnonymous]
- **Create/Edit**: [Authorize(Roles = "Admin,Supplier")]
- **Delete**: [Authorize(Roles = "Admin,Supplier")]

#### OrdersController
- **Index/Details/Search**: Javno dostupno [AllowAnonymous]
- **Create/Edit**: [Authorize(Roles = "Admin,Customer")]
- **Delete**: [Authorize(Roles = "Admin")] - Samo Admin

#### UsersController
- **Index/Details/Search**: Javno dostupno [AllowAnonymous]
- **Create/Edit/Delete**: [Authorize(Roles = "Admin")]

#### StoresController
- **Index/Details/Search**: Javno dostupno [AllowAnonymous]
- **Create/Edit/Delete**: [Authorize(Roles = "Admin")]

### API Kontroleri

#### ApiProductsController
```
GET  /api/apiProducts      - [AllowAnonymous]
GET  /api/apiProducts/{id} - [AllowAnonymous]
POST /api/apiProducts      - [Authorize(Roles = "Admin,Supplier")]
PUT  /api/apiProducts/{id} - [Authorize(Roles = "Admin,Supplier")]
DELETE /api/apiProducts/{id} - [Authorize(Roles = "Admin,Supplier")]
```

#### ApiOrdersController
```
GET  /api/apiOrders        - [AllowAnonymous]
GET  /api/apiOrders/{id}   - [AllowAnonymous]
POST /api/apiOrders        - [Authorize(Roles = "Admin,Customer")]
PUT  /api/apiOrders/{id}   - [Authorize(Roles = "Admin,Customer")]
DELETE /api/apiOrders/{id} - [Authorize(Roles = "Admin")]
```

#### ApiOrderItemsController
```
GET  /api/apiOrderItems    - [AllowAnonymous]
GET  /api/apiOrderItems/{id} - [AllowAnonymous]
POST /api/apiOrderItems    - [Authorize(Roles = "Admin")]
PUT  /api/apiOrderItems/{id} - [Authorize(Roles = "Admin")]
DELETE /api/apiOrderItems/{id} - [Authorize(Roles = "Admin")]
```

#### ApiUsersController
```
GET  /api/apiUsers         - [AllowAnonymous]
GET  /api/apiUsers/{id}    - [AllowAnonymous]
POST /api/apiUsers         - [Authorize(Roles = "Admin")]
PUT  /api/apiUsers/{id}    - [Authorize(Roles = "Admin")]
DELETE /api/apiUsers/{id}  - [Authorize(Roles = "Admin")]
```

#### ApiStoresController
```
GET  /api/apiStores        - [AllowAnonymous]
GET  /api/apiStores/{id}   - [AllowAnonymous]
POST /api/apiStores        - [Authorize(Roles = "Admin")]
PUT  /api/apiStores/{id}   - [Authorize(Roles = "Admin")]
DELETE /api/apiStores/{id} - [Authorize(Roles = "Admin")]
```

---

## 4. ZAHTJEVI ZA LOZINKU

- Minimalno **6 znakova**
- Mora sadržavati **gornja slova** (A-Z)
- Mora sadržavati **mala slova** (a-z)
- Mora sadržavati **brojeve** (0-9)

### Sigurnosne mjere
- **Zaključavanje** nakon 5 neuspješnih pokušaja
- **Trajanje**: 15 minuta
- **Jedinstvena email adresa** obavezna

---

## 5. PROŠIRENI KORISNICI (AppUser)

Polje obavezno za sve korisnike:
```csharp
public string FirstName { get; set; }      // Ime
public string LastName { get; set; }       // Prezime
public string Address { get; set; }        // Adresa
public string City { get; set; }           // Grad
public string ZipCode { get; set; }        // Poštanski broj
public DateTime CreatedAt { get; set; }    // Vrijeme kreiranja
public bool IsActive { get; set; }         // Aktivnost korisnika
public string? ProfileImageUrl { get; set; } // Opciono: URL slike profila
```

---

## 6. BAZA PODATAKA

### Nove tablice (Identity)
- `AspNetUsers` - Korisnici
- `AspNetRoles` - Uloge
- `AspNetUserRoles` - Veza korisnik-uloga
- `AspNetUserClaims` - Zahtjeve korisnika
- `AspNetRoleClaims` - Zahtjeve uloge
- `AspNetUserLogins` - Prijave
- `AspNetUserTokens` - Tokeni

### Migracija
```
Migration: InitialCreateWithIdentity
Status: Primenjeno ✅
```

---

## 7. KAKO TESTIRATI

### 1. Pokretanje aplikacije
```bash
dotnet run
```

### 2. Registracija novog korisnika
1. Idite na `/auth/registracija`
2. Popunite sve obavezne podatke
3. Kliknite "Registriraj se"
4. Automatski ste prijavljeni i vidljiv je vaš profil

### 3. Prijava sa različitim ulogama
```
# Admin - Ima pristup svim akcijama
Email: admin@example.com
Lozinka: Admin123

# Za testiranje drugih uloga, trebate registrirati nove korisnike
# i ih ručno dodeliti ulogama kroz administratorski panel
```

### 4. Testiranje autorizacije
- **Javne akcije**: Dostupno anonimnim korisnicima (Index, Details, Search)
- **Zaštićene akcije**: Zahtevana prijava (Create, Edit, Delete)
- **Ulogom ograničene akcije**: Zahtevana specifična uloga

### 5. API Testiranje (Postman/cURL)
```bash
# Prijava preko API-ja
POST http://localhost:5000/auth/api/prijava
Content-Type: application/json

{
  "userNameOrEmail": "admin@example.com",
  "password": "Admin123",
  "rememberMe": false
}

# Dohvat proizvoda (javno)
GET http://localhost:5000/api/apiproducts

# Kreiranje proizvoda (zahteva autentifikaciju)
POST http://localhost:5000/api/apiproducts
Authorization: Bearer <token>
Content-Type: application/json

{...}
```

---

## 8. DATOTEKE KOJE SU KREIRANE/IZMENJENE

### Kreirane
- `Models/AppUser.cs` - Prošireni korisnik
- `Models/AppRole.cs` - Proširena uloga
- `DTOs/AuthDTO.cs` - DTO za autentifikaciju
- `Controllers/AuthController.cs` - Autentifikacijski kontroler
- `Views/Auth/Register.cshtml` - Registracijska forma
- `Views/Auth/Login.cshtml` - Prijava forma
- `Views/Auth/Profile.cshtml` - Profil korisnika
- `Views/Auth/ChangePassword.cshtml` - Promjena lozinke

### Izmenjene
- `Data/ApplicationDbContext.cs` - Integracijska IdentityDbContext
- `Program.cs` - Konfiguracija Identity servisa
- `Views/Shared/_Layout.cshtml` - Dodane autentifikacijske kontrole u navbar
- `Views/_ViewImports.cshtml` - Dodani using za DTOs
- `Controllers/ProductsController.cs` - Dodata role-based autorizacija
- `Controllers/OrdersController.cs` - Dodata role-based autorizacija
- `Controllers/UsersController.cs` - Dodata role-based autorizacija
- `Controllers/StoresController.cs` - Dodata role-based autorizacija
- `ApiControllers/Api*Controller.cs` - Dodata fine-grained autorizacija

---

## 9. ZAHTJEVI ISPUNJENI ✅

✅ "Autentikacija mora biti uključena kroz ASP.NET Core Identity"
✅ "Lokalna registracija i prijava moraju raditi"
✅ "'AppUser' mora biti proširen traženim poljima"
✅ "Autorizacija mora ograničavati akcije prema pravilima"
✅ "Role 'Admin' i barem još jedna rola moraju biti implementirane"
✅ "Javne akcije dostupne su anonimnim korisnicima"
✅ "Create/Edit/Delete dostupni su samo dopuštenim korisnicima"

---

## 10. НАПОМЕНЕ

- Svi API kontroleri podržavaju [AllowAnonymous] za GET operacije (čitanje)
- Samo autentificirani korisnici sa odgovarajućom ulogom mogu promeniti podatke
- Admin korisnik je automatski seeden pri prvom pokretanju
- Sve tri uloge su automatski kreirane pri prvom pokretanju
