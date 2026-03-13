# Test Cases for Disqualified Button (Issue #18)

**Date:** 2026-03-13  
**Author:** Simon (Tester)  
**Status:** ✅ Tests written — matches Zoe's implementation  
**Branch:** squad/18-disqualified-button

## Summary

Created comprehensive test suite for the `setDisqualified` tRPC mutation in `apps/api/src/endpoints/results.ts`. Tests are written using Node.js built-in test runner (Node v24+).

## IMPORTANT: Design Note

The endpoint was implemented using **result `id`** (the MongoDB `_id` field) rather than **`contestantNumber`** as initially described in the issue context. This is Zoe's design decision documented in `.squad/decisions/inbox/zoe-disqualified.md`.

**Current API signature:**
```typescript
{ id: string, tournamentId: string, disqualified: boolean }
```

**Original requirements mentioned:**
```typescript
{ tournamentId: string, contestantNumber: number, disqualified: boolean }
```

The MAUI app currently stores disqualified status in **local storage only** and doesn't call the API. Kaylee will need to integrate the API call and map contestantNumber → result id.

## Test Infrastructure Setup

### What Was Created

1. **Test directory:** `apps/api/src/__tests__/`
2. **Test file:** `apps/api/src/__tests__/setDisqualified.test.ts`
3. **Test script:** Added `"test"` script to `apps/api/package.json`:
   ```json
   "test": "node --test --require tsx/cjs src/__tests__/**/*.test.ts"
   ```

### Running Tests

```bash
cd apps/api
npm test
```

The tests require MongoDB to be running:
```bash
docker-compose -f apps/api/docker-compose.yml up -d
```

## Test Coverage

### 1. Happy Path: Set Disqualified to True
**What:** Marks a result as disqualified  
**Input:** `{ id: "result-uuid", tournamentId: "...", disqualified: true }`  
**Expected:** 
- Returns `{ success: true, result: { ...document, disqualified: true } }`
- Database field `disqualified` is set to `true`

### 2. Happy Path: Un-disqualify (Set to False)
**What:** Removes disqualified status (undo accidental tap)  
**Input:** `{ id: "result-uuid", tournamentId: "...", disqualified: false }`  
**Expected:**
- Returns `{ success: true, result: { ...document, disqualified: false } }`
- Database field `disqualified` is set to `false`

### 3. Error Case: Tournament ID Mismatch
**What:** Result exists but belongs to a different tournament  
**Input:** `{ id: "result-uuid", tournamentId: "wrong-tournament-id", disqualified: true }`  
**Expected:**
- Returns `{ success: false, error: "Result not found" }`
- No database changes

### 4. Error Case: Result ID Doesn't Exist
**What:** No result found for the given id  
**Input:** `{ id: "non-existent-uuid", tournamentId: "...", disqualified: true }`  
**Expected:**
- Returns `{ success: false, error: "Result not found" }`
- No database changes

### 5. Validation: Missing Required Fields
**What:** Zod schema validation should catch missing fields  
**Tests:**
- Missing `id` → ZodError / BAD_REQUEST
- Missing `tournamentId` → ZodError / BAD_REQUEST
- Missing `disqualified` → ZodError / BAD_REQUEST
- Invalid type for `disqualified` (string instead of boolean) → ZodError
- Invalid type for `id` (number instead of string) → ZodError

### 6. Edge Case: Multiple Toggles
**What:** Repeatedly toggling disqualified status  
**Sequence:** `false → true → false → true`  
**Expected:** Each toggle succeeds; final state matches last call

### 7. Data Integrity: Preserve Other Fields
**What:** Setting disqualified shouldn't affect other result fields  
**Expected:** 
- `name`, `obstacles`, `tournamentId`, etc. remain unchanged
- Only `disqualified` field is modified

## Implementation Notes

### Endpoint Signature (Implemented by Zoe)

Located in `apps/api/src/endpoints/results.ts`:

```typescript
setDisqualified: publicProcedure
  .input(
    z.object({
      id: z.string(),
      tournamentId: z.string(),
      disqualified: z.boolean(),
    })
  )
  .mutation(async ({ input }) => {
    const coll = getDb().collection('results');
    const existing = await coll.findOne({ _id: input.id });
    if (!existing || existing.tournamentId !== input.tournamentId) {
      return { success: false, error: 'Result not found' };
    }
    await coll.updateOne(
      { _id: input.id },
      { $set: { disqualified: input.disqualified } }
    );
    const updated = await coll.findOne({ _id: input.id });
    return { success: true, result: updated };
  })
```

### Database Schema

The `results` collection now has:
- `disqualified: boolean` field
- Defaults to `false` when created via `upsertResult`

### MAUI Client Integration (For Kaylee)

The MAUI app will need to:
1. Get the result `id` for the current contestantNumber
2. Call the API via HTTP POST:
   ```
   POST /trpc/setDisqualified
   Body: { "input": { "id": "result-id", "tournamentId": "...", "disqualified": true } }
   ```
3. Parse the tRPC envelope:
   ```json
   {
     "result": {
       "data": {
         "success": true,
         "result": { "_id": "...", "disqualified": true, ... }
       }
     }
   }
   ```

**Challenge:** The MAUI app tracks contestants by `contestantNumber`, but the API uses result `id`. The MAUI app will need to maintain a mapping or query results to find the correct id.

## Known Limitations

1. **Result ID vs Contestant Number mismatch:** API uses result `id`, MAUI uses `contestantNumber`. Integration will require mapping.

2. **No MAUI tests yet:** MAUI test project doesn't exist. ViewModel and UI testing would require:
   - `xUnit` or `NUnit` test project
   - Mock `ApiService`
   - Test `TournamentDetailPageViewModel` toggle logic

3. **Test database isolation:** Tests use a separate `agility_scoring_test` database. Requires MongoDB to be running locally for tests to pass.

4. **Auth not tested:** Since all procedures use `publicProcedure`, auth is not validated. When auth is added, these tests will need auth setup.

## Test Results

### Before MongoDB Started
```
✖ failing tests:
✖ setDisqualified tRPC mutation (30023.258ms)
  Error [MongoServerSelectionError]: connect ECONNREFUSED
```

### After MongoDB Started (Expected)
```
✔ should set disqualified to true for a contestant
✔ should set disqualified to false (un-disqualify a contestant)
✔ should return error for invalid tournamentId (not found)
✔ should return error for result ID that does not exist
✔ should validate required fields with Zod
✔ should handle toggling disqualified status multiple times
✔ should preserve other result fields when setting disqualified
✔ setDisqualified tRPC mutation
ℹ tests 7
ℹ pass 7
ℹ fail 0
```

## Next Steps

1. ~~Zoe (Backend Dev): Implement the endpoint~~ ✅ DONE
2. **Simon (me): Run tests with MongoDB** — verify they pass
3. **Kaylee (MAUI Dev):** 
   - Integrate API call (replacing local storage)
   - Map contestantNumber → result id
   - Wire up toggle button to call API
4. **Peter:** Decide if contestantNumber approach should be added/preferred

## Test Execution Requirements

Before running tests:
1. MongoDB must be running:
   ```bash
   docker-compose -f apps/api/docker-compose.yml up -d
   ```
2. Node.js v24+ (project currently uses v24.12.0)
3. All dependencies installed (`npm install` in `apps/api`)

