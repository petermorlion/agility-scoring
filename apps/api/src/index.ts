import express from 'express';
import cors from 'cors';
import { createExpressMiddleware } from '@trpc/server/adapters/express';
import { appRouter } from './router';
import { connectToDb } from './db';

async function start() {
  // Ensure DB connection before starting the server
  await connectToDb();

  const app = express();
  const PORT = process.env.PORT || 3000;

  // Enable CORS for all origins (adjust in production)
  app.use(cors());

  // Health check endpoint
  app.get('/health', (req, res) => {
    res.json({ status: 'ok', timestamp: new Date().toISOString() });
  });

  // tRPC middleware
  app.use(
    '/trpc',
    createExpressMiddleware({
      router: appRouter,
      createContext: () => ({}),
    })
  );

  app.listen(PORT, () => {
    console.log(`🚀 API server running on http://localhost:${PORT}`);
    console.log(`📡 tRPC endpoint: http://localhost:${PORT}/trpc`);
  });
}

start().catch((err) => {
  console.error('Failed to start server', err);
  process.exit(1);
});
