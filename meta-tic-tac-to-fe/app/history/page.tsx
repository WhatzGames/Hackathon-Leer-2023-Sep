'use client';

import {ChangeEvent, useEffect, useState} from 'react';
import {PlayedGame} from '@/app/types';
import Game from '@/app/Game';
import {next} from 'sucrase/dist/types/parser/tokenizer';
import {useGameStore} from '@/app/game-store';

export default function HistoryPage() {
  const setField = useGameStore(state => state.setField);
  const [playedGames, setPlayedGames] = useState<PlayedGame[]>([]);
  const [viewGame, setViewGame] = useState<PlayedGame | null>(null);
  const [step, setStep] = useState(0);

  const fetchPlayedGames = () => {
    fetch('http://localhost:3000/api/history')
      .then(res => res.json())
      .then(games => setPlayedGames(games));
  }

  useEffect(() => fetchPlayedGames(), []);

  const handleChange = (e: ChangeEvent<HTMLSelectElement>) => {
    setViewGame(e.target.value ? playedGames.find(g => g.id === e.target.value)! : null);
  }

  const nextStep = () => {
    if (!viewGame || step > (viewGame.log.length - 1)) {
      return;
    }

    setField(...viewGame.log[step].move);

    const nextStepIndex = step + 1;
    if (nextStepIndex < viewGame.log.length) {
      setStep(nextStepIndex);
    }
  }

  return (
    <main id={'history-page'} className={'h-screen py-4 min-w-[828px]'}>
      <h1 className={'text-3xl mb-4'}>Historie</h1>

      <div className="w-full flex justify-between mb-4">
        <div className="flex gap-2">
          <select value={viewGame?.id}
                  className={'border border-slate-400 p-4 bg-white rounded-xl'}
                  onChange={handleChange}>
            <option value="">Such dir was aus...</option>
            {playedGames.map(p => <option key={p.id} value={p.id}>{p.id}</option>)}
          </select>

          {viewGame &&
            <button type={'button'}
                    className={'border border-slate-400 hover:bg-slate-200 rounded-xl p-4'}
                    onClick={() => nextStep()}>
              Weiter
            </button>
          }
        </div>

        <button type={'button'} className={'border border-slate-400 hover:bg-slate-200 rounded-xl p-4'} onClick={() => fetchPlayedGames()}>
          Neu laden
        </button>
      </div>

      {viewGame && <Game view/>}
    </main>
  );
}
