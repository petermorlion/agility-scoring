import { View, Text, TouchableOpacity, ScrollView, ActivityIndicator, TextInput, Image } from 'react-native';
import { useLocalSearchParams, Stack } from 'expo-router';
import { trpc } from '../../utils/trpc';
import { useState, useEffect } from 'react';
import { t } from '../i18n';

type ObstacleKey = 'aframe' | 'dogwalk' | 'seesaw' | 'tunnel' | 'chute' | 'jump' | 'tire';

interface ObstacleData {
  key: ObstacleKey;
  label: string;
  icon: any;
}

const obstacles: ObstacleData[] = [
  { key: 'aframe', label: 'A-Frame', icon: require('../../assets/obstacles/a-frame.png') },
  { key: 'dogwalk', label: 'Dog Walk', icon: require('../../assets/obstacles/dog-walk.png') },
  { key: 'seesaw', label: 'Seesaw', icon: require('../../assets/obstacles/seesaw.png') },
  { key: 'tunnel', label: 'Tunnel', icon: require('../../assets/obstacles/tunnel.png') },
  { key: 'chute', label: 'Chute', icon: require('../../assets/obstacles/chute.png') },
  { key: 'jump', label: 'Jump', icon: require('../../assets/obstacles/hurdle-jump.png') },
  { key: 'tire', label: 'Tire', icon: require('../../assets/obstacles/tire-jump.png') },
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
  const [contestantName, setContestantName] = useState<string>('');
  const [isEditingName, setIsEditingName] = useState<boolean>(false);

  const tournamentQuery = trpc.getTournament.useQuery({ id });
  const upsertResultMutation = trpc.upsertResult.useMutation();
  const getResultQuery = trpc.getResult.useQuery(
    { id: currentResultId, tournamentId: id },
    { 
      enabled: !!currentResultId,
      refetchOnMount: 'always',
      refetchOnWindowFocus: true,
      staleTime: 0,
    }
  );

  // Load result data when query succeeds
  useEffect(() => {
    if (getResultQuery.data?.success && getResultQuery.data.result) {
      const result = getResultQuery.data.result;
      setResultId(result.id);
      setContestantName(result.name || '');
      
      // Convert obstacles array to values object
      const newValues = { ...values };
      result.obstacles.forEach((obstacle) => {
        newValues[obstacle.obstacleKey] = obstacle.value;
      });
      setValues(newValues);
    } else if (getResultQuery.data?.success === false) {
      // Reset values when no result is found
      setResultId(undefined);
      setContestantName('');
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
    setIsEditingName(false);
    getResultQuery.refetch();
  };

  const saveContestantName = async (name: string) => {
    setContestantName(name);
    
    // Convert values to obstacles array
    const obstaclesArray = Object.entries(values).map(([key, value]) => ({
      obstacleKey: key as ObstacleKey,
      value: value as number,
    }));

    // Call mutation
    const result = await upsertResultMutation.mutateAsync({
      id: resultId,
      tournamentId: id,
      name: name || undefined,
      obstacles: obstaclesArray,
    });

    // Store result ID after first creation
    if (!resultId && result.result?.id) {
      setResultId(result.result.id);
    }
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
      name: contestantName || undefined,
      obstacles: obstaclesArray,
    });

    // Store result ID after first creation
    if (!resultId && result.result?.id) {
      setResultId(result.result.id);
    }
  };

  return (
    <>
      <Stack.Screen 
        options={{ 
          title: tournamentQuery.data?.tournament?.name || t('tournament.defaultTitle')
        }} 
      />
      <ScrollView className="flex-1 bg-gray-100">
      <View className="p-5 bg-primary items-center">
        <View className="flex-row items-center justify-between w-full mb-1">
          <TouchableOpacity
            className="w-12 h-12 justify-center items-center"
            onPress={() => {
              const prevId = String(Math.max(1, parseInt(currentResultId) - 1));
              navigateToResult(prevId);
            }}
            disabled={currentResultId === '1'}
          >
            <Text className={`text-4xl text-white font-bold ${currentResultId === '1' ? 'opacity-30' : ''}`}>
              {t('tournament.prevArrow')}
            </Text>
          </TouchableOpacity>
          
          {isEditingName ? (
            <TextInput
              className="text-2xl font-bold text-white bg-white/20 px-4 py-2 rounded-lg min-w-[200px] text-center"
              value={contestantName}
              onChangeText={setContestantName}
              onBlur={() => {
                setIsEditingName(false);
                saveContestantName(contestantName);
              }}
              onSubmitEditing={() => {
                setIsEditingName(false);
                saveContestantName(contestantName);
              }}
              placeholder={t('tournament.enterNamePlaceholder')}
              placeholderTextColor="#b3d4ff"
              autoFocus
              returnKeyType="done"
            />
          ) : (
            <TouchableOpacity onPress={() => setIsEditingName(true)}>
              <Text className="text-2xl font-bold text-white">
                {contestantName || t('tournament.contestantPlaceholder')}
              </Text>
            </TouchableOpacity>
          )}
          
          <TouchableOpacity
            className="w-12 h-12 justify-center items-center"
            onPress={() => {
              const nextId = String(parseInt(currentResultId) + 1);
              navigateToResult(nextId);
            }}
          >
            <Text className="text-4xl text-white font-bold">{t('tournament.nextArrow')}</Text>
          </TouchableOpacity>
        </View>
      </View>

      <View className="m-4 p-5 bg-white rounded-xl shadow-sm">
        <Text className="text-xl font-bold mb-4 text-gray-800">{t('tournament.obstaclesTitle')}</Text>
        {obstacles.map((obstacle) => (
          <View key={obstacle.key} className="flex-row items-center justify-between py-3 border-b border-gray-200">
            <View className="flex-row items-center flex-1">
              <Image source={obstacle.icon} style={{ width: 28, height: 28, marginRight: 8 }} />
              <Text className="text-base text-gray-800 font-medium">{t(`obstacles.${obstacle.key}`)}</Text>
            </View>
            
            <View className="flex-row items-center">
              <TouchableOpacity
                className="w-10 h-10 rounded-full bg-primary justify-center items-center"
                onPress={() => updateValue(obstacle.key, -1)}
                disabled={upsertResultMutation.isPending}
              >
                <Text className="text-2xl text-white font-light">{t('tournament.decrement')}</Text>
              </TouchableOpacity>
              
              <View className="w-12 items-center justify-center">
                {upsertResultMutation.isPending ? (
                  <ActivityIndicator size="small" color="#4a90e2" />
                ) : (
                  <Text className="text-xl font-bold text-gray-800">{values[obstacle.key]}</Text>
                )}
              </View>
              
              <TouchableOpacity
                className="w-10 h-10 rounded-full bg-primary justify-center items-center"
                onPress={() => updateValue(obstacle.key, 1)}
                disabled={upsertResultMutation.isPending}
              >
                <Text className="text-2xl text-white font-light">{t('tournament.increment')}</Text>
              </TouchableOpacity>
            </View>
          </View>
        ))}
      </View>
    </ScrollView>
    </>
  );
}
