import { useParams } from 'react-router-dom'

export default function TournamentPage() {
  const { id } = useParams<{ id: string }>()

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-xl shadow-sm p-6">
        <h2 className="text-xl font-bold mb-4 text-gray-800">Tournament Details</h2>
        <p className="text-gray-600 mb-4">Tournament ID: {id}</p>

        <div className="bg-gray-50 rounded-lg p-4">
          <h3 className="font-semibold text-gray-800 mb-3">Contestants</h3>
          <p className="text-gray-600 italic">Tournament details and contestants would be displayed here.</p>
        </div>

        <div className="mt-6 bg-gray-50 rounded-lg p-4">
          <h3 className="font-semibold text-gray-800 mb-3">Obstacles</h3>
          <p className="text-gray-600 italic">Obstacle configuration and scoring would be managed here.</p>
        </div>
      </div>
    </div>
  )
}