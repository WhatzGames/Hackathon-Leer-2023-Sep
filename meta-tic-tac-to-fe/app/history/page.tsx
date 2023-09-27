'use client';

import {ChangeEvent, useEffect, useState} from 'react';
import {PlayedGame} from '@/app/types';
import Game from '@/app/Game';
import {useGameStore} from '@/app/game-store';
import {LucidePause, LucidePlay, LucideRefreshCcw} from 'lucide-react';
import {Button} from '@/components/Button';
import {randomName} from '@/lib/random-name';
import Link from 'next/link';

export default function HistoryPage() {
  const setField = useGameStore(state => state.setField);
  const wait = useGameStore(state => state.wait);
  const setPlayers = useGameStore(state => state.setPlayers);
  const [play, setPlay] = useState(false);
  const [playedGames, setPlayedGames] = useState<PlayedGame[]>([]);
  const [viewGame, setViewGame] = useState<PlayedGame | null>(null);
  const [step, setStep] = useState(0);

  const fetchPlayedGames = () => {
    fetch('/api/history')
      .then(res => res.json())
      .then(games => setPlayedGames(games));
  };

  useEffect(() => fetchPlayedGames(), []);

  useEffect(() => {
    let interval: ReturnType<typeof setInterval>;

    if (play) {
      interval = setInterval(() => {
        nextStep(step);
      }, 2000);
    }

    return () => {
      if (interval) {
        clearInterval(interval);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [step, play]);

  const handleChange = (e: ChangeEvent<HTMLSelectElement>) => {
    const game = e.target.value ? playedGames.find(g => g.id === e.target.value)! : null;
    setViewGame(game);
    onNewGame(game);
  };

  const nextStep = (step: number) => {
    if (!viewGame || step > (viewGame.log.length - 1)) {
      return;
    }

    setField(...viewGame.log[step].move);

    const nextStepIndex = step + 1;
    if (nextStepIndex < viewGame.log.length) {
      setStep(nextStepIndex);
    }
  };

  const onNewGame = (game: PlayedGame | null = null) => {
    setPlay(false);

    game = game || viewGame;

    if (game) {
      game.players.forEach(player => {
        player.name = player.id === game!.self ? 'JAckathon' : randomName();
      });
      setPlayers(game?.players || []);
      setStep(0);
    }
  }

  const togglePlay = () => {
    setPlay(prev => !prev);
  }

  return (
    <main id={'history-page'} className={'min-w-[828px]'}>
      <div id={'link-container'} className={'absolute top-8 right-4 z-50'}>
        <Link href={'/'} className={'border border-amber-600 bg-amber-500 hover:bg-amber-400 p-4 rounded-xl'}>
          Selber spielen?
        </Link>
      </div>

      <h1 className={'text-5xl mb-8 -rotate-2'}>Historie</h1>

      <div className="w-full flex justify-between mb-4">
        <div className="flex gap-2">
          <select value={viewGame?.id}
                  className={'text-slate-800 border border-slate-400 hover:bg-slate-200 p-4 bg-white rounded-xl'}
                  onChange={handleChange}>
            <option value="">Such dir was aus...</option>
            {playedGames.map(p => <option key={p.id} value={p.id}>{p.id}</option>)}
          </select>

          <Button type={'button'}
                  disabled={play}
                  onClick={() => fetchPlayedGames()}>
            <LucideRefreshCcw/>
          </Button>

          {viewGame &&
            <>
              <Button type={'button'} onClick={togglePlay}>
                {play ? <LucidePause/> :<LucidePlay/>}
              </Button>
              <Button type={'button'} onClick={() => nextStep(step)} disabled={wait || play}>
                Weiter
              </Button>
            </>
          }
        </div>
      </div>

      {viewGame && <Game onNewGame={onNewGame} view/>}
    </main>
  );
}
