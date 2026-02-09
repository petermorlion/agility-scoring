import { Link } from 'react-router-dom'
import { FaGoogle } from 'react-icons/fa'

export default function SettingsPage() {
  return (
    <div className="max-w-2xl mx-auto space-y-8">
      <div className="bg-white rounded-xl shadow-sm p-6">
        <h2 className="text-xl font-bold mb-4 text-gray-800">Language</h2>
        <div className="space-y-2">
          {[
            { code: 'en', label: 'English' },
            { code: 'fr', label: 'Français' },
            { code: 'de', label: 'Deutsch' },
            { code: 'nl', label: 'Nederlands' }
          ].map((lang) => (
            <button
              key={lang.code}
              className="w-full p-3 rounded-lg bg-white border border-gray-200 text-left hover:bg-gray-50 transition-colors"
            >
              {lang.label}
            </button>
          ))}
        </div>
      </div>

      <div className="bg-white rounded-xl shadow-sm p-6">
        <h2 className="text-xl font-bold mb-4 text-gray-800">Account</h2>
        <Link
          to="/login"
          className="w-full block p-3 rounded-lg bg-white border border-gray-200 text-center hover:bg-gray-50 transition-colors"
        >
          <div className="flex items-center justify-center space-x-2">
            <FaGoogle className="text-red-500" />
            <span className="text-primary font-medium">Login with Google</span>
          </div>
        </Link>
      </div>
    </div>
  )
}