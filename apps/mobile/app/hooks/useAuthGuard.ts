import { useEffect } from 'react'
import { router } from 'expo-router'
import { useAuth } from '../context/AuthContext'

export const useAuthGuard = (redirectTo: string = '/login') => {
  const { isAuthenticated, loading, checkAuth } = useAuth()

  useEffect(() => {
    const verifyAuth = async () => {
      if (loading) return

      const isAuth = await checkAuth()
      
      if (!isAuth) {
        // Store the current path to redirect back after login
        const currentPath = window.location.pathname
        if (currentPath !== redirectTo) {
          try {
            // For Expo Router, we can use the router object
            router.replace({
              pathname: redirectTo,
              params: { redirect: currentPath }
            })
          } catch (error) {
            console.error('Redirect failed:', error)
            // Fallback redirect
            router.replace(redirectTo)
          }
        }
      }
    }

    verifyAuth()
  }, [isAuthenticated, loading, checkAuth, redirectTo])

  return { isAuthenticated, loading }
}