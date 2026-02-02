import { initTRPC } from '@trpc/server';
import { z } from 'zod';
import { getDb } from './db';
import { randomUUID } from 'crypto';

// Initialize tRPC
const t = initTRPC.create();

// Export reusable router and procedure helpers
export const router = t.router;
export const publicProcedure = t.procedure;

// Define the app router with endpoints backed by MongoDB
export const appRouter = router({
  hello: publicProcedure
    .input(z.object({ name: z.string().optional() }))
    .query(({ input }) => {
      return {
        greeting: `Hello ${input.name ?? 'World'}!`,
        timestamp: new Date().toISOString(),
        data: {
          users: [
            { id: 1, name: 'Alices', score: 95 },
            { id: 2, name: 'Bob', score: 87 },
            { id: 3, name: 'Charlie', score: 92 },
          ],
          message: 'This is dummy data from the API',
        },
      };
    }),

  getDummyData: publicProcedure.query(() => {
    return {
      competitions: [
        { id: 1, name: 'Spring Championship', date: '2026-03-15', status: 'upcoming' },
        { id: 2, name: 'Summer Open', date: '2026-06-20', status: 'upcoming' },
        { id: 3, name: 'Winter Classic', date: '2025-12-10', status: 'completed' },
      ],
      totalParticipants: 42,
      activeJudges: 8,
    };
  }),
  getTournaments: publicProcedure.query(async () => {
    const coll = getDb().collection('tournaments');
    const docs = await coll.find().toArray();
    const tournaments = docs.map((d: any) => ({ id: d._id, name: d.name, date: d.date }));
    return { tournaments };
  }),

  getTournament: publicProcedure
    .input(z.object({ id: z.string() }))
    .query(async ({ input }) => {
      const coll = getDb().collection('tournaments');
      const tournament = await coll.findOne({ _id: input.id });

      if (!tournament) {
        return { success: false, tournament: null };
      }

      return { success: true, tournament: { id: tournament._id, name: tournament.name, date: tournament.date } };
    }),

  upsertTournament: publicProcedure
    .input(z.object({ id: z.string().optional(), name: z.string(), date: z.string() }))
    .mutation(async ({ input }) => {
      const coll = getDb().collection('tournaments');
      const id = input.id ?? randomUUID();

      await coll.updateOne({ _id: id }, { $set: { name: input.name, date: input.date } }, { upsert: true });
      const tdoc = await coll.findOne({ _id: id });

      return { success: true, tournament: { id: tdoc._id, name: tdoc.name, date: tdoc.date }, action: input.id ? 'updated' : 'created' };
    }),

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
      const doc = { _id: id, tournamentId: input.tournamentId, name: input.name, obstacles: input.obstacles };
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
      return { success: true, result };
    }),
});

// Export type definition of API
export type AppRouter = typeof appRouter;
