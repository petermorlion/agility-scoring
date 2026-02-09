import { useState } from 'react'
import { Link } from 'react-router-dom'
import { trpc } from '../utils/trpc'

export default function HomePage() {
  const tournamentsQuery = trpc.getTournaments.useQuery()
  const [refreshing, setRefreshing] = useState(false)

  const onRefresh = async () => {
    setRefreshing(true)
    try {
      await tournamentsQuery.refetch()
    } catch (error) {
      console.error('Failed to refresh tournaments:', error)
    } finally {
      setRefreshing(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="bg-primary text-white p-6 rounded-xl">
        <h2 className="text-2xl font-bold mb-2">Tournaments</h2>
        <p className="text-primary-100">Manage your agility competitions</p>
      </div>

      <div className="bg-white rounded-xl shadow-sm p-6">
        <h3 className="text-xl font-bold mb-4 text-gray-800">Your Tournaments</h3>

        {tournamentsQuery.isLoading && (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto"></div>
            <p className="mt-2 text-gray-600">Loading tournaments...</p>
          </div>
        )}

        {tournamentsQuery.error && (
          <div className="bg-red-50 border border-red-200 text-red-800 p-4 rounded-lg">
            <p>Error: {tournamentsQuery.error.message}</p>
          </div>
        )}

        {tournamentsQuery.data && (
          <div className="space-y-4">
            {tournamentsQuery.data.tournaments.length === 0 ? (
              <p className="text-center text-gray-500 italic py-5">
                No tournaments yet. Create one to get started!
              </p>
            ) : (
              tournamentsQuery.data.tournaments.map((tournament) => (
                <Link
                  key={tournament.id}
                  to={`/tournament/${tournament.id}`}
                  className="block mb-2 pb-2 border-b border-gray-200 hover:bg-gray-50 transition-colors"
                >
                  <div className="flex justify-between items-center">
                    <div>
                      <h4 className="text-base font-medium text-gray-800">🏆 {tournament.name}</h4>
                      <p className="text-sm text-gray-600">📅 {tournament.date}</p>
                    </div>
                    <span className="text-primary">→</span>
                  </div>
                </Link>
              ))
            )}
          </div>
        )}
      </div>

      <div className="text-center text-sm text-gray-600">
        ✨ Powered by tRPC, React & Vite
      </div>
    </div>
  )
}