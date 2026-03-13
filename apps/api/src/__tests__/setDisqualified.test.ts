import { describe, it, before, after } from 'node:test';
import assert from 'node:assert';
import { MongoClient, Db } from 'mongodb';
import { appRouter } from '../router';
import { randomUUID } from 'crypto';

/**
 * Test suite for the setDisqualified tRPC mutation
 * 
 * Endpoint: setDisqualified
 * Input: { id: string, tournamentId: string, disqualified: boolean }
 * 
 * NOTE: The endpoint uses result `id` (document _id), not contestantNumber.
 * This differs from initial requirements which specified contestantNumber.
 * See .squad/decisions/inbox/simon-disqualified.md for details.
 * 
 * Expected behavior:
 *   - Updates the result document for the given id
 *   - Validates that result belongs to the specified tournament
 *   - Sets the disqualified field to true or false
 *   - Returns success: true and the updated result
 *   - Handles error cases: invalid id, tournamentId mismatch, validation errors
 */

describe('setDisqualified tRPC mutation', () => {
  let client: MongoClient;
  let db: Db;
  const TEST_DB = 'agility_scoring_test';
  const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://localhost:27017';

  before(async () => {
    // Connect to test database
    client = new MongoClient(MONGODB_URI);
    await client.connect();
    db = client.db(TEST_DB);
    
    // Clean up test collections
    await db.collection('tournaments').deleteMany({});
    await db.collection('results').deleteMany({});
  });

  after(async () => {
    // Clean up and close connection
    await db.collection('tournaments').deleteMany({});
    await db.collection('results').deleteMany({});
    await client.close();
  });

  it('should set disqualified to true for a contestant', async () => {
    // Arrange: Create a tournament and a result
    const tournamentId = randomUUID();
    const resultId = randomUUID();

    await db.collection('tournaments').insertOne({
      _id: tournamentId,
      name: 'Test Tournament',
      date: new Date().toISOString(),
    });

    await db.collection('results').insertOne({
      _id: resultId,
      tournamentId,
      name: 'Test Dog',
      obstacles: [],
      disqualified: false,
    });

    // Act: Call setDisqualified with disqualified: true
    const caller = appRouter.createCaller({});
    const result = await caller.setDisqualified({
      id: resultId,
      tournamentId,
      disqualified: true,
    });

    // Assert: Result should indicate success and disqualified should be true
    assert.strictEqual(result.success, true);
    assert.strictEqual(result.result.disqualified, true);

    // Verify in database
    const storedResult = await db.collection('results').findOne({ _id: resultId });
    assert.strictEqual(storedResult?.disqualified, true);
  });

  it('should set disqualified to false (un-disqualify a contestant)', async () => {
    // Arrange: Create a tournament and a disqualified result
    const tournamentId = randomUUID();
    const resultId = randomUUID();

    await db.collection('tournaments').insertOne({
      _id: tournamentId,
      name: 'Test Tournament 2',
      date: new Date().toISOString(),
    });

    await db.collection('results').insertOne({
      _id: resultId,
      tournamentId,
      name: 'Test Dog 2',
      obstacles: [],
      disqualified: true,
    });

    // Act: Call setDisqualified with disqualified: false
    const caller = appRouter.createCaller({});
    const result = await caller.setDisqualified({
      id: resultId,
      tournamentId,
      disqualified: false,
    });

    // Assert: Result should indicate success and disqualified should be false
    assert.strictEqual(result.success, true);
    assert.strictEqual(result.result.disqualified, false);

    // Verify in database
    const storedResult = await db.collection('results').findOne({ _id: resultId });
    assert.strictEqual(storedResult?.disqualified, false);
  });

  it('should return error for invalid tournamentId (not found)', async () => {
    // Arrange: Create a result but pass a different tournamentId
    const correctTournamentId = randomUUID();
    const wrongTournamentId = randomUUID();
    const resultId = randomUUID();

    await db.collection('tournaments').insertOne({
      _id: correctTournamentId,
      name: 'Test Tournament',
      date: new Date().toISOString(),
    });

    await db.collection('results').insertOne({
      _id: resultId,
      tournamentId: correctTournamentId,
      name: 'Test Dog',
      obstacles: [],
      disqualified: false,
    });

    // Act: Try to update with wrong tournamentId
    const caller = appRouter.createCaller({});
    const result = await caller.setDisqualified({
      id: resultId,
      tournamentId: wrongTournamentId,
      disqualified: true,
    });
    
    // Assert: Should return success: false
    assert.strictEqual(result.success, false);
    assert.ok(result.error);
  });

  it('should return error for result ID that does not exist', async () => {
    // Arrange: Create a tournament but no result with the given ID
    const tournamentId = randomUUID();
    const nonExistentResultId = randomUUID();

    await db.collection('tournaments').insertOne({
      _id: tournamentId,
      name: 'Test Tournament 3',
      date: new Date().toISOString(),
    });

    // Act: Try to update non-existent result
    const caller = appRouter.createCaller({});
    const result = await caller.setDisqualified({
      id: nonExistentResultId,
      tournamentId,
      disqualified: true,
    });
    
    // Assert: Should return success: false
    assert.strictEqual(result.success, false);
    assert.ok(result.error);
  });

  it('should validate required fields with Zod', async () => {
    const caller = appRouter.createCaller({});

    // Test missing id
    try {
      await caller.setDisqualified({
        tournamentId: randomUUID(),
        disqualified: true,
      } as any);
      assert.fail('Should have thrown validation error for missing id');
    } catch (error: any) {
      assert.ok(error.message.includes('id') || error.name === 'ZodError' || error.code === 'BAD_REQUEST');
    }

    // Test missing tournamentId
    try {
      await caller.setDisqualified({
        id: randomUUID(),
        disqualified: true,
      } as any);
      assert.fail('Should have thrown validation error for missing tournamentId');
    } catch (error: any) {
      assert.ok(error.message.includes('tournamentId') || error.name === 'ZodError' || error.code === 'BAD_REQUEST');
    }

    // Test missing disqualified field
    try {
      await caller.setDisqualified({
        id: randomUUID(),
        tournamentId: randomUUID(),
      } as any);
      assert.fail('Should have thrown validation error for missing disqualified');
    } catch (error: any) {
      assert.ok(error.message.includes('disqualified') || error.name === 'ZodError' || error.code === 'BAD_REQUEST');
    }

    // Test invalid type for disqualified (should be boolean)
    try {
      await caller.setDisqualified({
        id: randomUUID(),
        tournamentId: randomUUID(),
        disqualified: 'true' as any, // string instead of boolean
      });
      assert.fail('Should have thrown validation error for invalid disqualified type');
    } catch (error: any) {
      assert.ok(error.message.includes('disqualified') || error.name === 'ZodError' || error.code === 'BAD_REQUEST');
    }

    // Test invalid type for id (should be string)
    try {
      await caller.setDisqualified({
        id: 123 as any, // number instead of string
        tournamentId: randomUUID(),
        disqualified: true,
      });
      assert.fail('Should have thrown validation error for invalid id type');
    } catch (error: any) {
      assert.ok(error.message.includes('id') || error.name === 'ZodError' || error.code === 'BAD_REQUEST');
    }
  });

  it('should handle toggling disqualified status multiple times', async () => {
    // Arrange: Create a tournament and result
    const tournamentId = randomUUID();
    const resultId = randomUUID();

    await db.collection('tournaments').insertOne({
      _id: tournamentId,
      name: 'Test Tournament 4',
      date: new Date().toISOString(),
    });

    await db.collection('results').insertOne({
      _id: resultId,
      tournamentId,
      name: 'Test Dog 4',
      obstacles: [],
      disqualified: false,
    });

    const caller = appRouter.createCaller({});

    // Act & Assert: Toggle disqualified several times
    let result = await caller.setDisqualified({
      id: resultId,
      tournamentId,
      disqualified: true,
    });
    assert.strictEqual(result.result.disqualified, true);

    result = await caller.setDisqualified({
      id: resultId,
      tournamentId,
      disqualified: false,
    });
    assert.strictEqual(result.result.disqualified, false);

    result = await caller.setDisqualified({
      id: resultId,
      tournamentId,
      disqualified: true,
    });
    assert.strictEqual(result.result.disqualified, true);

    // Verify final state in database
    const storedResult = await db.collection('results').findOne({ _id: resultId });
    assert.strictEqual(storedResult?.disqualified, true);
  });

  it('should preserve other result fields when setting disqualified', async () => {
    // Arrange: Create a result with obstacles data
    const tournamentId = randomUUID();
    const resultId = randomUUID();
    const obstacles = [
      { obstacleKey: 'aframe', value: 1 },
      { obstacleKey: 'jump', value: 2 },
    ];

    await db.collection('tournaments').insertOne({
      _id: tournamentId,
      name: 'Test Tournament 5',
      date: new Date().toISOString(),
    });

    await db.collection('results').insertOne({
      _id: resultId,
      tournamentId,
      name: 'Test Dog 5',
      obstacles,
      disqualified: false,
    });

    // Act: Set disqualified to true
    const caller = appRouter.createCaller({});
    const result = await caller.setDisqualified({
      id: resultId,
      tournamentId,
      disqualified: true,
    });

    // Assert: Other fields should remain unchanged
    assert.strictEqual(result.result.name, 'Test Dog 5');
    assert.deepStrictEqual(result.result.obstacles, obstacles);
    assert.strictEqual(result.result.disqualified, true);
  });
});
