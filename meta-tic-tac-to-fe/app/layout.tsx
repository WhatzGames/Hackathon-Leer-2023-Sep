import './globals.css';
import type {Metadata} from 'next';
import {Acme, Josefin_Sans} from 'next/font/google';
import {cn} from '@/lib/cn';

const font = Acme({subsets: ['latin'], weight: ['400']});

export const metadata: Metadata = {
  title: 'JAckathon - Meta Tic-Tac-Toe',
  description: 'Proudly presented by journaway',
};

export default function RootLayout({
                                     children,
                                   }: {
  children: React.ReactNode
}) {
  return (
    <html lang="de">
    <body className={cn(font.className, 'w-screen h-screen bg-slate-700 text-white flex justify-center py-4')}>
    {children}
    </body>
    </html>
  );
}
