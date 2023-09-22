import './globals.css'
import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import {cn} from '@/lib/cn';

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'JAckathon - Meta Tic-Tac-Toe',
  description: 'Proudly presented by journaway',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="de">
      <body className={cn(inter.className, 'w-screen h-screen flex justify-center items-center')}>{children}</body>
    </html>
  )
}
