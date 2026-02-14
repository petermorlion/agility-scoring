import { View, Text, ScrollView, ActivityIndicator, TouchableOpacity, RefreshControl } from 'react-native';
import { router } from 'expo-router';
import { trpc } from '../utils/trpc';
import "../global.css"
import { useT } from './i18n';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import { useAuth } from './context/AuthContext';
import { useEffect } from 'react';

export default function TournamentListPage() {
  const { user, isAuthenticated, loading: authLoading } = useAuth();
  const tournamentsQuery = trpc.getTournaments.useQuery();
  const t = useT();

  // Redirect to login if not authenticated
  useEffect(() => {
    if (!isAuthenticated && !authLoading) {
      router.replace('/login?redirect=/tournament-list');
    }
  }, [isAuthenticated, authLoading]);

  if (authLoading) {
    return (
      <View className="flex-1 bg-gray-100 justify-center items-center">
        <ActivityIndicator size="large" color="#4a90e2" />
        <Text className="mt-4 text-gray-600">Loading auth...</Text>
      </View>
    );
  }

  if (!isAuthenticated) {
    return (
      <View className="flex-1 bg-gray-100 justify-center items-center">
        <ActivityIndicator size="large" color="#4a90e2" />
        <Text className="mt-4 text-gray-600">Redirecting to login...</Text>
      </View>
    );
  }

  const onRefresh = () => {
    tournamentsQuery.refetch();
  };

  return (
    <View className="flex-1 bg-gray-100">
      <ScrollView 
        className="flex-1"
        refreshControl={
          <RefreshControl
            refreshing={tournamentsQuery.isRefetching}
            onRefresh={onRefresh}
            colors={['#4a90e2']}
            tintColor="#4a90e2"
          />
        }
      >
      <View className="p-5 bg-primary items-center relative">
        <Text className="text-3xl font-bold text-white mb-1">{t('index.header')}</Text>
        {isAuthenticated && user ? (
          <Text className="text-sm text-gray-200 mb-1">Welcome, {user.name}!</Text>
        ) : (
          <Text className="text-sm text-gray-200 mb-1">{t('index.tournamentsLabelSmall')}</Text>
        )}
        <View className="absolute right-5 top-5 flex-row space-x-3">
          {isAuthenticated ? (
            <TouchableOpacity onPress={logout}>
              <MaterialCommunityIcons name="logout" size={24} color="#fff" />
            </TouchableOpacity>
          ) : (
            <TouchableOpacity onPress={() => router.push('/login')}>
              <MaterialCommunityIcons name="login" size={24} color="#fff" />
            </TouchableOpacity>
          )}
          <TouchableOpacity onPress={() => router.push('/settings')}>
            <MaterialCommunityIcons name="cog" size={24} color="#fff" />
          </TouchableOpacity>
        </View>
      </View>

      {/* Tournaments List */}
      <View className="m-4 p-5 bg-white rounded-xl shadow-sm">
        <Text className="text-xl font-bold mb-4 text-gray-800">{t('index.tournamentsCardTitle')}</Text>
        {tournamentsQuery.isLoading && <ActivityIndicator />}
        {tournamentsQuery.error && (
          <Text className="text-red-600 text-sm">{t('index.errorPrefix')}{tournamentsQuery.error.message}</Text>
        )}
        {tournamentsQuery.data && (
          <View>
              {tournamentsQuery.data.tournaments.length === 0 ? (
              <Text className="text-base text-gray-500 italic text-center py-5">{t('index.noTournaments')}</Text>
            ) : (
              tournamentsQuery.data.tournaments.map((tournament) => (
                <TouchableOpacity 
                  key={tournament.id} 
                  className="mb-2 pb-2 border-b border-gray-200"
                  onPress={() => router.push(`/tournament/${tournament.id}`)}
                >
                  <Text className="text-base mb-1 text-gray-800">{t('index.trophyPrefix')}{tournament.name}</Text>
                  <Text className="text-sm text-gray-600 mb-1">
                    {t('index.datePrefix')}{tournament.date}
                  </Text>
                </TouchableOpacity>
              ))
            )}
          </View>
        )}
      </View>

      <View className="p-5 items-center">
        <Text className="text-sm text-gray-600">{t('index.poweredBy')}</Text>
      </View>
      </ScrollView>

      {/* Floating Action Button */}
      
      <TouchableOpacity
        className="absolute right-5 bottom-5 w-14 h-14 rounded-full bg-primary justify-center items-center shadow-lg"
        onPress={() => {
          if (isAuthenticated) {
            router.push('/add-tournament');
          } else {
            router.push('/login');
          }
        }}
      >
        <Text className="text-4xl text-white font-light">{t('index.fabPlus')}</Text>
      </TouchableOpacity>
    </View>
  );
}