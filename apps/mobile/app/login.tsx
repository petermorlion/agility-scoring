import { View, Text, TouchableOpacity, ActivityIndicator, Alert, Linking } from 'react-native';
import { router } from 'expo-router';
import { useState, useEffect } from 'react';
import '../global.css';
import { useT } from './i18n';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';

// Import better-auth library
import { betterAuth } from 'better-auth';
import { google } from '@better-auth/core/social-providers';

export default function Login() {
  const [loading, setLoading] = useState(false);
  const t = useT();

  const handleGoogleLogin = async () => {
    try {
      setLoading(true);
      
      // Initialize better-auth with Google provider
      // Note: In a real app, you would configure this in your backend
      // and get the clientId from environment variables
      const googleProvider = google({
        clientId: 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com', // Replace with your actual Google Client ID
        accessType: 'offline',
        display: 'popup'
      });

      // Generate authorization URL
      const state = 'random_state_string'; // In production, use a proper state management
      const codeVerifier = 'random_code_verifier'; // In production, use PKCE
      const redirectURI = 'com.your.app:/oauth2redirect/google'; // Your app's deep link URI
      
      const authorizationURL = await googleProvider.createAuthorizationURL({
        state,
        codeVerifier,
        redirectURI,
        scopes: ['openid', 'email', 'profile'],
      });

      console.log('Authorization URL:', authorizationURL.toString());
      
      // Open the URL in the browser for Google authentication
      await Linking.openURL(authorizationURL.toString());
      
      // In a real app, you would:
      // 1. Handle the redirect back to your app
      // 2. Extract the authorization code from the URL
      // 3. Exchange the code for tokens
      // 4. Validate the tokens and get user info
      
      // For this demo, we'll simulate a successful login
      Alert.alert(
        t('login.successTitle'),
        `${t('login.successMessage')} User`
      );
      
      // Redirect to home page after successful login
      router.replace('/');
      
    } catch (error) {
      console.error('Google login error:', error);
      
      let errorMessage = t('login.errorMessage');
      if (error instanceof Error) {
        errorMessage += `\n${error.message}`;
      }
      
      Alert.alert(t('login.errorTitle'), errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View className="flex-1 bg-gray-100 justify-center items-center p-5">
      <View className="w-full max-w-md bg-white rounded-xl p-8 shadow-sm">
        <Text className="text-2xl font-bold text-center mb-6 text-gray-800">{t('login.title')}</Text>
        
        <Text className="text-base text-gray-600 text-center mb-8">
          {t('login.subtitle')}
        </Text>

        <TouchableOpacity
          className={`flex-row items-center justify-center bg-white border border-gray-300 rounded-lg p-4 mb-4 ${loading ? 'opacity-50' : ''}`}
          onPress={handleGoogleLogin}
          disabled={loading}
        >
          <MaterialCommunityIcons name="google" size={24} color="#DB4437" className="mr-3" />
          <Text className="text-base text-gray-700">{t('login.googleButton')}</Text>
        </TouchableOpacity>

        {loading && (
          <View className="mt-4">
            <ActivityIndicator size="large" color="#4a90e2" />
            <Text className="text-sm text-gray-600 mt-2 text-center">{t('login.loading')}</Text>
          </View>
        )}

        <TouchableOpacity
          className="mt-6"
          onPress={() => router.back()}
          disabled={loading}
        >
          <Text className="text-sm text-primary text-center">{t('login.backButton')}</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}