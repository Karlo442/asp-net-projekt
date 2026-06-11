# Žalbe - Nova Funkcionalnost

## Pregled

Dodan je sustav za uploadanje i upravljanje žalbama na Home stranici. Korisnici mogu:

✅ Uploadati žalbe s datotekama  
✅ Pregledati sve uplaćene žalbe  
✅ Brisati postojeće žalbe  
✅ Preuzimati priložene datoteke  

## Tehnologija

- **Frontend**: Dropzone.js za asinkroni upload
- **Backend**: ASP.NET Core Web API
- **Baza podataka**: SQLite (Entity Framework Core)
- **Pohrana datoteka**: wwwroot/uploads/complaints/

## Kako funkcionira

### 1. Uploadanje Žalbe

Na Home stranici (`/`) logicirani korisnici mogu:
1. Unijeti naslov žalbe
2. Unijeti opis žalbe
3. Prevući datoteku na Dropzone područje ili kliknuti za odabir
4. Kliknuti "Pošalji Žalbu"

### 2. API Endpointi

```
GET    /api/complaints           - Dohvata sve žalbe trenutnog korisnika
POST   /api/complaints           - Kreira novu žalbu s datotekom
DELETE /api/complaints/{id}      - Briše žalbu
```

### 3. Baza Podataka

Nova tablica `Complaints`:
- `Id` (int) - Primarni ključ
- `Title` (string) - Naslov žalbe
- `Description` (string) - Opis žalbe
- `FilePath` (string) - Putanja do datoteke
- `CreatedAt` (DateTime) - Datum kreiranja
- `UserId` (string) - ID korisnika

## Datoteke i Mapiranja

### Nove datoteke:
- [Models/Complaint.cs](HelperZaOptimalnuKupnju/HelperZaOptimalnuKupnju/Models/Complaint.cs)
- [DTOs/ComplaintDTO.cs](HelperZaOptimalnuKupnju/HelperZaOptimalnuKupnju/DTOs/ComplaintDTO.cs)
- [ApiControllers/ApiComplaintsController.cs](HelperZaOptimalnuKupnju/HelperZaOptimalnuKupnju/ApiControllers/ApiComplaintsController.cs)

### Ažurirane datoteke:
- [Data/ApplicationDbContext.cs](HelperZaOptimalnuKupnju/HelperZaOptimalnuKupnju/Data/ApplicationDbContext.cs) - Dodan DbSet za Complaints
- [Views/Home/Index.cshtml](HelperZaOptimalnuKupnju/HelperZaOptimalnuKupnju/Views/Home/Index.cshtml) - Dodana Dropzone komponenta
- [Views/Shared/_Layout.cshtml](HelperZaOptimalnuKupnju/HelperZaOptimalnuKupnju/Views/Shared/_Layout.cshtml) - Dodan Dropzone CDN

## Migracija Baze

```bash
cd HelperZaOptimalnuKupnju
dotnet ef migrations add AddComplaintsTable --no-build
dotnet ef database update
```

## Datotečni Sustav

Sve uplaćene datoteke se čuvaju u:
```
HelperZaOptimalnuKupnju/wwwroot/uploads/complaints/
```

Datoteke se imenuju kao: `{UserId}_{timestamp}_{originalFileName}`

## Sigurnost

- ✅ Samo logicirani korisnici mogu pristupiti
- ✅ Korisnici vide samo svoje žalbe
- ✅ Admin može brisati bilo koju žalbu
- ✅ Datoteke se brišu sa diska kada se briše žalba
- ✅ API zaštiten s `[Authorize]` atributom

## Ograničenja

- Maksimalna veličina datoteke: 10 MB
- Dozvoljeni formati: .pdf, .doc, .docx, .txt, .jpg, .jpeg, .png
- Žalbe se čuvaju bez brisanja (samo korisnik ili admin mogu obrisati)

## Testiranje

1. Prijaviti se kao korisnik
2. Otići na Home stranicu
3. Unijeti naslov i opis žalbe
4. Uploadati datoteku (ili preskočiti)
5. Kliknuti "Pošalji Žalbu"
6. Žalba se pojavljuje u popisu
7. Kliknuti "Obriši" za brisanje

## Budući Razvoj

- Dodati pretragu i filtriranje žalbi
- Dodati sortiranje po datumu
- Dodati drag-and-drop za više datoteka
- Dodati email notifikacije
- Dodati status žalbe (Otvorena, Zatvorena, itd.)
