import './globals.css';
import type {Metadata} from 'next';
import {Josefin_Sans} from 'next/font/google';
import {cn} from '@/lib/cn';

const font = Josefin_Sans({subsets: ['latin'], weight: ['400', '600']});

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
    <body className={cn(font.className, 'w-screen h-screen flex justify-center items-center')}>
    {children}
    </body>
    </html>
  );
}
