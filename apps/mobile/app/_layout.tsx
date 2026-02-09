import { Stack } from 'expo-router';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { httpBatchLink } from '@trpc/client';
import { trpc } from '../utils/trpc';
import { useState } from 'react';
import { LocaleProvider, useT } from './i18n';
import Constants from 'expo-constants';

export default function RootLayout() {
  const [queryClient] = useState(() => new QueryClient());

  const uri = Constants.expoConfig?.hostUri?.split(':').shift()?.concat(':3000') ?? 'localhost:3000';

  const [trpcClient] = useState(() =>
    trpc.createClient({
      links: [
        httpBatchLink({
          // Update this URL to match your API server
          // For local development:
          // - iOS Simulator: http://localhost:3000
          // - Android Emulator: http://10.0.2.2:3000
          // - Physical device: http://<your-computer-ip>:3000
          url: `http://${uri}/trpc`,
        }),
      ],
    })
  );

  return (
    <trpc.Provider client={trpcClient} queryClient={queryClient}>
      <QueryClientProvider client={queryClient}>
        <LocaleProvider>
          <Screens />
        </LocaleProvider>
      </QueryClientProvider>
    </trpc.Provider>
  );
}

// locale is initialized by LocaleProvider

function Screens() {
  const t = useT();

  return (
    <Stack>
      <Stack.Screen name="index" options={{ headerShown: false }} />
      <Stack.Screen name="add-tournament" options={{ title: t('layout.addTournamentTitle'), presentation: 'modal' }} />
      <Stack.Screen name="settings" options={{ title: t('settings.title') }} />
      <Stack.Screen name="login" options={{ title: t('login.title') }} />
    </Stack>
  );
}
