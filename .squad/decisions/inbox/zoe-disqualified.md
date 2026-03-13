# Decision: Disqualified Field Implementation

**Date:** 2026-06-09  
**By:** Zoe (Backend Dev)  
**Issue:** #18  
**PR:** #19

## Decision
Implemented `disqualified` as a boolean field on result documents with an explicit state mutation (not a toggle).

## Context
Issue #18 requires a button in the contestant page to mark contestants as disqualified, with the ability to undo accidental taps. The API needs to:
1. Store the disqualified state persistently
2. Return it with result queries
3. Allow clients to update it

## What Was Implemented
1. **Data model change:**
   - Added `disqualified: boolean` field to result documents
   - Defaults to `false` for new results (in `upsertResult`)
   - Backward compatible: `getResult` returns `false` if field doesn't exist on old documents

2. **New mutation: `setDisqualified`**
   - Input schema: `{ id: string, tournamentId: string, disqualified: boolean }`
   - Validates that result exists and belongs to the specified tournament
   - Uses MongoDB `$set` for atomic update
   - Returns `{ success: boolean, result?: Document, error?: string }`

## Why Explicit State (Not Toggle)
Chose to pass explicit `disqualified: boolean` value rather than implementing a toggle endpoint because:
- **Idempotent:** Client can retry safely without side effects
- **Clear intent:** The desired state is explicit in every call
- **Race-safe:** Multiple clients won't accidentally flip state
- **Simpler:** Client manages UI state, server just stores it

The MAUI client can implement toggle behavior by reading current state and inverting it before calling `setDisqualified`.

## Technical Notes
- Follows existing patterns in `apps/api/src/endpoints/results.ts`
- All validation via Zod
- Uses `publicProcedure` (no auth yet)
- MongoDB `$set` operator for partial document updates
- Pre-existing TypeScript errors (string IDs vs ObjectId) remain unresolved but don't affect runtime

## Next Steps
- Kaylee to implement MAUI UI and client-side logic
- Simon to write integration tests for the mutation
