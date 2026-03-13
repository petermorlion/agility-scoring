# API Tests

This directory contains integration tests for the API endpoints.

## Running Tests

### Prerequisites

1. **MongoDB must be running:**
   ```bash
   # Start MongoDB via docker-compose (from project root)
   cd ../..
   docker-compose -f apps/api/docker-compose.yml up -d
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

### Execute Tests

```bash
npm test
```

## Test Framework

- **Framework:** Node.js built-in test runner (`node:test`)
- **TypeScript:** Tests use `.ts` files compiled on-the-fly with `tsx`
- **Test pattern:** `src/__tests__/**/*.test.ts`

## Database

Tests use a separate test database:
- **Database name:** `agility_scoring_test`
- **Isolation:** Tests clean up before/after each suite
- **URI:** Uses `MONGODB_URI` env var or defaults to `mongodb://localhost:27017`

## Writing Tests

### Example Test Structure

```typescript
import { describe, it, before, after } from 'node:test';
import assert from 'node:assert';
import { appRouter } from '../router';

describe('my endpoint', () => {
  before(async () => {
    // Setup (connect to DB, seed data)
  });

  after(async () => {
    // Cleanup (delete test data, close connections)
  });

  it('should do something', async () => {
    // Arrange
    const caller = appRouter.createCaller({});
    
    // Act
    const result = await caller.myEndpoint({ input: 'data' });
    
    // Assert
    assert.strictEqual(result.success, true);
  });
});
```

### Calling tRPC Procedures

```typescript
const caller = appRouter.createCaller({});
const result = await caller.procedureName({ ...input });
```

## Current Test Coverage

- ✅ `setDisqualified` (7 test cases) — pending implementation
- ⚠️ Other endpoints not yet tested

## CI/CD

When adding CI/CD pipeline, tests should:
1. Start MongoDB in a container
2. Wait for MongoDB to be ready
3. Run `npm test`
4. Stop MongoDB container

Example GitHub Actions:
```yaml
services:
  mongodb:
    image: mongo:5
    ports:
      - 27017:27017
```
