# CLAUDE.md

Contexto del proyecto para Claude Code. Leer antes de generar o modificar código.

## Descripción del proyecto

Aplicación web para que cada usuario lleve su propia lista de animes (vistos, viendo, por ver, etc.), con calificación personal y favoritos. Los datos del catálogo de animes provienen de una API externa y se cachean en la base de datos local.

## Stack tecnológico

- **Backend / API**: ASP.NET Core (.NET 10) Web API, C#
- **Frontend**: Blazor
- **Base de datos**: SQL Server
- **ORM**: Entity Framework Core (con migraciones)
- **Autenticación**: ASP.NET Core Identity (`ApplicationUser : IdentityUser`)
- **Fuente de datos de animes**: Jikan API (API no oficial de MyAnimeList) + cache en la BD local

## Arquitectura

Separación en capas. No mezclar responsabilidades:

- **API (Controllers)**: solo reciben requests, validan y delegan. Sin lógica de negocio.
- **Services**: lógica de negocio (gestión de listas, consulta y cacheo de animes).
- **Repositories / EF Core**: acceso a datos.
- **DTOs**: nunca exponer entidades de EF directamente en los endpoints. Mapear entidad <-> DTO.

## Modelo de datos (entidades principales)

### Anime (catálogo cacheado)
- `Id` (PK interna), `MalId` (ID del anime en MyAnimeList, vía Jikan)
- `Title`, `TitleEnglish?`, `ImageUrl?`, `Synopsis?`, `EpisodeCount?`
- `AirStatus` (enum: Airing/Finished/NotYetAired), `Season?`, `Year?`
- `CachedAt` (para política de refresco del cache)
- Índice único: (`MalId`)

### UserAnimeEntry (tabla central: relación usuario <-> anime)
- `Id`, `UserId` (FK), `AnimeId` (FK)
- `WatchStatus` (enum: Watching, Completed, PlanToWatch, OnHold, Dropped)
- `EpisodesWatched`, `Score?` (1-10, personal del usuario), `IsFavorite`
- `StartedAt?`, `FinishedAt?`, `CreatedAt`, `UpdatedAt`
- Índice único: (`UserId`, `AnimeId`) — un usuario no repite el mismo anime

### ApplicationUser : IdentityUser
- Tiene colección `AnimeEntries`

**Importante**: `Score` e `IsFavorite` viven en `UserAnimeEntry`, NO en `Anime`. Son datos personales de cada usuario, no del anime global.

## Convenciones de código

- Nullability habilitado (`<Nullable>enable</Nullable>`); usar `?` apropiadamente.
- Async/await en todo acceso a BD y llamadas HTTP externas (sufijo `Async`).
- Inyección de dependencias para services y repositories.
- DTOs separados para request y response cuando difieran.
- Enums en C#, no strings mágicos para estados.

## Reglas de seguridad

- NUNCA hardcodear connection strings ni API keys en el código.
  Usar User Secrets en desarrollo y variables de entorno en producción.
- Configurar CORS en el API (el frontend Blazor corre en otro origen).
- Validar que un usuario solo pueda leer/modificar SUS propias entradas.

## Integración con Jikan API

- Jikan es la API REST no oficial de MyAnimeList. Base URL: `https://api.jikan.moe/v4`.
- No requiere API key.
- Respetar los límites de rate de Jikan (~3 req/s, ~60 req/min). Implementar throttling/retry.
- Antes de llamar a Jikan, revisar si el anime ya está en cache (por `MalId`).
- Endpoints principales: `/anime?q={query}` (búsqueda), `/anime/{id}` (detalle por MAL ID).
- Política de refresco: refrescar cache si pasaron X días, priorizando animes con `AirStatus = Airing` (su conteo de episodios cambia).
- Guardar el `MalId` como único para evitar duplicados en el catálogo.

## Alcance del MVP (en orden de prioridad)

1. Autenticación (registro / login).
2. Buscar anime (vía Jikan) y agregarlo a la lista del usuario.
3. Asignar estado (WatchStatus) y progreso de episodios.
4. Calificación (Score) y marcar como favorito.
5. Ver la lista propia filtrada por estado.

No implementar reseñas ni listas personalizadas todavía (fuera del MVP actual).

## Diseño visual

El sistema de diseño completo (paleta, tipografía, componentes, layout) está en **`DESIGN.md`**.
Consultarlo SIEMPRE antes de generar o modificar cualquier UI / componente Blazor.
Resumen: estilo minimalista y limpio, paleta azul/morado (tipo AniList), tema claro por defecto con soporte oscuro.

## Notas para Claude

- Al crear código nuevo, mantener la separación de capas descrita arriba.
- Para cualquier UI, seguir las reglas de `DESIGN.md` (usar CSS variables, nunca colores hardcodeados).
- Pedir confirmación antes de cambios estructurales grandes (migraciones que borren datos, renombrar entidades).
- Generar migraciones de EF Core cuando se modifique el modelo, pero no aplicarlas automáticamente a producción.