import { createTRPCReact } from '@trpc/react-query'
import { httpBatchLink } from '@trpc/client'
import type { AppRouter } from '@trpc/server' // This will be replaced with your actual API types

// Initialize tRPC client
const trpc = createTRPCReact<AppRouter>()

// Create client configuration
export const trpcClient = trpc.createClient({
  links: [
    httpBatchLink({
      url: '/trpc', // This will be proxied to the API server
    }),
  ],
})

export { trpc }