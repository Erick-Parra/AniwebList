# ROADMAP.md

Plan de desarrollo por fases para Claude Code. Cada fase incluye un prompt listo para pegar
y cómo verificar que quedó bien antes de pasar a la siguiente.

## Cómo usar este documento

- **Una fase a la vez.** Trabaja, verifica, haz `git commit`, y recién entonces sigue.
- **Cada prompt empieza con "Lee CLAUDE.md"** para que Claude Code use el contexto del proyecto
  en lugar de inventar convenciones.
- **No pidas todo de golpe.** El contexto se degrada y se mezclan responsabilidades.
- **Si una fase queda a medias**, continúa con: "sigue donde quedaste".
- **Stack confirmado**: .NET 10 (LTS, C# 14), ASP.NET Core Web API, Blazor, SQL Server, EF Core, Jikan API.

---

## Fase 0 — Estructura de la solución

**Prompt:**

```
Lee CLAUDE.md antes de empezar. Crea la estructura base de la solución en .NET 10 (C# 14) siguiendo la separación de capas descrita:

- Un archivo .sln
- Proyecto Web API (ASP.NET Core, .NET 10): AnimeList.Api
- Proyecto de lógica/dominio: AnimeList.Core (entidades, enums, DTOs, interfaces de servicios)
- Proyecto de acceso a datos: AnimeList.Infrastructure (DbContext, EF Core, repositorios, cliente Jikan)
- El frontend Blazor lo agregaremos en una fase posterior, no lo crees aún

Configura las referencias entre proyectos correctamente (Api → Core + Infrastructure; Infrastructure → Core).
Habilita nullable en todos los proyectos. No agregues lógica todavía, solo el esqueleto que compile.
Al terminar, dime cómo verificar que compila.
```

**Verificar:** `dotnet build` sin errores.

---

## Fase 1 — Entidades, enums y DbContext

**Prompt:**

```
Lee CLAUDE.md. Implementa el modelo de datos exactamente como está descrito ahí:

- Enums: WatchStatus y AnimeAirStatus (en AnimeList.Core)
- Entidad Anime (con MalId como identificador externo de Jikan, sin campo Source)
- Entidad UserAnimeEntry (tabla central)
- ApplicationUser : IdentityUser con su colección de AnimeEntries

Luego en AnimeList.Infrastructure:
- AppDbContext heredando de IdentityDbContext<ApplicationUser>
- Configuración Fluent API: índice único en Anime.MalId, índice único compuesto en (UserId, AnimeId) de UserAnimeEntry, y las relaciones con sus DeleteBehavior apropiados

No crees la migración todavía, solo el código. Dime si compila.
```

**Verificar:** compila.

---

## Fase 2 — Base de datos y migración inicial

**Prompt:**

```
Lee CLAUDE.md. Configura la conexión a SQL Server:

- Registra AppDbContext en el contenedor de DI del proyecto Api
- Usa la connection string desde User Secrets (NUNCA hardcodeada, según CLAUDE.md)
- Crea la migración inicial de EF Core
- Dame los comandos exactos para: configurar el secret de la connection string, generar la migración y aplicarla a la base de datos
```

**Verificar:** la migración se aplica y se crean las tablas en SQL Server.

---

## Fase 3 — Cliente de Jikan + cache

**Prompt:**

```
Lee CLAUDE.md, sección "Integración con Jikan API". Implementa en AnimeList.Infrastructure un servicio que consuma Jikan:

- IJikanService (interfaz en Core) con: BuscarAnimesAsync(query) y ObtenerAnimePorMalIdAsync(malId)
- Implementación usando IHttpClientFactory (registrar el HttpClient con la base URL de Jikan)
- Mapear la respuesta de Jikan a la entidad Anime / a un DTO
- Lógica de cache: antes de llamar a Jikan, buscar en la BD por MalId; respetar la política de refresco descrita en CLAUDE.md
- Respetar el rate limit (~3 req/s) con throttling o retry

Maneja los errores de red de forma limpia. Dime cómo probarlo.
```

**Verificar:** una búsqueda real devuelve resultados y los cachea en la BD.

---

## Fase 4 — Autenticación

**Prompt:**

```
Lee CLAUDE.md. Implementa autenticación con ASP.NET Core Identity en el Api:

- Endpoints de registro y login que devuelvan un JWT
- Configurar la autenticación JWT en el pipeline
- Las claves/secretos del JWT desde configuración, nunca hardcodeados
- Proteger los endpoints que lo requieran con [Authorize]

Dime cómo probar el registro y login (por ejemplo con curl o un .http file).
```

**Verificar:** registro + login devuelven un token JWT válido.

---

## Fase 5 — Endpoints de la lista (el corazón del MVP)

**Prompt:**

```
Lee CLAUDE.md. Implementa los endpoints de la lista de animes del usuario, respetando la separación en capas (Controller → Service → Repository) y usando DTOs:

- Agregar un anime a mi lista (por MalId; si no está en cache, traerlo de Jikan)
- Actualizar mi entrada: WatchStatus, EpisodesWatched, Score, IsFavorite
- Quitar un anime de mi lista
- Listar mi lista, con filtro opcional por WatchStatus

REGLA CRÍTICA (de CLAUDE.md): cada usuario solo puede ver y modificar SUS propias entradas. Obtén el userId del token, nunca del body.

Usa DTOs separados para request y response. Dame un archivo .http para probar todos los endpoints.
```

**Verificar:** el flujo completo (agregar, actualizar, listar, quitar) funciona con un usuario autenticado.

---

## Fase 6 — Frontend Blazor

**Prompt:**

```
Lee CLAUDE.md Y DESIGN.md (este último es obligatorio para toda UI). Crea el proyecto frontend Blazor (.NET 10) y agrégalo a la solución.

Implementa las páginas del MVP descritas en DESIGN.md:
- Login / Registro
- Mi lista (grid de AnimeCard + filtros por estado)
- Búsqueda (barra + resultados + botón agregar)
- Detalle de anime (controles de estado, progreso, score, favorito)

Usa las CSS variables de DESIGN.md (paleta azul/morado, minimalista), nunca colores hardcodeados.
Crea componentes reutilizables: AnimeCard, StatusBadge, Button.
El consumo del Api vía HttpClient con el token JWT.

Empieza SOLO por el login y la página "Mi lista". Las demás en pasos siguientes.
```

**Verificar:** login + ver tu lista desde el navegador.

**Pasos siguientes (mensajes cortos, uno por uno):**
- "Lee DESIGN.md. Ahora agrega la página de búsqueda de animes."
- "Lee DESIGN.md. Ahora agrega la página de detalle de anime con sus controles."
- "Agrega los estados de UI: loading (skeletons), vacío y error, según DESIGN.md."

---

## Después del MVP (backlog, fuera del alcance inicial)

- Reseñas por anime
- Listas personalizadas
- Estadísticas del usuario (horas vistas, géneros, conteos)
- Perfil público / compartir lista
- Tema oscuro (las variables ya están en DESIGN.md)

---

## Recordatorios de flujo de trabajo

- `git commit` al terminar (y verificar) cada fase.
- Si Claude Code se desvía de CLAUDE.md o DESIGN.md, recuérdaselo explícitamente.
- Mantener CLAUDE.md actualizado cuando se tomen decisiones nuevas.