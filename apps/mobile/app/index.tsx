import { View, Text, ScrollView, ActivityIndicator, TouchableOpacity, RefreshControl } from 'react-native';
import { router } from 'expo-router';
import { trpc } from '../utils/trpc';
import "../global.css"

export default function Index() {
  // Query the tournaments
  const tournamentsQuery = trpc.getTournaments.useQuery();

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
      <View className="p-5 bg-primary items-center">
        <Text className="text-3xl font-bold text-white mb-1">🐕 Agility Scoring App</Text>
        <Text className="text-sm text-gray-200">Tournaments</Text>
      </View>

      {/* Tournaments List */}
      <View className="m-4 p-5 bg-white rounded-xl shadow-sm">
        <Text className="text-xl font-bold mb-4 text-gray-800">Tournaments</Text>
        {tournamentsQuery.isLoading && <ActivityIndicator />}
        {tournamentsQuery.error && (
          <Text className="text-red-600 text-sm">Error: {tournamentsQuery.error.message}</Text>
        )}
        {tournamentsQuery.data && (
          <View>
            {tournamentsQuery.data.tournaments.length === 0 ? (
              <Text className="text-base text-gray-500 italic text-center py-5">No tournaments yet. Create one to get started!</Text>
            ) : (
              tournamentsQuery.data.tournaments.map((tournament) => (
                <TouchableOpacity 
                  key={tournament.id} 
                  className="mb-2 pb-2 border-b border-gray-200"
                  onPress={() => router.push(`/tournament/${tournament.id}`)}
                >
                  <Text className="text-base mb-1 text-gray-800">🏆 {tournament.name}</Text>
                  <Text className="text-sm text-gray-600 mb-1">
                    📅 {tournament.date}
                  </Text>
                </TouchableOpacity>
              ))
            )}
          </View>
        )}
      </View>

      <View className="p-5 items-center">
        <Text className="text-sm text-gray-600">
          ✨ Powered by tRPC, Expo & Express
        </Text>
      </View>
      </ScrollView>

      {/* Floating Action Button */}
      
      <TouchableOpacity
        className="absolute right-5 bottom-5 w-14 h-14 rounded-full bg-primary justify-center items-center shadow-lg"
        onPress={() => router.push('/add-tournament')}
      >
        <Text className="text-4xl text-white font-light mb-2">+</Text>
      </TouchableOpacity>
    </View>
  );
}
