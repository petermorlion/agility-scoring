# Zoe — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** Node.js/Express/tRPC/MongoDB in `apps/api`
- **Description:** Mobile app for creating dog agility tournaments and counting obstacle faults/refusals per run
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

### 2026-06-09: Disqualified Feature Implementation (#18) — API Revert
**Task:** Reverted API support for marking contestants as disqualified per Peter's decision to pause API work.

**What I did:**
- Reverted the `setDisqualified` mutation from `apps/api/src/endpoints/results.ts` (commit 7301f3a)
- Removed test file `apps/api/src/__tests__/setDisqualified.test.ts` (newly created test suite)
- Removed `npm test` script reference from `apps/api/package.json`
- Updated PR #19 comment explaining API work is paused; branch focus shifted to MAUI local storage only

**Decision context:** 
Peter's decision: "We're currently not continuing work on the API. This feature should be implemented in the MAUI app."

**Impact:**
- No new tRPC endpoints or MongoDB changes until further notice
- Kaylee's MAUI disqualified toggle (local storage) is complete and ready
- Simon's test suite for setDisqualified mutation on hold
- Future API integration may be revisited if project priorities change

**Cross-team notes:**
- Kaylee implemented local-only MAUI UI (no API integration needed for now)
- API would have required mapping logic: MAUI tracks by `contestantNumber` but API uses result `id` (UUID)

