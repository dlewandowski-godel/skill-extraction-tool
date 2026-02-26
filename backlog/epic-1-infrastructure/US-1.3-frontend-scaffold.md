# US-1.3 — React Frontend Scaffold

**Epic:** Epic 1 — Project Setup & Infrastructure  
**Status:** `[x] Done`

---

## User Story

> As a developer, I want the React + Vite + TypeScript frontend scaffolded with MUI v6, TanStack Query v5, and React Router v6 installed and configured so the team has a working base to build features on.

---

## Acceptance Criteria

- [ ] Frontend lives at `frontend/` in the repo root
- [ ] Created with Vite + React + TypeScript template (`npm create vite@latest`)
- [ ] MUI v6 (`@mui/material`, `@mui/icons-material`, `@emotion/react`, `@emotion/styled`) installed
- [ ] TanStack Query v5 (`@tanstack/react-query`) installed and `QueryClientProvider` wraps the app
- [ ] React Router v6 (`react-router-dom`) installed with a `<BrowserRouter>` at app root
- [ ] Absolute imports configured (`@/` alias maps to `src/`) in `vite.config.ts` and `tsconfig.json`
- [ ] A typed Axios instance (`src/lib/api-client.ts`) is configured with base URL from env and JWT interceptor (attaches `Authorization: Bearer <token>` header)
- [ ] Global MUI theme file (`src/theme.ts`) created with primary/secondary palette
- [ ] TanStack Query DevTools installed and rendered only in development mode
- [ ] `npm run dev` starts the dev server without errors
- [ ] `npm run build` produces a production bundle without errors

---

## Technical Notes

- TanStack Query v5: all hooks must use the single-object signature `useQuery({ queryKey, queryFn })`
- MUI v6: use `Grid2` (not deprecated `Grid`), and `size` prop instead of `xs/sm/md`
- JWT stored in memory (React context / Zustand), NOT in localStorage
- Axios interceptor should attach the in-memory token from auth context

---

## Unit Tests

- [ ] `vitest`, `@testing-library/react`, `@testing-library/user-event`, and `@testing-library/jest-dom` are installed as dev dependencies
- [ ] `vitest.config.ts` is configured with `jsdom` environment and `@testing-library/jest-dom` setup file
- [ ] A smoke test `App.test.tsx` confirms the app renders without crashing
- [ ] `npm test` runs with zero failures on the empty test suite

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-1.2](US-1.2-dotnet-scaffold.md) | **Next:** [US-1.4](US-1.4-efcore-postgres.md)
