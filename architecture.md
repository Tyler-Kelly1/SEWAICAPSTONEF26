# System Architecture & Coding Standards Guidelines

> [!IMPORTANT]
> All AI agents and developers **MUST** adhere to the architecture rules defined in this file when writing or refactoring code for this repository.

---

## 🏛️ Human-Led Architecture Overview

In compliance with project rules (see [`rules.md`](file:///C:/Users/tyler/Overload/rules.md)), all system architecture decisions are human-designed. This document serves as the authoritative blueprint for code structure and conventions.

---

## ⚙️ Backend (BE) Architecture & Practices

### 1. Three-Layered Architecture & Feature Slicing
Backend development is structured using a **three-layered architecture**, organized in **slices** where each slice corresponds to a specific page or view in the Frontend (FE).

```
   [ Controller Layer ] (Thin Controllers)
            │  (DI: Injects Engine)
            ▼
     [ Logic Layer ]    ((Page)Engine)
            │  (DI: Injects DbContext)
            ▼
  [ Data Access Layer ] (EF Core DbContext)
```

### 2. Layer Specifications & Rules

- **Layer 1: Data Access Layer (EF Core)**
  - Responsible for database interactions using standard `DbContext` adhering to EF Core guidelines.
  - Interacts directly with database models and queries.

- **Layer 2: Logic Layer (`(Page)Engine`)**
  - All business logic MUST reside in the logic layer.
  - Class Naming Convention: Each logic class MUST be named **`(Page)Engine`** matching its corresponding FE page/view slice (e.g., `TestTableEngine`, `DashboardEngine`).
  - `DbContext` MUST be injected into the corresponding Engine via Dependency Injection (DI).

- **Layer 3: Controller Layer**
  - Controllers MUST be **thin** and contain minimal to no business logic. Their sole responsibility is handling HTTP requests/responses and delegating actions to the Engine layer.
  - The corresponding `(Page)Engine` MUST be injected into the Controller via Dependency Injection (DI).

### 3. Request DTO & Parameter Handling Guidelines

- **GET Operations (`[HttpGet]`):**
  - Endpoints with **3 or fewer arguments** (e.g., query params, route parameters) may pass parameters individually in the action method signature.
  - Endpoints with **more than 3 arguments (> 3 args)** MUST use a dedicated Request DTO (e.g., bound with `[FromQuery]`).

- **Non-GET Operations (`POST`, `PUT`, `DELETE`, etc.):**
  - ALL non-GET endpoints MUST use a dedicated Request DTO for request payloads.
  - Request DTOs MUST enforce validation using attribute validation annotations (e.g., `[Required]`, `[StringLength]`, `[Range]`, etc.).

---

## 🎨 Frontend (FE) Architecture & Practices

### 1. HTTP Client Standard
- **Always use native JavaScript `fetch` instead of AXIOS.**
- Do NOT import or use `axios` for HTTP requests in the frontend codebase.

### 2. Vue Single File Component (SFC) Structure
All Vue components (`.vue` files) MUST strictly follow this top-to-bottom tag ordering:

1. `<script>`
2. `<template>`
3. `<style>` (CSS)

#### Example Vue Component Order:
```vue
<script setup>
// Script logic & imports
</script>

<template>
  <!-- HTML template -->
</template>

<style scoped>
/* CSS styling */
</style>
```

### 3. API Call Flow & Service Layer Architecture

#### Data Flow Directive:
All frontend API requests MUST follow this strict flow:
```
Service Layer ((PageName).api.js) ➔ Parent Component ➔ Child Component
```

#### Service Layer Rules:
- **File Structure & Naming:** Each page/view MUST have its own API service file inside `src/services/` named **`(PageName).api.js`** (e.g., `TestTable.api.js`).
- **Single Object Export:** Each `(PageName).api.js` file MUST export **one single object** containing all required API functions for that page/view.
- **Error Propagation:** Service layer functions MUST always return API data or throw errors.
- **Component Error Handling:** Errors MUST be caught and handled inside the **component layer**, NOT in the service layer. Service files must not swallow errors or perform UI alert handling.

#### Component & Flow Rules:
- **No API Calls in Children:** Child components MUST NEVER make API calls directly. Data and actions should be passed via props/events from the parent component.
- **Flow Length & Multi-Generation Fork Rule (>3 Generations):**
  - If a component data flow extends longer than 3 generations (e.g., Parent ➔ Child ➔ Grandchild ➔ Great-Grandchild):
    - Do NOT prop-drill through more than 3 component levels.
    - AI agents MUST opt for either a **Pinia store** or a **composable**.
    - **FORK DECISION RULE:** Upon encountering a flow longer than 3 generations, the AI agent **MUST ask the human user** for their choice (Pinia store vs. Composable). The user's selection and justification MUST be recorded in this `architecture.md` file before implementation.

---

## 📝 Architecture Decisions Log

*(Log any human user architectural choices, such as Pinia vs. Composable decisions, below).*

| Date | Feature / Flow | User Choice | Justification |
| :--- | :--- | :--- | :--- |
| *N/A* | *No multi-generation (>3 levels) component flows created yet* | - | - |
