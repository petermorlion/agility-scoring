import { initTRPC } from '@trpc/server';
import { z } from 'zod';

// In-memory storage for tournaments
interface Tournament {
  id: string;
  name: string;
  date: string;
}

const tournaments = new Map<string, Tournament>();
let nextId = 1;

// Initialize tRPC
const t = initTRPC.create();

// Export reusable router and procedure helpers
export const router = t.router;
export const publicProcedure = t.procedure;

// Define the app router with a dummy endpoint
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

  getTournaments: publicProcedure.query(() => {
    return {
      tournaments: Array.from(tournaments.values()),
    };
  }),

  upsertTournament: publicProcedure
    .input(z.object({
      id: z.string().optional(),
      name: z.string(),
      date: z.string(),
    }))
    .mutation(({ input }) => {
      const id = input.id || String(nextId++);
      const tournament: Tournament = {
        id,
        name: input.name,
        date: input.date,
      };
      
      tournaments.set(id, tournament);
      
      return {
        success: true,
        tournament,
        action: input.id ? 'updated' : 'created',
      };
    }),
});

// Export type definition of API
export type AppRouter = typeof appRouter;
