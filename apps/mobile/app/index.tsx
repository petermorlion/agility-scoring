import { View, Text, TouchableOpacity, ScrollView } from 'react-native';
import { router } from 'expo-router';
import "../global.css"
import { useT } from './i18n';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import { useAuth } from './context/AuthContext';

export default function Index() {
  const t = useT();
  const { isAuthenticated } = useAuth();

  return (
    <View className="flex-1 bg-gray-100">
      <ScrollView className="flex-1">
        {/* Header */}
        <View className="p-5 bg-primary items-center relative">
          <Text className="text-3xl font-bold text-white mb-1">{t('index.header')}</Text>
          <Text className="text-sm text-gray-200 mb-1">{t('index.welcomeSubtitle')}</Text>
          <View className="absolute right-5 top-5">
            <TouchableOpacity onPress={() => router.push('/settings')}>
              <MaterialCommunityIcons name="cog" size={24} color="#fff" />
            </TouchableOpacity>
          </View>
        </View>

        {/* Welcome Content */}
        <View className="m-4 p-5 bg-white rounded-xl shadow-sm items-center">
          <MaterialCommunityIcons name="dog" size={80} color="#4a90e2" className="mb-4" />
          
          <Text className="text-2xl font-bold mb-4 text-gray-800 text-center">
            {t('index.welcomeTitle')}
          </Text>
          
          <Text className="text-base text-gray-600 mb-6 text-center">
            {t('index.welcomeDescription')}
          </Text>

          {/* Login Button */}
          <TouchableOpacity
            className="bg-primary rounded-lg px-8 py-3 mb-4"
            onPress={() => router.push('/login?redirect=/tournament-list')}
          >
            <Text className="text-white font-semibold text-lg">
              {isAuthenticated ? t('index.viewTournaments') : t('index.loginToContinue')}
            </Text>
          </TouchableOpacity>

          {/* Features */}
          <View className="w-full mt-8">
            <Text className="text-lg font-semibold mb-4 text-gray-800 text-center">
              {t('index.featuresTitle')}
            </Text>
            
            <View className="space-y-3">
              <View className="flex-row items-center">
                <MaterialCommunityIcons name="trophy" size={20} color="#4a90e2" className="mr-3" />
                <Text className="text-base text-gray-700">{t('index.featureManageTournaments')}</Text>
              </View>
              
              <View className="flex-row items-center">
                <MaterialCommunityIcons name="timer" size={20} color="#4a90e2" className="mr-3" />
                <Text className="text-base text-gray-700">{t('index.featureTrackPerformance')}</Text>
              </View>
              
              <View className="flex-row items-center">
                <MaterialCommunityIcons name="chart-line" size={20} color="#4a90e2" className="mr-3" />
                <Text className="text-base text-gray-700">{t('index.featureAnalyzeResults')}</Text>
              </View>
            </View>
          </View>
        </View>

        <View className="p-5 items-center">
          <Text className="text-sm text-gray-600">{t('index.poweredBy')}</Text>
        </View>
      </ScrollView>
    </View>
  );
}