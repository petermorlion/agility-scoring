import { Link } from 'react-router-dom'

export default function TestPage() {
  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-md mx-auto bg-white rounded-xl shadow-sm p-6">
        <h1 className="text-2xl font-bold text-center mb-4 text-gray-800">✅ Web App Working!</h1>
        <p className="text-center text-gray-600 mb-6">
          The web application is running correctly. The white screen issue has been resolved.
        </p>
        
        <div className="space-y-3">
          <Link
            to="/"
            className="block w-full text-center bg-primary text-white py-3 rounded-lg hover:bg-blue-600 transition-colors"
          >
            Go to Home
          </Link>
          
          <Link
            to="/settings"
            className="block w-full text-center bg-gray-200 text-gray-800 py-3 rounded-lg hover:bg-gray-300 transition-colors"
          >
            Go to Settings
          </Link>
          
          <Link
            to="/login"
            className="block w-full text-center bg-white border border-gray-300 text-gray-800 py-3 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Go to Login
          </Link>
        </div>
        
        <div className="mt-6 text-center text-sm text-gray-500">
          <p>If you see this page, the web app is working correctly!</p>
          <p className="mt-2">Try navigating to different routes using the buttons above.</p>
        </div>
      </div>
    </div>
  )
}