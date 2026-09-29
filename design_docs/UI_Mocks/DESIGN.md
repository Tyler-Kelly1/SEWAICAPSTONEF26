---
name: Yellow Pad Pencil Log
colors:
  surface: '#f6ebd0'
  surface-dim: '#ebdca6'
  surface-bright: '#fbf2d3'
  surface-container-lowest: '#fcf5dc'
  surface-container-low: '#f4e7c5'
  surface-container: '#f0e1b9'
  surface-container-high: '#ebdcaf'
  surface-container-highest: '#e8d89f'
  on-surface: '#1f1d17'
  on-surface-variant: '#4a4639'
  inverse-surface: '#36301e'
  inverse-on-surface: '#fbf0d5'
  outline: '#786f58'
  outline-variant: '#cdbe8d'
  surface-tint: '#615e58'
  primary: '#1f1d18'
  on-primary: '#fefbe8'
  primary-container: '#2c2820'
  on-primary-container: '#7a7360'
  inverse-primary: '#cbc6be'
  secondary: '#575141'
  on-secondary: '#ffffff'
  secondary-container: '#eedea8'
  on-secondary-container: '#4a4533'
  tertiary: '#040301'
  on-tertiary: '#ffffff'
  tertiary-container: '#201d17'
  on-tertiary-container: '#8a857c'
  error: '#b91c1c'
  on-error: '#ffffff'
  error-container: '#fcdad7'
  on-error-container: '#93000a'
  primary-fixed: '#e8e2d9'
  primary-fixed-dim: '#cbc6be'
  on-primary-fixed: '#1d1b16'
  on-primary-fixed-variant: '#494640'
  secondary-fixed: '#ebe2cc'
  secondary-fixed-dim: '#cfc6b1'
  on-secondary-fixed: '#1f1b0e'
  on-secondary-fixed-variant: '#4c4637'
  tertiary-fixed: '#e9e1d8'
  tertiary-fixed-dim: '#ccc6bc'
  on-tertiary-fixed: '#1e1b16'
  on-tertiary-fixed-variant: '#4a463f'
  background: '#f6ebd0'
  on-background: '#201b0b'
  surface-variant: '#ede2c7'
typography:
  headline-xl:
    fontFamily: Patrick Hand
    fontSize: 40px
    fontWeight: '700'
    lineHeight: 48px
    letterSpacing: -0.02em
  headline-xl-mobile:
    fontFamily: Patrick Hand
    fontSize: 30px
    fontWeight: '700'
    lineHeight: 38px
    letterSpacing: -0.01em
  headline-lg:
    fontFamily: Patrick Hand
    fontSize: 28px
    fontWeight: '700'
    lineHeight: 36px
  headline-lg-mobile:
    fontFamily: Patrick Hand
    fontSize: 23px
    fontWeight: '700'
    lineHeight: 30px
  headline-md:
    fontFamily: Patrick Hand
    fontSize: 21px
    fontWeight: '700'
    lineHeight: 28px
  body-lg:
    fontFamily: Patrick Hand
    fontSize: 19px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Patrick Hand
    fontSize: 17px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Patrick Hand
    fontSize: 15px
    fontWeight: '400'
    lineHeight: 20px
  label-md:
    fontFamily: Patrick Hand
    fontSize: 15px
    fontWeight: '700'
    lineHeight: 18px
    letterSpacing: 0.03em
  label-sm:
    fontFamily: Patrick Hand
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 16px
    letterSpacing: 0.04em
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  gutter: 1.25rem
  gutter-mobile: 0.75rem
  margin: 2rem
  margin-mobile: 1rem
  space-xs: 0.25rem
  space-sm: 0.5rem
  space-md: 1rem
  space-lg: 1.5rem
  space-xl: 2.5rem
---

## Brand & Style
The brand captures the raw, analog, and tactile feeling of an architect's or lifter's worn legal notepad. It combines brutalist neo-sketch aesthetics with physical utilitarianism: ink-drawn borders, pencil hatching, taped-on scraps, and deliberate manual imperfections.

Targeted at athletes, craftsmen, and lifters who prefer rapid shorthand, tactile feedback, and directness over overly polished, sterile software interfaces. The experience feels gritty, reliable, immediate, and intimate—evoking the focus of a graphite pencil scribbling down numbers between heavy sets.

## Colors
The color palette derives completely from vintage legal pads, manila cardstock, and graphite drafting tools. 

- **Surface & Canvas:** Warm yellow-buff papers (`#f6ebd0`) graded across subtle container depths (`#fbf2d3` down to `#ebdca6`), evoking varied paper densities and aged fibrous textures.
- **Pencil Ink & Primary:** Deep graphite black (`#1f1d18`), used for linework, high-contrast badges, and primary action surfaces.
- **Accents & Annotations:** Desaturated earth secondary (`#575141`) for technical annotations, with vibrant cardinal red (`#b91c1c`) strictly reserved for active timers, warnings, and urgent cues.

## Typography
Typography is authentic, hand-drawn, and disciplined, unified by Patrick Hand across all text:

- **Headlines:** Set in Patrick Hand with bold weights to simulate stamped or firmly pressed lead headings.
- **Body & Notes:** Set in Patrick Hand mimicking natural notebook handwriting and marginalia notes.
- **Labels & Metrics:** Set in Patrick Hand to ground logged numbers, weights, sets, timestamps, and bracketed indexes `[01]`.

## Layout & Spacing
The layout follows a centered column structure mimicking physical notebook proportions (max content width ~576px / `max-w-xl` on larger screens).

- **Grid & Alignment:** Compact single-column vertical flow segmented by ruled section dividers, dense tabular data rows, and split data columns (such as bilateral split sets).
- **Rhythm:** Dense, content-rich spacing (`space-sm` to `space-md` gaps) minimizing dead space to mimic packed notepad pages.
- **Responsiveness:** Margins contract down to `margin-mobile` (16px) on mobile viewports with docked control trays anchored to the top and bottom view bounds.

## Elevation & Depth
Elevation is rendered strictly flat, mechanical, and hard-edged rather than through blurred, diffuse drop shadows:

- **Hard Ink Drop Shadows:** 1px or 2px offset solid shadows (`1px 1px 0px 0px #232018` or `2px 2px 0px 0px #232018`) ground containers, floating banners, and interactive buttons.
- **Active Tactile States:** Elements translate 1px to 2px diagonally (`active:translate-x-0.5 active:translate-y-0.5`) with collapse of drop shadows to simulate pressing a physical stamp into paper.
- **Textural Hatching:** Diagonal striping patterns (`pencil-hatch`, 45-degree repeating linear gradients) substitute for tint overlays to communicate active states, table header stripes, or highlighted blocks.

## Shapes
Shapes emphasize irregular, manual fabrication:

- **Asymmetric / Organic Radii:** Hand-cut corners with subtle variations (e.g., `4px 6px 3px 7px / 6px 3px 5px 4px`) providing an imperfect, scribbled contour.
- **Borders:** Solid 1.5px to 2px outlines in `#232018`, paired with dashed borders (`border-dashed border-outline`) for sub-dividers, tear-off margins, and tape strip boundaries.
- **Pills & Badges:** Small functional tags rely on sharp-to-subtle 4px curves (`rounded`) or fully rounded circular punch markers for repetition tallies.

## Components

### Buttons & Quick Actions
- **Primary Buttons:** High-contrast solid dark fill (`#1f1d18`), light ink text (`#fefbe8`), wrapped in a 1.5px hard border with a `2px 2px 0 #6b624c` hard shadow.
- **Hatched / Doodle Buttons:** Light tinted surface with diagonal graphite hatching (`pencil-hatch`), dark outline, and uppercase monospace text for secondary quick-actions.

### Input Fields & Search Bars
- Inset light paper surface (`#fbf5de`), bordered by a sketch line and subtle hard shadow. Monospaced placeholder text in muted graphite tones with zero border focus rings.

### Data Tables & Log Rows
- Dense ruled grid with hairline separators (`divide-y divide-outline-variant/60`), bordered headers with alternating diagonal hatch fill, and clear right-aligned verification checkmark boxes.
- Row zebra highlighting uses subtle buff variations (`#ebdca3/35`) rather than artificial gray tones.

### Floating Floating Timer & Bars
- Docked sheets pinned above the bottom bar with hard top and bottom offset shadows (`shadow-[0px_-2px_0px_0px_#232018]`), displaying large monospaced countdowns and quick incremental bump chips (`+30s`).

### Scraps & Marginalia Cards
- Taped or pinned note blocks with dashed borders, light contrast fills (`#f3e7c3/60`), and italicized note content simulating personal observations.