# Routing

> Who handles what. The coordinator checks this before spawning agents.

## Rules

| Signal | Route to |
|--------|----------|
| Architecture decisions, scope questions, cross-cutting concerns | Mal (Lead) |
| API endpoints, tRPC procedures, MongoDB queries, Node.js | Zoe (Backend Dev) |
| .NET MAUI, XAML, ViewModels, C# services, mobile UI | Kaylee (MAUI Dev) |
| Tests, quality, edge cases, validation, review | Simon (Tester) |
| "Team" or tasks touching both API and MAUI | Mal + Zoe + Kaylee in parallel |
| Logging, decisions, memory | Scribe (background, silent) |
| Work queue, issue triage, PR monitoring | Ralph |

## Domain Routing

| Domain | Agent |
|--------|-------|
| tRPC endpoint creation | Zoe |
| MongoDB schema / collections | Zoe |
| Obstacle fault counting logic | Zoe (API) + Kaylee (MAUI) |
| MAUI page creation (XAML + ViewModel) | Kaylee |
| ApiService HTTP calls | Kaylee |
| AuthService / Google OAuth | Kaylee |
| ConfigService (dev/prod environments) | Kaylee |
| Zod input validation | Zoe |
| Test coverage for API | Simon |
| Test coverage for MAUI ViewModels | Simon |
| Code review / PR review | Simon or Mal |
