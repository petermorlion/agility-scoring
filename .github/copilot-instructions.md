# Agility Scoring — Copilot Instructions

## Domain Model

This app tracks **dog agility competitions**. The core concepts:

- **Tournament** — a competition event (`name`, `date`)
- **Result** — one dog's run in a tournament. Stores fault counts per obstacle.
  - `tournamentId` — links back to a tournament
  - `name` — dog/handler name
  - `obstacles` — array of `{ obstacleKey, value }` where `value` is the fault/refusal count

**Obstacle keys** (the canonical enum used in both API and MAUI):
```
aframe | dogwalk | seesaw | tunnel | chute | jump | tire
```

Any new obstacle-related feature must use these exact keys. They are validated server-side via Zod enum and stored verbatim in MongoDB.

## Architecture

This is a Turborepo monorepo with two apps:

- **`apps/api`** — Node.js/Express API server with tRPC endpoints, backed by MongoDB
- **`apps/maui`** — .NET MAUI mobile app (C#) targeting Android, converted from original Expo/React Native

The MAUI app communicates with the API via plain HTTP, manually unwrapping the tRPC response envelope (`result.data`). There is no tRPC client library on the MAUI side.

## Commands

### API (Node.js)
```bash
cd apps/api
npm run dev      # starts MongoDB via Docker Compose, then tsx watch
npm run build    # tsc compile to dist/
npm run start    # run compiled output
```

### MAUI (.NET)
```bash
cd apps/maui
dotnet build -t:Run -f net10.0-android   # build and run on Android
```

### Monorepo (from root)
```bash
npm install      # install all dependencies
npm run dev      # run all apps in dev mode via Turborepo
npm run build    # build all apps
npm run lint     # lint all apps
```

## API Conventions

**Adding a new tRPC endpoint:**
1. Create a router file in `apps/api/src/endpoints/` exporting a `router({...})` 
2. Spread its procedures into `apps/api/src/router.ts` using `...myRouter._def.procedures`
3. All procedures use `publicProcedure` from `src/trpc.ts` — there is no auth middleware yet
4. Validate inputs with Zod

**tRPC HTTP calling convention (used by the MAUI client):**
- Queries → `GET /trpc/{procedureName}?input={json}` or `POST /trpc/{procedureName}` with `{ input: {...} }` body
- Mutations → `POST /trpc/{procedureName}` with `{ input: {...} }` body
- Response envelope: `{ result: { data: <payload> } }`
- Health check (no tRPC): `GET /health`

**MongoDB:**
- Uses the native `mongodb` driver (not Mongoose)
- Document IDs are `string` UUIDs (via `randomUUID()`), not `ObjectId`
- Access the DB via `getDb()` from `src/db.ts` — never call `connectToDb()` from endpoint files
- Collections: `tournaments`, `results`

**Environment variables:**
- `MONGODB_URI` — default: `mongodb://localhost:27017`
- `MONGODB_DB` — default: `agility_scoring`
- `PORT` — default: `3000`

## MAUI Conventions

**MVVM pattern:**
- All ViewModels extend `BaseViewModel` which implements `INotifyPropertyChanged` via `SetProperty<T>`
- Use `IsBusy`/`IsNotBusy` from `BaseViewModel` to gate loading states
- All services and ViewModels are registered as singletons in `MauiProgram.cs`

**Services:**
- `ConfigService` — detects dev vs. prod via `Debugger.IsAttached`; dev API URL is `http://localhost:3000`
- `ApiService` — wraps HTTP calls to tRPC; parses the `{ result: { data: ... } }` envelope manually using `System.Text.Json`
- `AuthService` — Google OAuth via MSAL (`Microsoft.Identity.Client`)

**tRPC response parsing in MAUI:**
tRPC HTTP responses are wrapped as `{ result: { data: <payload> } }`. `ApiService` always checks for `result` → `data` before deserializing. DTOs use `[JsonPropertyName]` attributes where the JSON property name differs from the C# convention.

**Adding a new page:**
1. Create `Views/MyPage.xaml` + `Views/MyPage.xaml.cs` and `ViewModels/MyPageViewModel.cs`
2. Register all three as singletons in `MauiProgram.cs`
3. Add any new `ApiService` methods for the feature alongside matching DTO classes in `ApiService.cs`

**Auth:**
- `AuthService` uses MSAL (`Microsoft.Identity.Client`) with Google OAuth. Call `LoginAsync()` to trigger interactive login; `CheckAuthAsync()` to restore session from `SecureStorage`.
- The `CurrentUser` property holds the signed-in user (`Id`, `Name`, `Email`).
- Auth is not yet enforced on the API — `publicProcedure` is used everywhere.

**Project targets:** `net10.0-android` only; iOS support is planned but not yet wired up in the `.csproj`.
