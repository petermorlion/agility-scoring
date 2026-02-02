import { router } from './trpc';
import { demoRouter } from './endpoints/demo';
import { tournamentsRouter } from './endpoints/tournaments';
import { resultsRouter } from './endpoints/results';

// Merge all endpoint routers into the main app router
export const appRouter = router({
  ...demoRouter._def.procedures,
  ...tournamentsRouter._def.procedures,
  ...resultsRouter._def.procedures,
});

// Export type definition of API
export type AppRouter = typeof appRouter;
