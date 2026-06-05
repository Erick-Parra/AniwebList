# DESIGN.md

Guía de diseño visual del proyecto. Toda UI generada debe seguir estas reglas.
Estilo objetivo: **minimalista y limpio**, paleta **azul/morado (tipo AniList)**.

## Principios de diseño

- **Minimalismo**: espacio en blanco generoso, pocos elementos por pantalla, jerarquía clara.
- **Contenido primero**: la portada del anime es la protagonista. La UI no compite con ella.
- **Consistencia**: mismos espaciados, mismos radios de borde, mismos colores en toda la app.
- **Sin ruido visual**: evitar sombras pesadas, gradientes llamativos o bordes innecesarios.
- **Legibilidad**: buen contraste, tamaños de fuente cómodos.

## Paleta de colores

Definir como CSS variables (en `wwwroot/css/app.css` o similar). Tema claro por defecto + soporte para oscuro.

```css
:root {
  /* Marca (azul/morado) */
  --color-primary: #6366f1;        /* indigo principal */
  --color-primary-hover: #4f46e5;
  --color-accent: #8b5cf6;         /* violeta de acento */

  /* Superficies (tema claro) */
  --color-bg: #f8f9fc;             /* fondo general */
  --color-surface: #ffffff;        /* tarjetas, paneles */
  --color-border: #e5e7eb;

  /* Texto */
  --color-text: #1f2333;           /* texto principal */
  --color-text-muted: #6b7280;     /* secundario */

  /* Estados (WatchStatus) */
  --status-watching: #6366f1;      /* azul/indigo */
  --status-completed: #22c55e;     /* verde */
  --status-plan: #8b5cf6;          /* violeta */
  --status-onhold: #f59e0b;        /* ámbar */
  --status-dropped: #ef4444;       /* rojo */

  /* Funcionales */
  --color-success: #22c55e;
  --color-warning: #f59e0b;
  --color-danger: #ef4444;
}

/* Tema oscuro */
[data-theme="dark"] {
  --color-bg: #0f1117;
  --color-surface: #1a1d29;
  --color-border: #2a2e3d;
  --color-text: #e8eaf0;
  --color-text-muted: #9aa0b0;
}
```

**Regla de uso del color**: el azul/morado es para acciones y marca (botones, links, estado activo). NO usar color para texto de cuerpo. Los colores de estado solo en badges/etiquetas de WatchStatus.

## Tipografía

- Fuente: sistema sans-serif moderno (`Inter`, `system-ui`, o similar).
- Escala (rem):
  - `--text-xs: 0.75rem` (badges, metadatos)
  - `--text-sm: 0.875rem` (secundario)
  - `--text-base: 1rem` (cuerpo)
  - `--text-lg: 1.25rem` (títulos de tarjeta)
  - `--text-xl: 1.5rem` (títulos de sección)
  - `--text-2xl: 2rem` (título de página)
- Pesos: 400 (normal), 500 (medio), 600 (semibold para títulos). Evitar 700+ salvo casos puntuales.

## Espaciado y layout

- Sistema de espaciado en múltiplos de 4px: `4, 8, 12, 16, 24, 32, 48, 64`.
- Radio de borde: `--radius-sm: 6px`, `--radius-md: 10px`, `--radius-lg: 16px`.
- Ancho máximo de contenido: `1200px`, centrado, con padding lateral de 16-24px.
- Sombras suaves y sutiles únicamente:
  - `--shadow-sm: 0 1px 2px rgba(0,0,0,0.05)`
  - `--shadow-md: 0 4px 12px rgba(0,0,0,0.08)`

## Componentes clave

### AnimeCard (tarjeta de anime)
- Portada (`ImageUrl`) en relación de aspecto ~2:3, esquinas redondeadas (`--radius-md`).
- Título debajo (`--text-lg`, máx 2 líneas con ellipsis).
- Badge de `WatchStatus` (color según estado) en esquina o bajo el título.
- Score con icono de estrella si existe (`--text-sm`, color muted).
- Hover: leve elevación (`--shadow-md`) y transición suave (150ms).

### Grid de animes
- Grid responsivo: `repeat(auto-fill, minmax(160px, 1fr))`, gap de 16-24px.
- En móvil: 2-3 columnas; en escritorio: 5-6 columnas.

### Badge de estado
- Pequeño, texto `--text-xs`, padding 4px 8px, radio `--radius-sm`.
- Color de fondo tenue (color de estado al ~15% de opacidad) + texto del color de estado.

### Botones
- **Primario**: fondo `--color-primary`, texto blanco, hover `--color-primary-hover`.
- **Secundario**: fondo transparente, borde `--color-border`, texto `--color-text`.
- Radio `--radius-md`, padding 8px 16px, transición 150ms.

### Inputs / búsqueda
- Fondo `--color-surface`, borde `--color-border`, radio `--radius-md`.
- Focus: borde `--color-primary` + ring sutil.
- La barra de búsqueda de animes es un elemento destacado (es la acción principal del MVP).

### Navbar
- Limpia, fondo `--color-surface`, borde inferior `--color-border`.
- Logo/nombre a la izquierda, búsqueda al centro, avatar/usuario a la derecha.
- Sticky en scroll.

## Estructura de páginas (MVP)

1. **Login / Registro**: centrado, formulario simple sobre `--color-bg`, marca arriba.
2. **Mi lista**: grid de AnimeCards + filtros por WatchStatus (tabs o chips arriba).
3. **Búsqueda**: barra de búsqueda + resultados en grid + botón "agregar a mi lista".
4. **Detalle de anime**: portada grande + sinopsis + controles del usuario (estado, progreso, score, favorito).

## Estados de UI (no olvidar)

- **Loading**: skeletons con la forma de las tarjetas (no spinners genéricos a pantalla completa).
- **Vacío**: mensaje amable + ilustración/icono simple + CTA (ej: "Aún no tienes animes. ¡Busca uno!").
- **Error**: mensaje claro, no técnico, con opción de reintentar.

## Accesibilidad

- Contraste mínimo AA en texto.
- Foco visible en elementos interactivos.
- Imágenes con `alt` (título del anime).
- Targets táctiles de al menos 44x44px en móvil.

## Notas para Claude

- Toda UI generada en Blazor debe usar estas CSS variables, nunca colores hardcodeados.
- Priorizar componentes reutilizables (AnimeCard, StatusBadge, Button) sobre estilos repetidos.
- Mantener el minimalismo: ante la duda, quitar antes que agregar.