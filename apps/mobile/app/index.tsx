import { View, Text, StyleSheet, ScrollView, ActivityIndicator } from 'react-native';
import { trpc } from '../utils/trpc';

export default function Index() {
  // Query the hello endpoint
  const helloQuery = trpc.hello.useQuery({ name: 'Agility App' });
  
  // Query the dummy data endpoint
  const dummyDataQuery = trpc.getDummyData.useQuery();

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>🐕 Agility Scoring App</Text>
        <Text style={styles.subtitle}>Connected to tRPC API</Text>
      </View>

      {/* Hello Query */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Hello Endpoint</Text>
        {helloQuery.isLoading && <ActivityIndicator />}
        {helloQuery.error && (
          <Text style={styles.error}>Error: {helloQuery.error.message}</Text>
        )}
        {helloQuery.data && (
          <View>
            <Text style={styles.text}>{helloQuery.data.greeting}</Text>
            <Text style={styles.subtext}>
              Timestamp: {new Date(helloQuery.data.timestamp).toLocaleTimeString()}
            </Text>
            <Text style={styles.cardSubtitle}>Users:</Text>
            {helloQuery.data.data.users.map((user) => (
              <Text key={user.id} style={styles.text}>
                • {user.name} - Score: {user.score}
              </Text>
            ))}
          </View>
        )}
      </View>

      {/* Dummy Data Query */}
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Competitions Data</Text>
        {dummyDataQuery.isLoading && <ActivityIndicator />}
        {dummyDataQuery.error && (
          <Text style={styles.error}>Error: {dummyDataQuery.error.message}</Text>
        )}
        {dummyDataQuery.data && (
          <View>
            <Text style={styles.text}>
              Total Participants: {dummyDataQuery.data.totalParticipants}
            </Text>
            <Text style={styles.text}>
              Active Judges: {dummyDataQuery.data.activeJudges}
            </Text>
            <Text style={styles.cardSubtitle}>Competitions:</Text>
            {dummyDataQuery.data.competitions.map((comp) => (
              <View key={comp.id} style={styles.competitionItem}>
                <Text style={styles.text}>📅 {comp.name}</Text>
                <Text style={styles.subtext}>
                  {comp.date} • {comp.status}
                </Text>
              </View>
            ))}
          </View>
        )}
      </View>

      <View style={styles.footer}>
        <Text style={styles.footerText}>
          ✨ Powered by tRPC, Expo & Express
        </Text>
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
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
  competitionItem: {
    marginBottom: 10,
    paddingBottom: 10,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
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
});
