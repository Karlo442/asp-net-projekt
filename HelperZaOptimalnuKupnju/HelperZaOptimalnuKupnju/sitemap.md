# Sitemap — Semantički model usmjeravanja

Popis dostupnih URL-ova u aplikaciji i mapiranje na controller, akciju i view.

- `/` → Controller: [HelperZaOptimalnuKupnju/Controllers/HomeController.cs](HelperZaOptimalnuKupnju/Controllers/HomeController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/Home/Index.cshtml](HelperZaOptimalnuKupnju/Views/Home/Index.cshtml)
- `/privacy` → Controller: [HelperZaOptimalnuKupnju/Controllers/HomeController.cs](HelperZaOptimalnuKupnju/Controllers/HomeController.cs) — Action: `Privacy()` — View: [HelperZaOptimalnuKupnju/Views/Home/Privacy.cshtml](HelperZaOptimalnuKupnju/Views/Home/Privacy.cshtml)

- `/proizv` and `/proizv/lista` → Controller: [HelperZaOptimalnuKupnju/Controllers/ProductsController.cs](HelperZaOptimalnuKupnju/Controllers/ProductsController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/Products/Index.cshtml](HelperZaOptimalnuKupnju/Views/Products/Index.cshtml)
- `/proizv/{id}` and `/proizv/detalji/{id}` → Controller: [HelperZaOptimalnuKupnju/Controllers/ProductsController.cs](HelperZaOptimalnuKupnju/Controllers/ProductsController.cs) — Action: `Details(int id)` — View: [HelperZaOptimalnuKupnju/Views/Products/Details.cshtml](HelperZaOptimalnuKupnju/Views/Products/Details.cshtml)

- `/trgov` and `/trgov/sve` → Controller: [HelperZaOptimalnuKupnju/Controllers/StoresController.cs](HelperZaOptimalnuKupnju/Controllers/StoresController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/Stores/Index.cshtml](HelperZaOptimalnuKupnju/Views/Stores/Index.cshtml)
- `/trgov/{id}` and `/trgov/info/{id}` → Controller: [HelperZaOptimalnuKupnju/Controllers/StoresController.cs](HelperZaOptimalnuKupnju/Controllers/StoresController.cs) — Action: `Details(int id)` — View: [HelperZaOptimalnuKupnju/Views/Stores/Details.cshtml](HelperZaOptimalnuKupnju/Views/Stores/Details.cshtml)

- `/narudz` and `/narudz/sve` → Controller: [HelperZaOptimalnuKupnju/Controllers/OrdersController.cs](HelperZaOptimalnuKupnju/Controllers/OrdersController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/Orders/Index.cshtml](HelperZaOptimalnuKupnju/Views/Orders/Index.cshtml)
- `/narudz/{id}` and `/narudz/prikazi/{id}` → Controller: [HelperZaOptimalnuKupnju/Controllers/OrdersController.cs](HelperZaOptimalnuKupnju/Controllers/OrdersController.cs) — Action: `Details(int id)` — View: [HelperZaOptimalnuKupnju/Views/Orders/Details.cshtml](HelperZaOptimalnuKupnju/Views/Orders/Details.cshtml)

- `/korisn` and `/korisn/lista` → Controller: [HelperZaOptimalnuKupnju/Controllers/UsersController.cs](HelperZaOptimalnuKupnju/Controllers/UsersController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/Users/Index.cshtml](HelperZaOptimalnuKupnju/Views/Users/Index.cshtml)
- `/korisn/{id}` and `/korisn/profil/{id}` → Controller: [HelperZaOptimalnuKupnju/Controllers/UsersController.cs](HelperZaOptimalnuKupnju/Controllers/UsersController.cs) — Action: `Details(int id)` — View: [HelperZaOptimalnuKupnju/Views/Users/Details.cshtml](HelperZaOptimalnuKupnju/Views/Users/Details.cshtml)

- `/orderitems` (konvencionalno) → Controller: [HelperZaOptimalnuKupnju/Controllers/OrderItemsController.cs](HelperZaOptimalnuKupnju/Controllers/OrderItemsController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/OrderItems/Index.cshtml](HelperZaOptimalnuKupnju/Views/OrderItems/Index.cshtml)
- `/orderitems/details/{id}` → Controller: [HelperZaOptimalnuKupnju/Controllers/OrderItemsController.cs](HelperZaOptimalnuKupnju/Controllers/OrderItemsController.cs) — Action: `Details(int id)` — View: [HelperZaOptimalnuKupnju/Views/OrderItems/Details.cshtml](HelperZaOptimalnuKupnju/Views/OrderItems/Details.cshtml)

- `/baze` → Controller: [HelperZaOptimalnuKupnju/Controllers/DatabasesController.cs](HelperZaOptimalnuKupnju/Controllers/DatabasesController.cs) — Action: `Index()` — View: [HelperZaOptimalnuKupnju/Views/Databases/Index.cshtml](HelperZaOptimalnuKupnju/Views/Databases/Index.cshtml)
- `/baze/download` → Controller: [HelperZaOptimalnuKupnju/Controllers/DatabasesController.cs](HelperZaOptimalnuKupnju/Controllers/DatabasesController.cs) — Action: `Download()` — (serves file download; no view)

- `/Home/Error` → Controller: [HelperZaOptimalnuKupnju/Controllers/HomeController.cs](HelperZaOptimalnuKupnju/Controllers/HomeController.cs) — Action: `Error()` — View: [HelperZaOptimalnuKupnju/Views/Shared/Error.cshtml](HelperZaOptimalnuKupnju/Views/Shared/Error.cshtml)

Napomene:

- Neke rute su definirane atributima u controllerima (npr. `ProductsController`, `StoresController`, `OrdersController`, `UsersController`, `DatabasesController`).
- Konvencionalne rute (bez atributa) slijede pattern `/Controller/Action/{id?}` za kontrolere koji nemaju prilagođene atribute.
- Ako želiš, mogu pokrenuti brzu provjeru (otvoriti svaku rutu i usporediti view/akciju s kodom) i ažurirati ovu datoteku sa stvarnim brojevima linija u referencama.
