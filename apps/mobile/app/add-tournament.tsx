import { View, Text, TextInput, TouchableOpacity, ScrollView, ActivityIndicator } from 'react-native';
import { router } from 'expo-router';
import { useState } from 'react';
import { trpc } from '../utils/trpc';
import '../global.css';

export default function AddTournament() {
  const [name, setName] = useState('');
  const [date, setDate] = useState('');
  
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
    if (!name.trim() || !date.trim()) {
      return;
    }
    
    mutation.mutate({
      name: name.trim(),
      date: date.trim(),
    });
  };

  return (
    <ScrollView className="flex-1 bg-gray-100">
      <View className="p-5">
        <Text className="text-2xl font-bold mb-8 text-gray-800">Add Tournament</Text>
        
        <View className="mb-5">
          <Text className="text-base font-semibold mb-2 text-gray-800">Tournament Name</Text>
          <TextInput
            className="bg-white rounded-lg p-4 text-base border border-gray-300 text-gray-800"
            value={name}
            onChangeText={setName}
            placeholder="Enter tournament name"
            placeholderTextColor="#999"
          />
        </View>

        <View className="mb-5">
          <Text className="text-base font-semibold mb-2 text-gray-800">Date</Text>
          <TextInput
            className="bg-white rounded-lg p-4 text-base border border-gray-300 text-gray-800"
            value={date}
            onChangeText={setDate}
            placeholder="YYYY-MM-DD"
            placeholderTextColor="#999"
          />
        </View>

        {mutation.error && (
          <Text className="text-red-600 text-sm mb-2">Error: {mutation.error.message}</Text>
        )}

        <TouchableOpacity
          className={`p-4 rounded-lg items-center mt-2 ${(!name.trim() || !date.trim() || mutation.isPending) ? 'bg-gray-400' : 'bg-blue-500'}`}
          onPress={handleSubmit}
          disabled={!name.trim() || !date.trim() || mutation.isPending}
        >
          {mutation.isPending ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text className="text-white text-base font-semibold">Create Tournament</Text>
          )}
        </TouchableOpacity>

        <TouchableOpacity
          className="p-4 rounded-lg items-center mt-2"
          onPress={() => router.back()}
          disabled={mutation.isPending}
        >
          <Text className="text-gray-600 text-base">Cancel</Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}
