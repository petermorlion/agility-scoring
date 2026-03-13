# Simon — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** Turborepo monorepo — Node.js/Express/tRPC/MongoDB (`apps/api`) + .NET MAUI C# (`apps/maui`)
- **Description:** Mobile app for creating dog agility tournaments and counting obstacle faults/refusals per run
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

### 2026-03-13: Test Infrastructure Setup for Disqualified Button (Issue #18)
**What:** Created comprehensive test suite for `setDisqualified` tRPC mutation using Node.js built-in test runner.

**Key learnings:**
- Project uses Node v24.12.0, which has native test runner (`node:test` module) — no Jest/Mocha needed
- No existing test infrastructure was present in `apps/api/` — first test file created
- Tests written TDD-style before implementation to guide backend development
- MongoDB test database pattern: connect to separate `agility_scoring_test` db to avoid polluting production data
- tRPC router testing via `appRouter.createCaller({})` for direct procedure invocation

**Test coverage created:**
1. Set disqualified to true (happy path)
2. Set disqualified to false / un-disqualify (toggle undo)
3. Invalid tournamentId (error case)
4. Non-existent contestant number (error case)
5. Zod validation for all required fields + type checking
6. Multiple toggle operations (edge case)
7. Preserve other result fields during update (data integrity)

**Edge cases identified but NOT tested:**
- Negative/zero contestant numbers (needs Zod `.positive()` validation)
- Concurrent updates (race conditions)
- Backward compatibility with results missing `disqualified` field
- MAUI UI debouncing and error handling (requires UI tests)

**Files created:**
- `apps/api/src/__tests__/setDisqualified.test.ts` (7 test cases)
- `.squad/decisions/inbox/simon-disqualified.md` (implementation notes for Zoe)
- `.squad/decisions/inbox/simon-disqualified-test-cases.md` (detailed edge case analysis)

**package.json changes:**
- Added `"test": "node --test --require tsx/cjs src/__tests__/**/*.test.ts"` script

**Next steps:**
- Zoe implements `setDisqualified` endpoint in `results.ts`
- Tests should pass after implementation
- Kaylee wires up MAUI toggle button to call endpoint
- Simon reviews implementation and runs tests
