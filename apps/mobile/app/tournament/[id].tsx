import { View, Text, StyleSheet, TouchableOpacity, ScrollView, ActivityIndicator } from 'react-native';
import { useLocalSearchParams } from 'expo-router';
import { trpc } from '../../utils/trpc';
import { useState, useEffect } from 'react';

type ObstacleKey = 'aframe' | 'dogwalk' | 'seesaw' | 'tunnel' | 'chute' | 'jump' | 'tire';

interface ObstacleData {
  key: ObstacleKey;
  label: string;
  emoji: string;
}

const obstacles: ObstacleData[] = [
  { key: 'aframe', label: 'A-Frame', emoji: '🔺' },
  { key: 'dogwalk', label: 'Dog Walk', emoji: '🚶' },
  { key: 'seesaw', label: 'Seesaw', emoji: '⚖️' },
  { key: 'tunnel', label: 'Tunnel', emoji: '🌀' },
  { key: 'chute', label: 'Chute', emoji: '📍' },
  { key: 'jump', label: 'Jump', emoji: '🦘' },
  { key: 'tire', label: 'Tire', emoji: '⭕' },
];

export default function TournamentScoring() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const [values, setValues] = useState<Record<ObstacleKey, number>>({
    aframe: 0,
    dogwalk: 0,
    seesaw: 0,
    tunnel: 0,
    chute: 0,
    jump: 0,
    tire: 0,
  });
  const [resultId, setResultId] = useState<string | undefined>(undefined);

  const upsertResultMutation = trpc.upsertResult.useMutation();

  const updateValue = async (obstacleKey: ObstacleKey, delta: number) => {
    const newValue = Math.max(0, values[obstacleKey] + delta);
    const newValues = { ...values, [obstacleKey]: newValue };
    setValues(newValues);

    // Convert values to obstacles array
    const obstaclesArray = Object.entries(newValues).map(([key, value]) => ({
      obstacleKey: key as ObstacleKey,
      value: value as number,
    }));

    // Call mutation
    const result = await upsertResultMutation.mutateAsync({
      id: resultId,
      tournamentId: id,
      obstacles: obstaclesArray,
    });

    // Store result ID after first creation
    if (!resultId && result.result?.id) {
      setResultId(result.result.id);
    }
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>Tournament Scoring</Text>
        <Text style={styles.subtitle}>Tournament ID: {id}</Text>
      </View>

      <View style={styles.card}>
        <Text style={styles.cardTitle}>Obstacles</Text>
        {obstacles.map((obstacle) => (
          <View key={obstacle.key} style={styles.obstacleRow}>
            <View style={styles.obstacleInfo}>
              <Text style={styles.emoji}>{obstacle.emoji}</Text>
              <Text style={styles.obstacleLabel}>{obstacle.label}</Text>
            </View>
            
            <View style={styles.controls}>
              <TouchableOpacity
                style={styles.button}
                onPress={() => updateValue(obstacle.key, -1)}
                disabled={upsertResultMutation.isPending}
              >
                <Text style={styles.buttonText}>−</Text>
              </TouchableOpacity>
              
              <View style={styles.valueContainer}>
                {upsertResultMutation.isPending ? (
                  <ActivityIndicator size="small" color="#4a90e2" />
                ) : (
                  <Text style={styles.value}>{values[obstacle.key]}</Text>
                )}
              </View>
              
              <TouchableOpacity
                style={styles.button}
                onPress={() => updateValue(obstacle.key, 1)}
                disabled={upsertResultMutation.isPending}
              >
                <Text style={styles.buttonText}>+</Text>
              </TouchableOpacity>
            </View>
          </View>
        ))}
      </View>

      {resultId && (
        <View style={styles.statusCard}>
          <Text style={styles.statusText}>✓ Result ID: {resultId}</Text>
        </View>
      )}
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
    fontSize: 24,
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
  obstacleRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  obstacleInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  emoji: {
    fontSize: 24,
    marginRight: 10,
  },
  obstacleLabel: {
    fontSize: 16,
    color: '#333',
    fontWeight: '500',
  },
  controls: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  button: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#4a90e2',
    justifyContent: 'center',
    alignItems: 'center',
  },
  buttonText: {
    fontSize: 24,
    color: '#fff',
    fontWeight: '300',
  },
  valueContainer: {
    width: 50,
    alignItems: 'center',
    justifyContent: 'center',
  },
  value: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
  },
  statusCard: {
    margin: 15,
    marginTop: 0,
    padding: 15,
    backgroundColor: '#e8f5e9',
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#4caf50',
  },
  statusText: {
    fontSize: 14,
    color: '#2e7d32',
    textAlign: 'center',
  },
});
