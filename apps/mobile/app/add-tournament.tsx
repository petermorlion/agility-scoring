import { View, Text, TextInput, TouchableOpacity, ScrollView, ActivityIndicator } from 'react-native';
import { router } from 'expo-router';
import { useState } from 'react';
import { trpc } from '../utils/trpc';
import DateTimePicker from '@react-native-community/datetimepicker';
import '../global.css';
import { t } from './i18n';

export default function AddTournament() {
  const [name, setName] = useState('');
  const [date, setDate] = useState(new Date());
  const [showDatePicker, setShowDatePicker] = useState(false);
  
  const utils = trpc.useContext();
  const mutation = trpc.upsertTournament.useMutation({
    onSuccess: () => {
      // Invalidate and refetch tournaments
      utils.getTournaments.invalidate();
      // Go back to the main screen
      router.back();
    },
  });

  const handleSubmit = () => {
    if (!name.trim()) {
      return;
    }
    
    mutation.mutate({
      name: name.trim(),
      date: date.toISOString().split('T')[0],
    });
  };

  return (
    <ScrollView className="flex-1 bg-gray-100">
      <View className="p-5">
        <Text className="text-2xl font-bold mb-8 text-gray-800">{t('addTournament.title')}</Text>
        
        <View className="mb-5">
          <Text className="text-base font-semibold mb-2 text-gray-800">{t('addTournament.tournamentNameLabel')}</Text>
          <TextInput
            className="bg-white rounded-lg p-4 text-base border border-gray-300 text-gray-800"
            value={name}
            onChangeText={setName}
            placeholder={t('addTournament.enterTournamentName')}
            placeholderTextColor="#999"
          />
        </View>

        <View className="mb-5">
          <Text className="text-base font-semibold mb-2 text-gray-800">{t('addTournament.dateLabel')}</Text>
          <TouchableOpacity
            className="bg-white rounded-lg p-4 border border-gray-300"
            onPress={() => setShowDatePicker(true)}
          >
            <Text className="text-base text-gray-800">
              {date.toISOString().split('T')[0]}
            </Text>
          </TouchableOpacity>
          {showDatePicker && (
            <DateTimePicker
              value={date}
              mode="date"
              display="default"
              onChange={(event, selectedDate) => {
                setShowDatePicker(false);
                if (selectedDate) {
                  setDate(selectedDate);
                }
              }}
            />
          )}
        </View>

        {mutation.error && (
          <Text className="text-red-600 text-sm mb-2">{t('addTournament.errorPrefix')}{mutation.error.message}</Text>
        )}

        <TouchableOpacity
          className={`p-4 rounded-lg bg-primary items-center mt-2 ${(!name.trim() || mutation.isPending) ? 'bg-gray-400' : 'bg-primary'}`}
          onPress={handleSubmit}
          disabled={!name.trim() || mutation.isPending}
        >
          {mutation.isPending ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text className="text-white text-base font-semibold">{t('addTournament.createButton')}</Text>
          )}
        </TouchableOpacity>

        <TouchableOpacity
          className="p-4 rounded-lg items-center mt-2"
          onPress={() => router.back()}
          disabled={mutation.isPending}
        >
          <Text className="text-gray-600 text-base">{t('addTournament.cancel')}</Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}
