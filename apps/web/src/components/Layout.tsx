import { Link, useLocation } from 'react-router-dom'
import { FaCog, FaPlus, FaHome } from 'react-icons/fa'

export default function Layout({ children }: { children: React.ReactNode }) {
  const location = useLocation()

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-primary text-white p-4 shadow-md">
        <div className="container mx-auto flex justify-between items-center">
          <h1 className="text-xl font-bold">🐕 Agility Scoring</h1>
          <nav className="flex items-center space-x-4">
            <Link to="/" className={`flex items-center space-x-1 ${location.pathname === '/' ? 'font-bold' : ''}`}>
              <FaHome />
              <span>Home</span>
            </Link>
            <Link to="/settings" className={`flex items-center space-x-1 ${location.pathname === '/settings' ? 'font-bold' : ''}`}>
              <FaCog />
              <span>Settings</span>
            </Link>
          </nav>
        </div>
      </header>

      <main className="container mx-auto p-4 flex-1">
        {children}
      </main>

      <div className="fixed bottom-6 right-6">
        <Link to="/add-tournament" className="w-12 h-12 bg-primary rounded-full flex items-center justify-center text-white shadow-lg hover:bg-blue-600 transition-colors">
          <FaPlus size={20} />
        </Link>
      </div>
    </div>
  )
}