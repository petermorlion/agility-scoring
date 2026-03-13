# Disqualified Button — Edge Cases & Test Plan

**Issue:** #18  
**Feature:** Toggle button to mark contestant as disqualified  
**Author:** Simon (Tester)  
**Date:** 2026-03-13

## Test Cases Summary

Total test cases: **7**  
Framework: Node.js built-in test runner (v24+)  
Status: ✅ Written (awaiting implementation)

## Edge Cases Identified

### 1. Race Condition: Concurrent Disqualification Toggles
**Scenario:** Two judges/devices toggle the same contestant's DQ status at the same time  
**Risk:** Last write wins, but intermediate state may be lost  
**Mitigation:** Not tested (would require concurrent request simulation)  
**Priority:** LOW (unlikely in single-judge workflow)

### 2. Orphaned Result Records
**Scenario:** Contestant result exists but tournament was deleted  
**Behavior:** `setDisqualified` with a valid contestantNumber but invalid tournamentId  
**Test coverage:** ✅ Covered in test case #3 (invalid tournamentId)  
**Expected:** Should fail gracefully

### 3. Partial Data Migration
**Scenario:** Existing results don't have the `disqualified` field (created before feature)  
**Behavior:** Querying old records should default to `disqualified: false`  
**Test coverage:** ⚠️ Not explicitly tested (assumes MongoDB `$set` adds field to old docs)  
**Recommendation:** Add migration script or test backward compatibility

### 4. UI Double-tap/Rapid Toggle
**Scenario:** User rapidly taps the disqualified button multiple times  
**Behavior:** UI should debounce or disable button during API call  
**Test coverage:** ✅ Covered in test case #6 (multiple toggles)  
**Note:** MAUI UI-level debouncing not tested (requires UI tests)

### 5. Boolean Coercion
**Scenario:** Caller sends "true" (string) instead of true (boolean)  
**Behavior:** Zod should reject with validation error  
**Test coverage:** ✅ Covered in test case #5 (validation)

### 6. Negative or Zero Contestant Numbers
**Scenario:** `contestantNumber: 0` or `contestantNumber: -1`  
**Behavior:** Should either be rejected by validation or treated as valid (if 0 is allowed)  
**Test coverage:** ❌ NOT COVERED  
**Recommendation:** Add Zod `.positive()` if contestant numbers start at 1

### 7. Very Large Contestant Numbers
**Scenario:** `contestantNumber: 999999999`  
**Behavior:** Valid but won't match any result  
**Test coverage:** ✅ Covered in test case #4 (non-existent contestant)

### 8. Network Failures / API Timeout
**Scenario:** MAUI app sends request but API is unreachable  
**Behavior:** MAUI app should show error and leave toggle in previous state  
**Test coverage:** ❌ NOT COVERED (requires integration/E2E test)  
**Recommendation:** MAUI `ApiService` should handle exceptions

### 9. Result Without Contestant Number
**Scenario:** Legacy result document doesn't have `contestantNumber` field  
**Behavior:** Query won't match; DQ toggle fails  
**Test coverage:** ⚠️ Not tested  
**Recommendation:** Ensure `upsertResult` always sets `contestantNumber`

### 10. Database Connection Loss During Update
**Scenario:** MongoDB connection drops mid-update  
**Behavior:** Update may fail; transaction not committed  
**Test coverage:** ❌ NOT COVERED (requires DB fault injection)  
**Priority:** LOW (MongoDB driver handles reconnection)

## Missing Test Coverage

### Backend (API)
- ❌ Negative/zero contestant numbers validation
- ❌ Backward compatibility with results missing `disqualified` field
- ❌ Concurrent updates (race conditions)
- ❌ Database connection failures

### Frontend (MAUI)
- ❌ UI tests for TournamentDetailPage toggle button
- ❌ ViewModel tests for toggle logic
- ❌ Network error handling in `ApiService`
- ❌ Debouncing/disabling button during API call
- ❌ Optimistic UI updates (toggle before API confirms)

## Recommendations for Implementation

### Backend (Zoe)
1. **Add contestant number validation:**
   ```typescript
   contestantNumber: z.number().int().positive()
   ```
2. **Ensure `upsertResult` includes `disqualified: false` by default:**
   ```typescript
   const doc = { 
     _id: id, 
     tournamentId: input.tournamentId, 
     name: input.name, 
     obstacles: input.obstacles,
     disqualified: false, // <-- Add this
   };
   ```
3. **Return consistent error structure:**
   ```typescript
   if (!result) {
     return { success: false, error: 'Contestant not found in this tournament' };
   }
   ```

### Frontend (Kaylee)
1. **Disable button during API call:**
   ```csharp
   public async Task ToggleDisqualifiedAsync()
   {
       if (IsBusy) return;
       IsBusy = true;
       try {
           await _apiService.SetDisqualifiedAsync(TournamentId, ContestantNumber, !Disqualified);
           Disqualified = !Disqualified; // Update local state
       } catch (Exception ex) {
           // Show error, leave Disqualified unchanged
           await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
       } finally {
           IsBusy = false;
       }
   }
   ```
2. **Add loading indicator to button** (spinner or disable state)

## Test Execution Plan

### Phase 1: Backend Tests (Current)
```bash
cd apps/api
npm test
```
**Expected:** All tests fail (endpoint not implemented)

### Phase 2: After Backend Implementation
```bash
cd apps/api
npm test
```
**Expected:** All 7 tests pass

### Phase 3: MAUI Manual Testing
1. Run MAUI app on Android device
2. Navigate to TournamentDetailPage
3. Tap "Disqualified" toggle
4. Verify UI reflects change
5. Navigate away and back → verify state persists
6. Toggle off → verify un-disqualification works
7. Airplane mode → toggle → verify error handling

### Phase 4: MAUI Unit Tests (Future)
- Create `AgilityScoring.Tests.csproj` with xUnit
- Mock `ApiService`
- Test `TournamentDetailPageViewModel.ToggleDisqualifiedAsync()`

## Acceptance Criteria

- [ ] Backend: All 7 API tests pass
- [ ] Backend: Endpoint added to `resultsRouter` in `results.ts`
- [ ] Backend: Zod validation prevents invalid input
- [ ] MAUI: Toggle button exists on TournamentDetailPage
- [ ] MAUI: Button is disabled during API call
- [ ] MAUI: Error alert shown on API failure
- [ ] MAUI: State persists after navigation
- [ ] Manual test: Rapid toggling doesn't cause errors

## Questions for Team

1. **Should we enforce tournament existence?** Currently, the test expects failure if tournamentId doesn't match a tournament, but the endpoint could work without checking tournaments collection.

2. **Contestant number vs. result ID?** The MAUI app uses `contestantNumber` to identify results, but results also have unique `_id`. Should we use `resultId` instead for direct lookups?

3. **Undo history?** Should we track when/who disqualified a contestant (audit log)? Not in current scope but could be future enhancement.

4. **Visual indication?** How should disqualified contestants appear in the UI? Red banner? Strike-through? This affects MAUI UI but not API.
