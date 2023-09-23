'use client';

import Game from '@/app/Game';
import Link from 'next/link';
import {StartScreen} from '@/app/StartScreen';
import {useGameStore} from '@/app/game-store';

export default function GamePage() {
  const isRunning = useGameStore(state => state.isRunning);

  return (
    <main id={'manual-game'}>
      <div id={'link-container'} className={'absolute top-8 right-4 z-50'}>
        <Link href={'/history'} className={'border border-amber-600 bg-amber-500 hover:bg-amber-400 p-4 rounded-xl'}>
          Historie
        </Link>
      </div>

      <h1 className={'text-5xl text-center mb-16 mt-8 -rotate-2'}>JAckathon - Meta Tic-Tac-Toe</h1>
      {!isRunning && <StartScreen/>}
      <Game/>
    </main>
  );
}
