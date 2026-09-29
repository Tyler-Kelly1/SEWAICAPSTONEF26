# UI Design System & Component Standards

> [!IMPORTANT]
> This document defines the **authoritative UI design system, color scheme, typography, and visual philosophy** for the Overload application. All frontend engineers and AI agents **MUST** strictly adhere to these standards when creating or modifying user interfaces.
>
> Source Reference: Derived directly from [`DESIGN.md`](file:///C:/Users/tyler/Overload/design_docs/UI_Mocks/DESIGN.md) and [`sessionMock.html`](file:///C:/Users/tyler/Overload/design_docs/UI_Mocks/sessionMock.html).
> Architectural Context: Must be used alongside [`rules.md`](file:///C:/Users/tyler/Overload/rules.md) and [`architecture.md`](file:///C:/Users/tyler/Overload/architecture.md).

---

## 📖 1. Design Manifesto & Core UI Philosophy

### The "Yellow Pad Pencil Log" / "Analog Gym Journal"
Overload rejects the sterile, homogenized, hyper-rounded SaaS aesthetic of modern mobile apps. Strength training and progressive overload are tactile, grounded, and gritty disciplines. Athletes have relied on yellow legal pads, battered spiral notebooks, grease pencils, and beat-up paper logs on the chalk-dusted gym floor for decades because **they are fast, legible, direct, and distraction-free**.

Our design philosophy combines **Analog Tactility** with **Digital Precision**:
1. **Utilitarian Neobrutalism:** Hard, solid-ink borders (1.5px–2px), zero-blur drop shadows, and high-contrast contrast ratios make every button and container feel like physical cardboard or heavy stock paper.
2. **Hand-Drawn Imperfection & Textures:** Hand-drawn border curves, pencil crosshatch fills, and solid clean divider lines evoke an architect's or lifter's worn legal notepad.
3. **Mechanical Tactility:** Every interactive element has mechanical feedback—clicking or tapping visibly depresses the element (`active:translate-x-0.5 active:translate-y-0.5`) and collapses its cast shadow (`active:shadow-none`), feeling like a physical rubber stamp or mechanical switch.
4. **Handwritten Notebook Rigor:** All titles, sets, weights, reps, RPE ratings, and timers use the authentic handwritten *Patrick Hand* font to recreate an authentic gym journal scribbled down between heavy sets.
5. **Gym-Floor Ergonomics (Mobile-First):** High contrast outdoors and under harsh fluorescent gym lights; warm yellow-buff paper (`#fff8ef` background, `#f6ebd0` surfaces) eliminates harsh screen glare; primary controls remain strictly within thumb reach.

---

## 🎨 2. Global Color Palette & Semantic System

The color system uses a Material 3 semantic structure customized for the **Yellow Pad Pencil Log** aesthetic from [`DESIGN.md`](file:///C:/Users/tyler/Overload/design_docs/UI_Mocks/DESIGN.md). The palette strictly eliminates sterile pure whites and cold digital grays, favoring authentic yellow legal pad tones, manila cardstock, deep graphite charcoal, and bold cardinal red correction pencil accents.

> [!IMPORTANT]
> **No Sterile Pure White Rule:** Sterile `#ffffff` is strictly forbidden across the interface. All canvases, card surfaces, active input wells, and light-toned badges MUST use the **Yellow Pad palette** (`#f6ebd0` fine drywall textured canvas matching panels, `#fcf5dc` active input wells) to recreate an authentic tactile gym atmosphere.

### 2.1 Color Tokens Specification

| Token Category | Token Name | Hex Code | Visual Swatch / RGB | Semantic Description & Usage |
| :--- | :--- | :--- | :--- | :--- |
| **Legal Pad & Manila** | `background` | `#f6ebd0` | `rgb(246, 235, 208)` | Default application canvas; warm off-white fine drywall stippled background matching panels |
| | `surface` | `#f6ebd0` | `rgb(246, 235, 208)` | Default card and component surface |
| | `surface-bright` | `#fbf2d3` | `rgb(251, 242, 211)` | Highlighted paper, elevated headers |
| | `surface-container-lowest` | `#fcf5dc` | `rgb(252, 245, 220)` | Pale warm paper; active inputs (NO sterile `#ffffff`) |
| | `surface-container-low` | `#f4e7c5` | `rgb(244, 231, 197)` | Command bars, subtle card sections, table row highlights |
| | `surface-container` | `#f0e1b9` | `rgb(240, 225, 185)` | Secondary buttons, card internal wells |
| | `surface-container-high` | `#ebdcaf` | `rgb(235, 220, 175)` | Status tapes, auxiliary tags (`30° BENCH`) |
| | `surface-container-highest`| `#e8d89f` | `rgb(232, 216, 159)` | Active navigation tabs, secondary badge containers |
| | `surface-variant` | `#ede2c7` | `rgb(237, 226, 199)` | Neutral surface variant |
| | `surface-dim` | `#ebdca6` | `rgb(235, 220, 166)` | Recessed wells, dimmed containers |
| **Pencil Ink & Graphite** | `primary` | `#1f1d18` | `rgb(31, 29, 24)` | Deep graphite black ink; primary headings, borders, buttons |
| | `primary-container` | `#2c2820` | `rgb(44, 40, 32)` | Warm charcoal container; hard shadow color (`#2c2820`) |
| | `on-primary` | `#fefbe8` | `rgb(254, 251, 232)` | High-contrast light yellow text on primary buttons |
| | `on-surface` | `#1f1d17` | `rgb(31, 29, 23)` | High-contrast body text and readable typography |
| | `on-surface-variant` | `#4a4639` | `rgb(74, 70, 57)` | Muted pencil graphite text, secondary notes, captions |
| | `outline` | `#786f58` | `rgb(120, 111, 88)` | Medium pencil sketch lines, placeholder text |
| | `outline-variant` | `#cdbe8d` | `rgb(205, 190, 141)` | Weathered paper grid lines, solid divider rules |
| | `surface-tint` | `#615e58` | `rgb(97, 94, 88)` | Mid-tone graphite shading |
| **Aged Earth & Secondary** | `secondary` | `#575141` | `rgb(87, 81, 65)` | Desaturated earth secondary for technical annotations |
| | `secondary-container` | `#eedea8` | `rgb(238, 222, 168)` | Warm manila chips and tags |
| | `on-secondary-container` | `#4a4533` | `rgb(74, 69, 51)` | Text on secondary tinted containers |
| | `on-secondary` | `#ffffff` | `rgb(255, 255, 255)` | Text on solid secondary buttons |
| **Cardinal Red Accent & Status** | `error` | `#b91c1c` | `rgb(185, 28, 28)` | Vibrant cardinal red; active timers, alerts, warning cues |
| | `error-container` | `#fcdad7` | `rgb(252, 218, 215)` | Warning / error card background |
| | `on-error` | `#ffffff` | `rgb(255, 255, 255)` | Text on red alert buttons |
| | `on-error-container` | `#93000a` | `rgb(147, 0, 10)` | Dark red text on error containers |
| **Dark Theme Inversions** | `inverse-surface` | `#36301e` | `rgb(54, 48, 30)` | Dark mode container surface |
| | `inverse-on-surface` | `#fbf0d5` | `rgb(251, 240, 213)` | Pale yellow pad text in dark mode |
| | `inverse-primary` | `#cbc6be` | `rgb(203, 198, 190)` | Muted chalk white for dark mode primary accents |

---

## ✍️ 3. Typography Hierarchy & Font Token Specs

The typography system is unified under the authentic handwritten typeface **Patrick Hand** as defined in [`DESIGN.md`](file:///C:/Users/tyler/Overload/design_docs/UI_Mocks/DESIGN.md):

```
┌─────────────────────────────────────────────────────────────┐
│ PATRICK HAND → Heavy Headings, Body Notes, & Data Telemetry │
└─────────────────────────────────────────────────────────────┘
```

### 3.1 Typeface Specification
- **Unified Font Family:** `Patrick Hand`, cursive, sans-serif
- **Headlines (`font-headline-*`):** `Patrick Hand` (Bold / Weight 700) - Expressive, stamped lead headings.
- **Body (`font-body-*`):** `Patrick Hand` (Regular / Weight 400) - Natural handwritten journal notes and descriptions.
- **Data / Labels (`font-label-*` / `font-mono`):** `Patrick Hand` (Weight 700 / 400) - Logged numbers, sets, weights, timestamps, and indexes `[01]`.

### 3.2 Typography Scale Token Reference (from `DESIGN.md`)

| Token Name | Font Family | Size | Line Height | Letter Spacing | Weight | Typical Use Case |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `headline-xl` | Patrick Hand | 40px | 48px | `-0.02em` | 700 (Bold) | Desktop session header, primary splash title |
| `headline-xl-mobile` | Patrick Hand | 30px | 38px | `-0.01em` | 700 (Bold) | Mobile session title |
| `headline-lg` | Patrick Hand | 28px | 36px | normal | 700 (Bold) | Major section headings, routine titles |
| `headline-lg-mobile` | Patrick Hand | 23px | 30px | normal | 700 (Bold) | App bar header (`SCRATCHPAD`) |
| `headline-md` | Patrick Hand | 21px | 28px | normal | 700 (Bold) | Exercise names (`BARBELL BACK SQUAT`), Rest countdown |
| `body-lg` | Patrick Hand | 19px | 28px | normal | 400 (Regular) | Exercise execution tips, long descriptions |
| `body-md` | Patrick Hand | 17px | 24px | normal | 400 (Regular) | Default interface text, list items |
| `body-sm` | Patrick Hand | 15px | 20px | normal | 400 (Regular) | Subtitles, auxiliary information |
| `label-md` | Patrick Hand | 15px | 18px | `+0.03em` | 700 (Bold) | Set row data (`SET 1: 32kg × 10`), quick note input |
| `label-sm` | Patrick Hand | 13px | 16px | `+0.04em` | 400 (Regular) | Table headers, chip buttons, timestamps (`⏱ 42:15`), tags |

---

## 🧱 4. Elevation, Borders & Tactile Neobrutalism System

Modern floating shadows with large blur radiuses (`box-shadow: 0 10px 25px rgba(...)`) are **strictly prohibited**. Instead, depth is achieved through **physical cutouts, hard offset drop shadows, and sketch borders**.

### 4.1 Hard Offset Drop Shadows
Depth is communicated through crisp, unblurred drop shadows using `#27272a` (Light Mode) or `#c7c6cb` (Dark Mode):

```css
/* Neobrutalist Shadow Tokens */
--shadow-sm:     1px 1px 0px 0px #27272a;  /* Small pills, badges, checkboxes */
--shadow-btn:    2px 2px 0px 0px #27272a;  /* Command bar buttons, interactive action buttons */
--shadow-top:    0px -2px 0px 0px #27272a; /* Bottom navigation bar top edge */
--shadow-bottom: 0px 2px 0px 0px #27272a;  /* Sticky header bar bottom edge */
```

> [!IMPORTANT]
> **No Shadows on Cards Rule:** Cards and container frames **MUST NOT** have drop shadows (`box-shadow: none`). Structural cards are grounded directly onto the canvas with crisp solid ink borders (`1.5px–2px solid #121315`). Drop shadows are reserved exclusively for interactive buttons, chips, and tactile click states, never structural cards.

### 4.2 Tactile Click / Active State (The Stamp Effect)
All interactive buttons, chips, and cards must simulate a physical press down into the page:
```css
/* Interactive Active State */
.btn-tactile:active {
  transform: translate(0.125rem, 0.125rem); /* translate-x-0.5 translate-y-0.5 (2px) */
  box-shadow: none !important;
}
```

### 4.3 Asymmetrical Sketch Borders
To evoke a notebook drawn with a ruler and technical pen, borders use subtle asymmetrical radii:

```css
/* Standard Sketch Border (Cards, Containers) */
.sketch-border {
  border: 2px solid #27272a;
  border-radius: 4px 6px 3px 7px / 6px 3px 5px 4px;
}

/* Small Sketch Border (Pills, Chips, Inputs, Badges) */
.sketch-border-sm {
  border: 1.5px solid #27272a;
  border-radius: 3px 4px 2px 5px / 4px 2px 4px 3px;
}
```

---

## 📐 5. Textures, Patterns & Visual Metaphors

The design incorporates physical sketchbook techniques as functional UI states:

### 5.1 Pencil Crosshatching (`.pencil-hatch` & `.pencil-hatch-dense`)
Pencil hatching is used to denote:
- Exercise category header strips
- Completed or shaded workout sets
- Active chip filters
- Visual anchors for quick-entry tools

```css
/* Light 45-degree Pencil Crosshatching */
.pencil-hatch {
  background: repeating-linear-gradient(
    -45deg,
    rgba(39, 39, 42, 0.12),
    rgba(39, 39, 42, 0.12) 2px,
    transparent 2px,
    transparent 6px
  );
}

/* Dense Pencil Crosshatching (High intensity / heavy shading) */
.pencil-hatch-dense {
  background: repeating-linear-gradient(
    -45deg,
    rgba(39, 39, 42, 0.25),
    rgba(39, 39, 42, 0.25) 2px,
    transparent 2px,
    transparent 4px
  );
}
```

### 5.2 Solid Rule Dividers & Prohibition of Dotted/Dashed Lines
> [!IMPORTANT]
> **No Dotted or Dashed Lines Rule:** Dotted and dashed lines are strictly removed from the design system to eliminate visual clutter and ensure clean architectural hierarchy. All dividers, section splits, and card internal rules must be clean **solid lines** (e.g. `border-b border-outline-variant` or `border-b border-primary`).

### 5.3 Technical Wireframe Box
Used for exercise diagram placeholders, equipment muscle maps, or technical schematics:
```css
.wireframe-box {
  position: relative;
  background: #faf9f5;
}
.wireframe-box::before {
  content: "";
  position: absolute;
  inset: 0;
  pointer-events: none;
  background: linear-gradient(to top right, transparent calc(50% - 1px), #71717a calc(50% - 0.5px), #71717a calc(50% + 0.5px), transparent calc(50% + 1px));
}
.wireframe-box::after {
  content: "";
  position: absolute;
  inset: 0;
  pointer-events: none;
  background: linear-gradient(to bottom right, transparent calc(50% - 1px), #71717a calc(50% - 0.5px), #71717a calc(50% + 0.5px), transparent calc(50% + 1px));
}
```

### 5.4 Physical Badges, Rotations & Tally Marks
- **Angled Stamps:** Badges such as `★ PR!` or urgent alerts feature a slight rotation (`transform -rotate-1` or `-rotate-2`) as if stamped by hand.
- **Tally Marks:** Repetition tracking can use tally marks (`|||| /`) alongside numeric totals to reinforce the scratchpad metaphor.
- **Checked Stamps:** Completed set checkboxes are represented as solid ink stamped boxes (`[✓]`).

---

## 🧩 6. Core Component Standard Blueprints

### 6.1 Top Sticky App Bar
- **Position:** Sticky at top (`sticky top-0 z-40`).
- **Styling:** Surface background, thick bottom border (`border-b-2 border-primary`), hard downward drop shadow (`shadow-[0px_2px_0px_0px_#27272a]`).
- **Content:**
  - Left: Brand icon / sketchbook icon (`draw` or `edit_note`), title (`SCRATCHPAD`), and session counter (`// LOG #42`).
  - Right: Elapsed workout timer badge in `sketch-border-sm` (`⏱ 42:15`), overflow menu button (`more_vert`).

### 6.2 Rapid Note Entry / Command Bar
- **Purpose:** Fast unstructured or structured logging without tedious multi-step modal dialogs.
- **Structure:**
  - Outer container with `sketch-border` and `shadow-[2px_2px_0px_0px_#27272a]`.
  - Header: Small uppercase monospaced label (`RAPID NOTE ENTRY`) with a solid horizontal rule divider.
  - Input: Monospaced input field with pencil glyph (`✏️`), enclosed in a pure white (`bg-surface-container-lowest`) inset box.
  - Action Button: Solid primary button `[ + ADD ]` with tactile click depression.
  - Quick Chips Row: Pill buttons with `sketch-border-sm` and `.pencil-hatch` textures (`+ SET`, `+ DROPSET`, `+ REST 2M`, `⚡ RPE 9.5`).

### 6.3 Exercise Logging Cards
- **Structure:**
  - Card Header: `border-t-2 border-b-2 border-primary` with `.pencil-hatch` background. Contains exercise index (e.g., `[01]`), uppercase title (`BARBELL BACK SQUAT`), and badge/equipment tag.
  - Set Table: High-density tabular layout.
    - Columns: `SET`, `WEIGHT`, `REPS`, `RPE`, `DONE`.
    - Warm-up sets: Indicated with `(w)` and lighter graphite text.
    - Working / PR sets: Bold monospace text with subtle background tinting (`bg-surface-container-low/20`).
    - Completed Checkbox: Square stamp with checkmark icon (`✓`).
  - Footer / Notes Strip: Perforated dashed border with sticky note icon (`sticky_note_2`) and italicized user comments.

### 6.4 Floating Rest Timer Bar
- **Position:** Docked above the bottom navigation bar (`fixed bottom-14 left-0 w-full z-40`).
- **Styling:** Enclosed in `sketch-border` with dual upward and downward shadows (`shadow-[0px_-2px_0px_0px_#27272a,2px_2px_0px_0px_#27272a]`).
- **Features:**
  - Pulsing red indicator dot (`w-3 h-3 rounded-full bg-error animate-pulse`).
  - Monospaced countdown display (`01:30`) in `font-headline-md font-bold`.
  - Quick control buttons: Increment `+30s` (hatched button) and `SKIP >>` (solid ink button).

### 6.5 Bottom Sticky Navigation Bar
- **Position:** Fixed at bottom (`fixed bottom-0 left-0 w-full z-50`).
- **Styling:** Thick top border (`border-t-2 border-primary`), upward shadow (`shadow-[0px_-2px_0px_0px_#27272a]`).
- **Destinations:**
  1. `Scratchpad` (Active: elevated with `border border-primary` and `shadow-[1px_1px_0px_0px_#27272a]`).
  2. `Log` (`history_edu`).
  3. `Routines` (`fitness_center`).
  4. `Settings` (`settings`).

---

## 💻 7. Vuetify & Frontend Implementation Architecture

In accordance with [`architecture.md`](file:///C:/Users/tyler/Overload/architecture.md), all frontend components must strictly adhere to the Vue SFC sequence (`<script>`, `<template>`, `<style>`). Below is how Vuetify 3 and CSS must be configured to realize these standards.

### 7.1 Vuetify Theme Configuration (`vuetify.js`)
Vuetify's theme colors must match the semantic color tokens:

```javascript
// src/plugins/vuetify.js
import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'

const scratchpadLightTheme = {
  dark: false,
  colors: {
    background: '#faf9f5',
    surface: '#faf9f5',
    'surface-bright': '#faf9f5',
    'surface-variant': '#e3e2df',
    primary: '#121315',
    'primary-container': '#27272a',
    secondary: '#5d5e66',
    'secondary-container': '#e3e1ec',
    error: '#ba1a1a',
    'error-container': '#ffdad6',
    outline: '#77767b',
    'outline-variant': '#c7c6cb',
    'on-background': '#1b1c1a',
    'on-surface': '#1b1c1a',
    'on-primary': '#ffffff',
  },
}

export default createVuetify({
  theme: {
    defaultTheme: 'scratchpadLightTheme',
    themes: {
      scratchpadLightTheme,
    },
  },
})
```

### 7.2 Global CSS Tokens (`src/style.css` / Global Styles)
These classes should be imported globally so all components can immediately reuse the sketch textures:

```css
@import url('https://fonts.googleapis.com/css2?family=Patrick+Hand&display=swap');

:root {
  --color-paper: #faf9f5;
  --color-ink: #121315;
  --color-charcoal: #27272a;
  --color-pencil: #77767b;
  --color-pencil-light: #c7c6cb;
  --color-accent-red: #ba1a1a;
}

/* Typography Classes */
.font-headline,
.font-body,
.font-mono,
.font-label {
  font-family: 'Patrick Hand', cursive, sans-serif !important;
}

/* Neobrutalist Sketch Borders */
.sketch-border {
  border: 2px solid #27272a;
  border-radius: 4px 6px 3px 7px / 6px 3px 5px 4px;
}

.sketch-border-sm {
  border: 1.5px solid #27272a;
  border-radius: 3px 4px 2px 5px / 4px 2px 4px 3px;
}

/* Hard Drop Shadows */
.shadow-hard-sm { box-shadow: 1px 1px 0px 0px #27272a; }
.shadow-hard-md { box-shadow: 2px 2px 0px 0px #27272a; }
.shadow-hard-top { box-shadow: 0px -2px 0px 0px #27272a; }
.shadow-hard-bottom { box-shadow: 0px 2px 0px 0px #27272a; }

/* Tactile Active Press */
.tactile-btn {
  transition: transform 0.08s ease, box-shadow 0.08s ease;
}
.tactile-btn:active {
  transform: translate(1.5px, 1.5px);
  box-shadow: 0px 0px 0px 0px #27272a !important;
}

/* Pencil Textures */
.pencil-hatch {
  background: repeating-linear-gradient(
    -45deg,
    rgba(39, 39, 42, 0.12),
    rgba(39, 39, 42, 0.12) 2px,
    transparent 2px,
    transparent 6px
  );
}

.pencil-hatch-dense {
  background: repeating-linear-gradient(
    -45deg,
    rgba(39, 39, 42, 0.25),
    rgba(39, 39, 42, 0.25) 2px,
    transparent 2px,
    transparent 4px
  );
}
```

---

## 🚫 8. Standards Checklist: Anti-Patterns vs. Golden Rules

### Strict Anti-Patterns (NEVER DO)
- ❌ **NO Sterile Pure White (`#ffffff`):** Stark pure white `#ffffff` is prohibited for general surfaces, card bodies, and input wells. Use grunge off-white/yellow tokens (`#f2ecd9`, `#faf6ea`).
- ❌ **NO Shadows on Cards:** Cards, panels, and dossier frames must **NOT** have drop shadows (`box-shadow: none`). Shadows are restricted strictly to interactive clickable controls (buttons, chips).
- ❌ **NO Dotted or Dashed Lines:** Dotted or dashed borders (`border-dotted`, `border-dashed`) are forbidden. All dividers and section lines must be clean solid rules.
- ❌ **NO Blurry Soft Shadows:** Do not use Tailwind `shadow-lg`, `shadow-xl`, or CSS `box-shadow: 0 10px 20px rgba(...)`. Shadows must be solid ink `#27272a` with zero blur.
- ❌ **NO Pill-Shaped Generic SaaS Buttons:** Do not use `rounded-full` for standard action buttons or modals unless specifically styling circular icon buttons or radio indicators.
- ❌ **NO Gradient Button Backgrounds:** Do not use multi-color linear gradients (`bg-gradient-to-r from-blue-500 to-purple-600`). Use flat solid ink or `.pencil-hatch` fills.
- ❌ **NO Low-Contrast Gray Text:** Do not use light gray text (`#9ca3af` or `#d1d5db`) for primary reading content on paper backgrounds. High contrast is required.
- ❌ **NO Proportionally Spaced Numbers in Workout Tables:** Workout telemetry (sets, weight, reps, RPE, timers) must NEVER be formatted in standard generic sans-serif; it must consistently use the defined `Patrick Hand` font formatting.

### Golden Rules (ALWAYS ENFORCE)
- ✅ **ALWAYS Use Weathered Grunge Paper Tones:** Render interfaces with aged yellowed paper and manila tones (`#f2ecd9`, `#faf6ea`) to emulate authentic physical training journals.
- ✅ **ALWAYS Keep Structural Cards Shadowless:** Ground cards directly onto the canvas with 1.5px–2px solid ink borders and zero box-shadow (`box-shadow: none`).
- ✅ **ALWAYS Use Clean Solid Dividers:** Use solid rules (`border-b border-outline-variant` or `border-primary`) for all internal line separators.
- ✅ **ALWAYS Include Tactile Click States:** Every clickable element must have an active displacement (`translate-x-0.5 translate-y-0.5`).
- ✅ **ALWAYS Retain Solid Ink Borders:** Major containers, table headers, and app bars must have clean, visible solid ink borders (1.5px to 2px).
- ✅ **ALWAYS Use Patrick Hand Font:** Unified `Patrick Hand` across all titles, body notes, and data labels for an authentic analog journal aesthetic.
- ✅ **ALWAYS Test in High Ambient Light:** The grunge paper background (`#f2ecd9`) and graphite ink (`#121315`) must remain easily legible on mobile screens outdoors or under fluorescent gym fixtures.
