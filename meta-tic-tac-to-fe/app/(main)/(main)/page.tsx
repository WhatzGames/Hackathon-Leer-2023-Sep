'use client';

import Game from '@/app/Game';
import Link from 'next/link';

export default function GamePage() {
  return (
    <main id={'manual-game'}>
      <div id={'link-container'} className={'absolute top-4 right-4'}>
        <Link href={'/history'} className={'border border-slate-400 p-4 rounded-xl hover:bg-slate-200'}>
          Historie
        </Link>
      </div>
      <Game/>
    </main>
  );
}
