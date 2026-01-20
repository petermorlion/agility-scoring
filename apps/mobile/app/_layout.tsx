import { Stack } from 'expo-router';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { httpBatchLink } from '@trpc/client';
import { trpc } from '../utils/trpc';
import { useState } from 'react';

export default function RootLayout() {
  const [queryClient] = useState(() => new QueryClient());
  const [trpcClient] = useState(() =>
    trpc.createClient({
      links: [
        httpBatchLink({
          // Update this URL to match your API server
          // For local development:
          // - iOS Simulator: http://localhost:3000
          // - Android Emulator: http://10.0.2.2:3000
          // - Physical device: http://<your-computer-ip>:3000
          url: 'http://localhost:3000/trpc',
        }),
      ],
    })
  );

  return (
    <trpc.Provider client={trpcClient} queryClient={queryClient}>
      <QueryClientProvider client={queryClient}>
        <Stack>
          <Stack.Screen name="index" options={{ title: 'Agility Scoring' }} />
          <Stack.Screen name="add-tournament" options={{ title: 'Add Tournament', presentation: 'modal' }} />
        </Stack>
      </QueryClientProvider>
    </trpc.Provider>
  );
}
