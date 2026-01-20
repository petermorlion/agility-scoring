import { createTRPCReact } from '@trpc/react-query';
import type { AppRouter } from '@agility-scoring/api/src/router';

export const trpc = createTRPCReact<AppRouter>();
