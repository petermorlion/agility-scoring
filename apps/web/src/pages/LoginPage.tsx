import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { FaGoogle } from 'react-icons/fa'
import { google } from '@better-auth/core/social-providers'

export default function LoginPage() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  const handleGoogleLogin = async () => {
    try {
      setLoading(true)
      setError(null)

      // Initialize better-auth with Google provider
      const googleProvider = google({
        clientId: '650791542042-uaimjpjvrg8tm8s671n6e0fj604rs5b4.apps.googleusercontent.com', // Replace with your actual Google Client ID
        accessType: 'offline',
        display: 'popup'
      })

      // Generate authorization URL
      const state = 'random_state_string' // In production, use a proper state management
      const codeVerifier = 'random_code_verifier' // In production, use PKCE
      const redirectURI = 'http://localhost:3001/oauth2redirect/google' // Your web app's redirect URI
      
      const authorizationURL = await googleProvider.createAuthorizationURL({
        state,
        codeVerifier,
        redirectURI,
        scopes: ['openid', 'email', 'profile'],
      })

      console.log('Authorization URL:', authorizationURL.toString())
      
      // Open the URL in a new window for Google authentication
      window.location.href = authorizationURL.toString()
      
      // In a real app, you would:
      // 1. Handle the redirect back to your app
      // 2. Extract the authorization code from the URL
      // 3. Exchange the code for tokens
      // 4. Validate the tokens and get user info
      
      // For this demo, we'll simulate a successful login
      setTimeout(() => {
        navigate('/')
      }, 2000)

    } catch (error) {
      console.error('Google login error:', error)
      setError(error instanceof Error ? error.message : 'Failed to login with Google')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-md mx-auto mt-10">
      <div className="bg-white rounded-xl shadow-sm p-8">
        <h1 className="text-2xl font-bold text-center mb-6 text-gray-800">Login</h1>
        <p className="text-center text-gray-600 mb-8">Sign in to access your account</p>

        <button
          onClick={handleGoogleLogin}
          disabled={loading}
          className={`w-full flex items-center justify-center bg-white border border-gray-300 rounded-lg p-4 mb-4 hover:bg-gray-50 transition-colors ${loading ? 'opacity-50 cursor-not-allowed' : ''}`}
        >
          <FaGoogle className="text-red-500 mr-3" />
          <span className="text-gray-700 font-medium">Continue with Google</span>
        </button>

        {loading && (
          <div className="text-center mt-4">
            <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-primary mx-auto"></div>
            <p className="mt-2 text-sm text-gray-600">Authenticating...</p>
          </div>
        )}

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-800 p-3 rounded-lg mt-4">
            <p className="text-sm">{error}</p>
          </div>
        )}

        <button
          onClick={() => navigate(-1)}
          disabled={loading}
          className="mt-6 text-sm text-primary hover:underline"
        >
          Back to app
        </button>
      </div>
    </div>
  )
}