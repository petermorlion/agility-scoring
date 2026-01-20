import { View, Text, StyleSheet, ScrollView, ActivityIndicator, TouchableOpacity } from 'react-native';
import { router } from 'expo-router';
import { trpc } from '../utils/trpc';

export default function Index() {
  // Query the tournaments
  const tournamentsQuery = trpc.getTournaments.useQuery();

  return (
    <View style={styles.container}>
      <ScrollView style={styles.scrollView}>
      <View style={styles.header}>
        <Text style={styles.title}>🐕 Agility Scoring App</Text>
        <Text style={styles.subtitle}>Tournaments</Text>
      </View>

      {/* Tournaments List */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Tournaments</Text>
        {tournamentsQuery.isLoading && <ActivityIndicator />}
        {tournamentsQuery.error && (
          <Text style={styles.error}>Error: {tournamentsQuery.error.message}</Text>
        )}
        {tournamentsQuery.data && (
          <View>
            {tournamentsQuery.data.tournaments.length === 0 ? (
              <Text style={styles.emptyText}>No tournaments yet. Create one to get started!</Text>
            ) : (
              tournamentsQuery.data.tournaments.map((tournament) => (
                <View key={tournament.id} style={styles.tournamentItem}>
                  <Text style={styles.text}>🏆 {tournament.name}</Text>
                  <Text style={styles.subtext}>
                    📅 {tournament.date}
                  </Text>
                </View>
              ))
            )}
          </View>
        )}
      </View>

      <View style={styles.footer}>
        <Text style={styles.footerText}>
          ✨ Powered by tRPC, Expo & Express
        </Text>
      </View>
      </ScrollView>

      {/* Floating Action Button */}
      <TouchableOpacity
        style={styles.fab}
        onPress={() => router.push('/add-tournament')}
      >
        <Text style={styles.fabText}>+</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  scrollView: {
    flex: 1,
  },
  header: {
    padding: 20,
    backgroundColor: '#4a90e2',
    alignItems: 'center',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#fff',
    marginBottom: 5,
  },
  subtitle: {
    fontSize: 14,
    color: '#e0e0e0',
  },
  card: {
    margin: 15,
    padding: 20,
    backgroundColor: '#fff',
    borderRadius: 10,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  cardTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    marginBottom: 15,
    color: '#333',
  },
  cardSubtitle: {
    fontSize: 16,
    fontWeight: '600',
    marginTop: 15,
    marginBottom: 10,
    color: '#555',
  },
  text: {
    fontSize: 16,
    marginBottom: 5,
    color: '#333',
  },
  subtext: {
    fontSize: 14,
    color: '#666',
    marginBottom: 5,
  },
  tournamentItem: {
    marginBottom: 10,
    paddingBottom: 10,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  emptyText: {
    fontSize: 16,
    color: '#999',
    fontStyle: 'italic',
    textAlign: 'center',
    paddingVertical: 20,
  },
  error: {
    color: 'red',
    fontSize: 14,
  },
  footer: {
    padding: 20,
    alignItems: 'center',
  },
  footerText: {
    fontSize: 14,
    color: '#666',
  },
  fab: {
    position: 'absolute',
    right: 20,
    bottom: 20,
    width: 60,
    height: 60,
    borderRadius: 30,
    backgroundColor: '#4a90e2',
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 8,
  },
  fabText: {
    fontSize: 32,
    color: '#fff',
    fontWeight: '300',
  },
});
