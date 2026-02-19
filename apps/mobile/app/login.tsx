import { View, Text, TouchableOpacity, ActivityIndicator, Alert, Linking, Platform } from 'react-native';
import { router, useLocalSearchParams } from 'expo-router';
import { useState, useEffect } from 'react';
import '../global.css';
import { useT } from './i18n';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import { useAuth } from './context/AuthContext';
import { Config } from '../config';

// Import better-auth library
import { betterAuth } from 'better-auth';
import { google } from '@better-auth/core/social-providers';

export default function Login() {
  const [loading, setLoading] = useState(false);
  const t = useT();
  const { redirect } = useLocalSearchParams<{ redirect?: string }>();
  const { login } = useAuth();

  // Handle deep linking for OAuth callback
  useEffect(() => {
    const handleDeepLink = (event: { url: string }) => {
      const url = event.url;
      console.log('Deep link received:', url);
      
      // Check if this is our Google OAuth redirect
      if (url.startsWith(Config.googleAuth.redirectUri)) {
        // Extract the authorization code from the URL
        const urlObj = new URL(url);
        const code = urlObj.searchParams.get('code');
        const state = urlObj.searchParams.get('state');
        const error = urlObj.searchParams.get('error');
        
        if (error) {
          console.error('Google OAuth error:', error);
          Alert.alert(t('login.errorTitle'), `${t('login.errorMessage')}: ${error}`);
          return;
        }
        
        if (code && state) {
          // In a real implementation, you would exchange the code for tokens here
          console.log('Authorization code received:', code);
          
          // For now, we'll simulate a successful login with mock user data
          const mockUser = {
            id: 'google-12345',
            name: 'Test User',
            email: 'test@example.com'
          };
          
          login(mockUser).then(() => {
            Alert.alert(
              t('login.successTitle'),
              `${t('login.successMessage')} ${mockUser.name}`
            );
            
            if (redirect) {
              router.replace(redirect);
            } else {
              router.replace('/tournament-list');
            }
          });
        }
      }
    };
    
    // Add event listener for deep links
    const subscription = Linking.addEventListener('url', handleDeepLink);
    
    // Check if app was launched from a deep link
    Linking.getInitialURL().then(url => {
      if (url) {
        handleDeepLink({ url });
      }
    });
    
    return () => {
      subscription.remove();
    };
  }, [redirect, t, login]);

  const handleGoogleLogin = async () => {
    try {
      setLoading(true);
      
      // Use the appropriate client ID based on platform
      const clientId = Platform.OS === 'android'
        ? Config.googleAuth.androidClientId
        : Platform.OS === 'ios'
          ? Config.googleAuth.iosClientId
          : Config.googleAuth.webClientId;

      // Initialize better-auth with Google provider using config
      const googleProvider = google({
        clientId: clientId,
        accessType: 'offline',
        display: 'popup'
      });

      // Generate authorization URL
      const state = 'random_state_string'; // In production, use a proper state management
      const codeVerifier = 'random_code_verifier'; // In production, use PKCE
      const redirectURI = Config.googleAuth.redirectUri; // Your app's deep link URI
      
      const authorizationURL = await googleProvider.createAuthorizationURL({
        state,
        codeVerifier,
        redirectURI,
        scopes: ['openid', 'email', 'profile'],
      });

      console.log('Authorization URL:', authorizationURL.toString());
      
      // Open the URL in the browser for Google authentication
      await Linking.openURL(authorizationURL.toString());
      
      // The deep link handler (useEffect) will take care of the rest
      // when Google redirects back to our app
      
    } catch (error) {
      console.error('Google login error:', error);
      
      let errorMessage = t('login.errorMessage');
      if (error instanceof Error) {
        errorMessage += `\n${error.message}`;
      }
      
      Alert.alert(t('login.errorTitle'), errorMessage);
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