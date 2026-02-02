import { z } from 'zod';
import { publicProcedure, router } from '../trpc';
import { getDb } from '../db';
import { randomUUID } from 'crypto';

export const tournamentsRouter = router({
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
});
