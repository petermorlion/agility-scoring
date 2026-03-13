import { z } from 'zod';
import { publicProcedure, router } from '../trpc';
import { getDb } from '../db';
import { randomUUID } from 'crypto';

export const resultsRouter = router({
  upsertResult: publicProcedure
    .input(
      z.object({
        id: z.string().optional(),
        tournamentId: z.string(),
        name: z.string().optional(),
        obstacles: z.array(
          z.object({ obstacleKey: z.enum(['aframe', 'dogwalk', 'seesaw', 'tunnel', 'chute', 'jump', 'tire']), value: z.number() })
        ),
      })
    )
    .mutation(async ({ input }) => {
      const coll = getDb().collection('results');
      const id = input.id ?? randomUUID();
      const doc = { _id: id, tournamentId: input.tournamentId, name: input.name, obstacles: input.obstacles, disqualified: false };
      await coll.updateOne({ _id: id }, { $set: doc }, { upsert: true });
      const stored = await coll.findOne({ _id: id });
      return { success: true, result: stored, action: input.id ? 'updated' : 'created' };
    }),

  getResult: publicProcedure
    .input(z.object({ id: z.string(), tournamentId: z.string() }))
    .query(async ({ input }) => {
      const coll = getDb().collection('results');
      const result = await coll.findOne({ _id: input.id });
      if (!result || result.tournamentId !== input.tournamentId) return { success: false, result: null };
      // Ensure disqualified field exists for backward compatibility
      if (result && result.disqualified === undefined) {
        result.disqualified = false;
      }
      return { success: true, result };
    }),

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
    }),
});
