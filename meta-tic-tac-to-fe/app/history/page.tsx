'use client';

import {ChangeEvent, ChangeEventHandler, useEffect, useState} from 'react';
import {PlayedGame} from '@/app/types';
import Game from '@/app/Game';
import {useGameStore} from '@/app/game-store';

export default function HistoryPage() {
  const nextStep = useGameStore(state => state.nextStep);
  const [playedGames, setPlayedGames] = useState<PlayedGame[]>([]);
  const [viewGame, setViewGame] = useState<PlayedGame | null>(null);

  const fetchPlayedGames = () => {
    fetch('http://localhost:3000/api/history')
      .then(res => res.json())
      .then(games => setPlayedGames(games));
  }

  useEffect(() => fetchPlayedGames(), []);

  useEffect(() => {
    const interval = setInterval(() => {
      nextStep();
    }, 2000)

    return () => {
      clearInterval(interval);
    }
  }, [viewGame]);

  const handleChange = (e: ChangeEvent<HTMLSelectElement>) => {
    setViewGame(e.target.value ? playedGames.find(g => g.id === e.target.value)! : null);
  }

  return (
    <main id={'history-page'} className={'min-w-[800px]'}>
      <h1 className={'text-3xl mb-4'}>Historie</h1>

      <div className="w-full flex justify-between">
        <select value={viewGame?.id}
                className={'w-[480px] border border-slate-400 p-4 bg-white rounded-xl'}
                onChange={handleChange}>
          <option value="">Such dir was aus...</option>
          {playedGames.map(p => <option key={p.id} value={p.id}>{p.id}</option>)}
        </select>

        <button type={'button'} className={'border border-slate-400 rounded-xl p-4'} onClick={() => fetchPlayedGames()}>
          Neu laden
        </button>
      </div>

      <button onClick={() => nextStep()}>Next step</button>

      {viewGame &&
        <Game playedGame={viewGame}/>
      }
    </main>
  );
}
