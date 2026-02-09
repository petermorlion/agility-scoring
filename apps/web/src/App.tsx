import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { trpc, trpcClient } from './utils/trpc'
import HomePage from './pages/HomePage'
import SettingsPage from './pages/SettingsPage'
import LoginPage from './pages/LoginPage'
import AddTournamentPage from './pages/AddTournamentPage'
import TournamentPage from './pages/TournamentPage'
import Layout from './components/Layout'
import TestPage from './test-page'

const queryClient = new QueryClient()

function App() {
  return (
    <trpc.Provider client={trpcClient} queryClient={queryClient}>
      <QueryClientProvider client={queryClient}>
        <Router>
          <Layout>
            <Routes>
              <Route path="/test" element={<TestPage />} />
              <Route path="/" element={<HomePage />} />
              <Route path="/settings" element={<SettingsPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/add-tournament" element={<AddTournamentPage />} />
              <Route path="/tournament/:id" element={<TournamentPage />} />
            </Routes>
          </Layout>
        </Router>
      </QueryClientProvider>
    </trpc.Provider>
  )
}

export default App