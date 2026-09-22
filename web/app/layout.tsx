import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = {
  title: 'Bragado Trip Connect',
  description: 'Todas las salidas entre Bragado y Buenos Aires en un solo lugar.',
}

export default function RootLayout({ children }: LayoutProps<'/'>) {
  return (
    <html lang="es">
      <body>{children}</body>
    </html>
  )
}
