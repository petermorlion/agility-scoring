# Zoe — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** Node.js/Express/tRPC/MongoDB in `apps/api`
- **Description:** Mobile app for creating dog agility tournaments and counting obstacle faults/refusals per run
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

### 2026-06-09: Disqualified Feature Implementation (#18)
**Task:** Implemented API support for marking contestants as disqualified (issue #18).

**What I did:**
- Added `disqualified` boolean field to result documents (default `false`)
- Updated `upsertResult` mutation to initialize `disqualified` as `false` for new results
- Updated `getResult` query to ensure backward compatibility (returns `false` if field doesn't exist on old documents)
- Created new `setDisqualified` mutation with Zod validation:
  - Input: `{ id: string, tournamentId: string, disqualified: boolean }`
  - Validates the result exists and belongs to the tournament before updating
  - Uses MongoDB `$set` operator for atomic updates
  - Returns `{ success: boolean, result?: Document, error?: string }`

**Technical notes:**
- Followed existing patterns in `results.ts` for consistency
- The mutation accepts explicit boolean state (not a toggle) — client sends desired state
- Pre-existing TypeScript errors exist in codebase (MongoDB expects ObjectId but we use string UUIDs) — these are unrelated to this feature
- All procedures use `publicProcedure` (no auth middleware yet)

**Outcome:** 
- PR #19 created: https://github.com/petermorlion/agility-scoring/pull/19
- Ready for Kaylee to implement MAUI client side
- Simon wrote 7 test cases; ready to execute with MongoDB

**Cross-team notes:**
- Kaylee implemented local-only MAUI UI (no API integration yet)
- Simon identified design discrepancy: API uses result `id` but MAUI tracks by `contestantNumber` — requires mapping logic
- Future: MAUI API integration will need to map contestant numbers to result IDs
