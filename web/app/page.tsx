import { redirect } from 'next/navigation'
import { currentProfile, homeOf } from '@/lib/auth'

// The home page has nothing of its own: it sends each account to its view.
export default async function Home() {
  const { role } = await currentProfile()

  redirect(homeOf[role])
}
