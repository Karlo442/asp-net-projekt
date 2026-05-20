# SKILL: Create a "List" page

Purpose
-------
This skill documents a standardized procedure for creating a "List" page in the ASP.NET MVC app. Use it as a template when scaffolding new list views backed by a controller action and optional data source.

When to use
-----------
- Create a paginated, searchable list of entities (Products, Stores, Users, Orders, etc.).
- Add a new admin or public listing page following app conventions.

Inputs the skill expects
-----------------------
- `EntityName` (e.g., Product)
- `ControllerName` (e.g., ProductsController)
- `Route` (e.g., `/proizv/lista`)
- `ViewPath` (e.g., Views/Products/Index.cshtml)
- `ModelType` (fully-qualified or project type)
- Optional: `SupportsPaging` (true/false), `SupportsSearch` (true/false)

Steps (high level)
------------------
1. Add controller action returning `IActionResult Index(string? q, int page = 1)` that queries the `ApplicationDbContext` and projects to a view model.
2. Create or update the Razor view at `ViewPath` with `@model PagedList<ViewModelType>` or `IEnumerable<ViewModelType>` if not paging.
3. Add optional client-side search box and server-side filtering in the action.
4. Register route using attribute routing if you want a custom URL (e.g., `[Route("proizv")]` on controller, `[HttpGet("lista")]` on action) or rely on conventional routing.
5. Add navigation link in `_Layout.cshtml`.

Example
-------
- `EntityName`: Product
- `ControllerName`: ProductsController
- `Route`: /proizv/lista
- `ViewPath`: Views/Products/Index.cshtml

Minimal code snippets
---------------------
Controller action:

```
public IActionResult Index(string? q, int page = 1)
{
    var items = _context.Products
        .Where(p => q == null || p.Name.Contains(q))
        .OrderBy(p => p.Name)
        .Skip((page-1)*PageSize)
        .Take(PageSize)
        .ToList();

    return View(items);
}
```

View header:

```
@model IEnumerable<Project.Models.ProductViewModel>
<h2>Products</h2>
```

Notes and conventions
---------------------
- Place list views under `Views/<ControllerName>/Index.cshtml` unless you need a custom name.
- Use attribute routing for localized short paths (`/proizv/lista`) to match existing app style.
- Keep the skill file up to date with any project-wide view model conventions.

Use this SKILL.md as the canonical checklist when creating or reviewing new list pages.
