interface GoogleAuthConfig {
  clientId: string;
  androidClientId: string;
  iosClientId: string;
  webClientId: string;
  redirectUri: string;
  scopes: string[];
}

interface AppConfig {
  apiUrl: string;
  googleAuth: GoogleAuthConfig;
  environment: 'development' | 'production';
  isDebug: boolean;
}

// Development configuration
const devConfig: AppConfig = {
  environment: 'development',
  isDebug: true,
  apiUrl: 'http://localhost:3000',
  googleAuth: {
    clientId: 'YOUR_DEV_GOOGLE_CLIENT_ID.apps.googleusercontent.com',
    androidClientId: 'YOUR_DEV_ANDROID_CLIENT_ID.apps.googleusercontent.com',
    iosClientId: 'YOUR_DEV_IOS_CLIENT_ID.apps.googleusercontent.com',
    webClientId: 'YOUR_DEV_WEB_CLIENT_ID.apps.googleusercontent.com',
    redirectUri: 'agility-scoring://oauth2redirect/google',
    scopes: ['openid', 'email', 'profile']
  }
};

// Production configuration
const prodConfig: AppConfig = {
  environment: 'production',
  isDebug: false,
  apiUrl: 'https://api.yourdomain.com',
  googleAuth: {
    clientId: 'YOUR_PROD_GOOGLE_CLIENT_ID.apps.googleusercontent.com',
    androidClientId: 'YOUR_PROD_ANDROID_CLIENT_ID.apps.googleusercontent.com',
    iosClientId: 'YOUR_PROD_IOS_CLIENT_ID.apps.googleusercontent.com',
    webClientId: 'YOUR_PROD_WEB_CLIENT_ID.apps.googleusercontent.com',
    redirectUri: 'agility-scoring://oauth2redirect/google',
    scopes: ['openid', 'email', 'profile']
  }
};

// Determine which config to use
const getConfig = (): AppConfig => {
  // Check if we're in production mode
  const isProduction = process.env.NODE_ENV === 'production' || process.env.APP_VARIANT === 'release';
  
  // You can also use environment variables directly
  const envConfig = {
    clientId: isProduction 
      ? process.env.PROD_GOOGLE_CLIENT_ID 
      : process.env.DEV_GOOGLE_CLIENT_ID,
    androidClientId: isProduction
      ? process.env.PROD_GOOGLE_ANDROID_CLIENT_ID
      : process.env.DEV_GOOGLE_ANDROID_CLIENT_ID,
    iosClientId: isProduction
      ? process.env.PROD_GOOGLE_IOS_CLIENT_ID
      : process.env.DEV_GOOGLE_IOS_CLIENT_ID,
    webClientId: isProduction
      ? process.env.PROD_GOOGLE_WEB_CLIENT_ID
      : process.env.DEV_GOOGLE_WEB_CLIENT_ID,
  };
  
  // If environment variables are available, use them
  if (envConfig.clientId && envConfig.androidClientId) {
    return {
      environment: isProduction ? 'production' : 'development',
      isDebug: !isProduction,
      apiUrl: isProduction 
        ? process.env.PROD_API_URL || 'https://api.yourdomain.com'
        : process.env.DEV_API_URL || 'http://localhost:3000',
      googleAuth: {
        clientId: envConfig.clientId,
        androidClientId: envConfig.androidClientId,
        iosClientId: envConfig.iosClientId || envConfig.clientId,
        webClientId: envConfig.webClientId || envConfig.clientId,
        redirectUri: 'agility-scoring://oauth2redirect/google',
        scopes: ['openid', 'email', 'profile']
      }
    };
  }
  
  // Fallback to hardcoded config (for development without .env file)
  return isProduction ? prodConfig : devConfig;
};

export const Config = getConfig();

export type { AppConfig, GoogleAuthConfig };