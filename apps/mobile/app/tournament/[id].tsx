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
  const [currentResultId, setCurrentResultId] = useState<string>('1');

  const upsertResultMutation = trpc.upsertResult.useMutation();
  const getResultQuery = trpc.getResult.useQuery(
    { id: currentResultId, tournamentId: id },
    { enabled: !!currentResultId }
  );

  // Load result data when query succeeds
  useEffect(() => {
    if (getResultQuery.data?.success && getResultQuery.data.result) {
      const result = getResultQuery.data.result;
      setResultId(result.id);
      
      // Convert obstacles array to values object
      const newValues = { ...values };
      result.obstacles.forEach((obstacle) => {
        newValues[obstacle.obstacleKey] = obstacle.value;
      });
      setValues(newValues);
    } else if (getResultQuery.data?.success === false) {
      // Reset values when no result is found
      setResultId(undefined);
      setValues({
        aframe: 0,
        dogwalk: 0,
        seesaw: 0,
        tunnel: 0,
        chute: 0,
        jump: 0,
        tire: 0,
      });
    }
  }, [getResultQuery.data]);

  const navigateToResult = (newResultId: string) => {
    setCurrentResultId(newResultId);
  };

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
        <View style={styles.navigationRow}>
          <TouchableOpacity
            style={styles.navButton}
            onPress={() => {
              const prevId = String(Math.max(1, parseInt(currentResultId) - 1));
              navigateToResult(prevId);
            }}
            disabled={currentResultId === '1'}
          >
            <Text style={[styles.navButtonText, currentResultId === '1' && styles.navButtonDisabled]}>
              ←
            </Text>
          </TouchableOpacity>
          
          <Text style={styles.title}>Contestant {currentResultId}</Text>
          
          <TouchableOpacity
            style={styles.navButton}
            onPress={() => {
              const nextId = String(parseInt(currentResultId) + 1);
              navigateToResult(nextId);
            }}
          >
            <Text style={styles.navButtonText}>→</Text>
          </TouchableOpacity>
        </View>
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
  navigationRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    width: '100%',
    marginBottom: 5,
  },
  navButton: {
    width: 50,
    height: 50,
    justifyContent: 'center',
    alignItems: 'center',
  },
  navButtonText: {
    fontSize: 32,
    color: '#fff',
    fontWeight: 'bold',
  },
  navButtonDisabled: {
    opacity: 0.3,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#fff',
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
